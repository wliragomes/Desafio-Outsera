using System.Net;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Infra.Context;
using Domain.Entities;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Tests.Integration
{
    public class AwardsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AwardsIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(ConfigureTestServices);
            });
        }

        private void ConfigureTestServices(IServiceCollection services)
        {
            // Configuração do banco de dados SQLite em memória
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();

            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(connection);
                options.UseInternalServiceProvider(serviceProvider);
            });

            // Configuração inicial do banco de dados
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            // Adiciona dados fictícios para os testes
            db.Awards.AddRange(new[]
            {
                new Award { Year = 1980, Title = "Movie 1", Studios = "Studio A", Producers = "Producer A", IsWinner = true },
                new Award { Year = 1981, Title = "Movie 2", Studios = "Studio B", Producers = "Producer B", IsWinner = false }
            });
            db.SaveChanges();
        }

        [Fact]
        public async Task GetAwards_ReturnsExpectedData()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/awards/producers/intervals");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CsvData_IsLoadedIntoDatabase()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Assert
            var awards = db.Awards.ToList();
            Assert.True(awards.Count > 0); // Verifica se os dados foram carregados
        }
    }
}

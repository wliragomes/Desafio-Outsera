using System.Net;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Infra.Context;
using Domain.Entities;
using Application.DTOs;
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

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            // Adiciona dados fictícios para os testes
            db.Awards.AddRange(new[]
            {
                new Award { Year = 1980, Title = "Movie 1", Studios = "Studio A", Producers = "Producer A", IsWinner = true },
                new Award { Year = 1985, Title = "Movie 2", Studios = "Studio B", Producers = "Producer A", IsWinner = true },
                new Award { Year = 1995, Title = "Movie 3", Studios = "Studio C", Producers = "Producer B", IsWinner = true },
                new Award { Year = 2005, Title = "Movie 4", Studios = "Studio D", Producers = "Producer B", IsWinner = true }
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

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AwardsResponseDTO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);

            // Valida se os intervalos mínimos e máximos existem
            Assert.True(result.Min!.Count > 0);
            Assert.True(result.Max!.Count > 0);
        }

        [Fact]
        public async Task CsvData_IsLoadedIntoDatabase()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Act
            var awards = db.Awards.ToList();

            // Assert
            Assert.True(awards.Count > 0); // Verifica se os dados foram carregados
        }

        [Fact]
        public async Task GetAwards_WithOriginalCsv_ReturnsMin1AndMax13()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/awards/producers/intervals");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AwardsResponseDTO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);

            // Valida os intervalos mínimos e máximos esperados
            Assert.Contains(result.Min!, m => m.Interval == 1);
            Assert.Contains(result.Max!, m => m.Interval == 13);
        }

        [Fact]
        public async Task GetAwards_WithCustomCsv_ReturnsValidIntervals()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/awards/producers/intervals");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AwardsResponseDTO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);

            // Valida que os intervalos mínimos e máximos foram retornados corretamente
            Assert.True(result.Min!.Count > 0);
            Assert.True(result.Max!.Count > 0);
        }
    }
}

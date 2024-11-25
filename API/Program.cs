using Domain.Entities;
using Infra.Context;
using Infra.IoC;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructureApi(builder.Configuration);

//---------------
// Banco de dados em memória
var inMemorySqliteConnection = new SqliteConnection("DataSource=:memory:");
inMemorySqliteConnection.Open(); // Mantém a conexão aberta enquanto a aplicação estiver rodando

// Configuração do DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(inMemorySqliteConnection)); // Configura o banco em memória para o contexto
//---------------

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Cria as tabelas no banco de dados em memória e carrega os dados do CSV
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated(); // Garante que as tabelas sejam criadas

    // Carrega os dados do CSV se a tabela estiver vazia
    if (!dbContext.Awards.Any())
    {
        Console.WriteLine("Carregando dados do arquivo CSV...");
        var filePath = Path.Combine(AppContext.BaseDirectory, "Data/movielist.csv");

        if (File.Exists(filePath))
        {
            var awardsList = new List<Award>();
            var lines = await File.ReadAllLinesAsync(filePath);

            foreach (var line in lines.Skip(1)) // Ignora o cabeçalho
            {
                var columns = line.Split(';');
                if (columns.Length >= 5)
                {
                    awardsList.Add(new Award
                    {
                        Year = int.Parse(columns[0], CultureInfo.InvariantCulture),
                        Title = columns[1],
                        Studios = columns[2],
                        Producers = columns[3],
                        IsWinner = columns[4].Trim().ToLower() == "yes"
                    });
                }
            }

            dbContext.Awards.AddRange(awardsList);
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Dados carregados com sucesso.");
        }
        else
        {
            Console.WriteLine($"Arquivo CSV não encontrado em {filePath}");
        }
    }
    else
    {
        Console.WriteLine("Dados já carregados no banco.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(p => p
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
);

app.MapControllers();

app.Run();

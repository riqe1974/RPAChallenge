using API.Repositories;
using Microsoft.Data.Sqlite;
using Shared.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.FromLogContext()
          .WriteTo.Console();
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar repositório SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=/data/quotes.db";
builder.Services.AddSingleton<IQuoteRepository>(sp => new SqliteQuoteRepository(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
using RPA.Services;
using Serilog;
using Shared.Interfaces;

namespace RPA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((context, config) =>
                {
                    config.ReadFrom.Configuration(context.Configuration)
                          .Enrich.FromLogContext()
                          .WriteTo.Console();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    // Configuração do intervalo de execução
                    var intervalMinutes = configuration.GetValue<int>("ScrapingIntervalMinutes", 5);
                    var interval = TimeSpan.FromMinutes(intervalMinutes);

                    // String de conexão com SQLite
                    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=/data/quotes.db";

                    // Registra o repositório SQLite
                    services.AddSingleton<IQuoteRepository>(sp => new SqliteQuoteRepository(connectionString));

                    // Configura HttpClient com timeout
                    services.AddHttpClient<IScrapingService, ScrapingService>((sp, client) =>
                    {
                        var timeoutSeconds = configuration.GetValue<int>("HttpTimeoutSeconds", 30);
                        client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                        client.BaseAddress = new Uri(configuration["TargetUrl"] ?? "http://quotes.toscrape.com/");
                    });

                    // Registra serviços de persistência
                    services.AddScoped<IPersistenceService, PersistenceService>();

                    // Registra o Worker como serviço hospedado
                    services.AddHostedService(sp => new Worker(
                        sp.GetRequiredService<ILogger<Worker>>(),
                        sp.GetRequiredService<IScrapingService>(),
                        sp.GetRequiredService<IPersistenceService>(),
                        interval
                    ));
                });
        }
    }
}


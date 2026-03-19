using RPA.Services;

namespace RPA
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IScrapingService _scrapingService;
        private readonly IPersistenceService _persistenceService;
        private readonly TimeSpan _interval;

        public Worker(ILogger<Worker> logger, IScrapingService scrapingService, IPersistenceService persistenceService, TimeSpan interval)
        {
            _logger = logger;
            _scrapingService = scrapingService;
            _persistenceService = persistenceService;
            _interval = interval;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Iniciando execução do RPA em: {time}", DateTimeOffset.Now);

                try
                {
                    var quotes = await _scrapingService.ScrapeQuotesAsync();
                    _logger.LogInformation($"Encontradas {quotes.Count()} citações.");

                    if (quotes.Any())
                    {
                        await _persistenceService.SaveQuotesAsync(quotes);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro durante a execução do RPA.");
                }

                _logger.LogInformation($"Aguardando {_interval.TotalMinutes} minutos até a próxima execução.");
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}

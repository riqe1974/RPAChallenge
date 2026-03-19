using HtmlAgilityPack;
using Polly;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.Services
{
    public class ScrapingService : IScrapingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScrapingService> _logger;

        public ScrapingService(HttpClient httpClient, ILogger<ScrapingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Quote>> ScrapeQuotesAsync()
        {
            // Política de retry: tenta 3 vezes com espera exponencial
          var retryPolicy = Policy
             .Handle<HttpRequestException>()
             .Or<TaskCanceledException>()
             .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                 onRetry: (exception, timeSpan, retryCount, context) =>
                 {
                     _logger.LogWarning($"Tentativa {retryCount} falhou. Esperando {timeSpan} segundos. Erro: {exception.Message}");
                 });

            return await retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync("");
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                var quotes = ExtractQuotesFromHtml(html);
                return quotes;
            });

        }

        private IEnumerable<Quote> ExtractQuotesFromHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var quoteNodes = doc.DocumentNode.SelectNodes("//div[@class='quote']");
            if (quoteNodes == null)
                return new List<Quote>();

            var quotes = new List<Quote>();
            foreach (var node in quoteNodes)
            {
                var textNode = node.SelectSingleNode(".//span[@class='text']");
                var authorNode = node.SelectSingleNode(".//small[@class='author']");
                var tagsNodes = node.SelectNodes(".//a[@class='tag']");

                if (textNode == null || authorNode == null)
                    continue;

                var text = textNode.InnerText.Trim();
                var author = authorNode.InnerText.Trim();
                var tags = tagsNodes != null ? string.Join(",", tagsNodes.Select(t => t.InnerText)) : null;

                quotes.Add(new Quote
                {
                    Text = text,
                    Author = author,
                    Tags = tags,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return quotes;
        }
    }
}

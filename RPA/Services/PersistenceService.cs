using Shared.Interfaces;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.Services
{
    public class PersistenceService : IPersistenceService
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly ILogger<PersistenceService> _logger;

        public PersistenceService(IQuoteRepository quoteRepository, ILogger<PersistenceService> logger)
        {
            _quoteRepository = quoteRepository;
            _logger = logger;
        }

        public async Task SaveQuotesAsync(IEnumerable<Quote> quotes)
        {
            // Aqui poderíamos implementar lógica para evitar duplicatas, por exemplo, verificando se já existe citação com mesmo texto e autor.
            foreach (var quote in quotes)
            {
                // Exemplo simples: salva todos, sem verificação de duplicidade.
                await _quoteRepository.AddAsync(quote);
                _logger.LogInformation($"Salva citação: {quote.Text} - {quote.Author}");
            }
        }
    }
}

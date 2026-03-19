using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.Services
{
    public interface IPersistenceService
    {
        Task SaveQuotesAsync(IEnumerable<Quote> quotes);
    }
}

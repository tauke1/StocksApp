using StocksApp.StocksApiClients.Models;
using StocksApp.StocksApiClients.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.StocksApiClients
{
    public interface IStocksApiClient
    {
        Task<IList<StockInfo>> GetStocksHistoryAsync(string ticker, DateRange dateRange = null, SpecificDateInterval dateInterval = null);
        IList<KeyValuePair<DateInterval, int>> GetValidIntervals();
    }
}

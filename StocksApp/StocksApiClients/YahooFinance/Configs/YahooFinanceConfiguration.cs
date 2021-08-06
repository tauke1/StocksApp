using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.StocksApiClients.YahooFinance.Configs
{
    public class YahooFinanceConfiguration
    {
        public string BaseUrl { get; set; }
        public string ScrapeUrl { get; set; }
        public int TimeoutInSeconds { get; set; }
    }
}

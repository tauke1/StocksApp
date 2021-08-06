using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.StocksApiClients.Tiingo.Configs
{
    public class TiingoConfiguration
    {
        public string BaseUrl { get; set; }
        public string Token { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}

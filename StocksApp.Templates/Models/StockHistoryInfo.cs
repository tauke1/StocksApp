using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Templates.Models
{
    public class StockHistoryInfo
    {
        public string Ticker { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IList<StockHistoryItem> StockHistoryItems { get; set; }
    }
}

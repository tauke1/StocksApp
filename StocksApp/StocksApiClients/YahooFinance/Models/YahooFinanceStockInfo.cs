using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.StocksApiClients.YahooFinance.Models
{
    public class YahooFinanceStockInfo
    {
        [DataMember(Name = "Date")]
        public DateTime Date { get; set; }
        
        [DataMember(Name = "Open")]
        public decimal Open { get; set; }
        
        [DataMember(Name = "High")]
        public decimal High { get; set; }
        
        [DataMember(Name = "Low")]
        public decimal Low { get; set; }
        
        [DataMember(Name = "Close")]
        public decimal Close { get; set; }
        
        [DataMember(Name = "Volume")]
        public decimal Volume { get; set; }
    }
}

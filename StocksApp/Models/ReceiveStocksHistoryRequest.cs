using StocksApp.StocksApiClients.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Models
{
    public class ReceiveStocksHistoryRequest
    {
        public string Ticker {get;set;}
        public DateIntervalWithShift IntervalWithShift { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

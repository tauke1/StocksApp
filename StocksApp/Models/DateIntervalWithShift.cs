using StocksApp.StocksApiClients.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Models
{
    public class DateIntervalWithShift
    {
        public DateInterval DateInterval { get; set; }
        public int Shift { get; set; }

        public override string ToString()
        {
            return $"{Shift} {DateInterval}";
        }
    }
}

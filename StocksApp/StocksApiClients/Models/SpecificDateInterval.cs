using StocksApp.StocksApiClients.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.StocksApiClients.Models
{
    public class SpecificDateInterval
    {
        public DateInterval Interval { get; }
        public int Shift { get; }

        public SpecificDateInterval(DateInterval interval, int shift)
        {
            if (shift <= 0)
                throw new ArgumentOutOfRangeException(nameof(shift), "argument must not be less or equal than zero");

            Shift = shift;
            Interval = interval;
        }
    }
}

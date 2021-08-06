using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.StocksApiClients.Models
{
    public class DateRange
    {
        public DateTime? StartDate { get; }
        public DateTime? EndDate { get;}
        public DateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate > endDate)
                throw new ArgumentOutOfRangeException(nameof(startDate), $"argument must not be later than {nameof(endDate)}");

            StartDate = startDate;
            EndDate = endDate;
        }


    }
}

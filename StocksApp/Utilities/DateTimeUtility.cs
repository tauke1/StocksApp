using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Utilities
{
    public class DateTimeUtility : IDateTimeUtility
    {
        public long GetEpochTime(DateTime dateTime)
        {
            DateTimeOffset dateTimeOffset = (DateTimeOffset)dateTime;
            // returns epoch time relative to user's time zone,
            return dateTimeOffset.ToUnixTimeSeconds();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Utilities
{
    public interface IDateTimeUtility
    {
        long GetEpochTime(DateTime dateTime);
    }
}

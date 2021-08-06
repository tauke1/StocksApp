using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.StocksApiClients.Tiingo.Models
{
    public class TiingoStockInfo
    {
        /// <summary>
        /// The date this data pertains to
        /// </summary>
        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// The closing price for the asset on the given date
        /// </summary>
        [DataMember(Name = "close")]
        public decimal Close { get; set; }

        /// <summary>
        /// The high price for the asset on the given date
        /// </summary>
        [DataMember(Name = "high")]
        public decimal High { get; set; }

        /// <summary>
        /// The low price for the asset on the given date
        /// </summary>
        [DataMember(Name = "low")]
        public decimal Low { get; set; }

        /// <summary>
        /// The opening price for the asset on the given date
        /// </summary>
        [DataMember(Name = "open")]
        public decimal Open { get; set; }

        [DataMember(Name = "volume")]
        public decimal Volume { get; set; }

        /// <summary>
        /// The adjusted closing price for the asset on the given date
        /// </summary>
        [DataMember(Name = "adjClose")]
        public decimal AdjClose { get; set; }

        /// <summary>
        /// The adjusted high price for the asset on the given date
        /// </summary>
        [DataMember(Name = "adjHigh")]
        public decimal AdjHigh { get; set; }

        /// <summary>
        /// The adjusted high price for the asset on the given date
        /// </summary>
        [DataMember(Name = "adjLow")]
        public decimal AdjLow { get; set; }

        /// <summary>
        /// The adjusted opening price for the asset on the given date
        /// </summary>
        [DataMember(Name = "adjOpen")]
        public decimal AdjOpen { get; set; }

        /// <summary>
        /// The number of shares traded for the asset
        /// </summary>
        [DataMember(Name = "adjVolume")]
        public decimal AdjVolume { get; set; }

        /// <summary>
        /// The dividend paid out on "date" (note that "date" will be the "exDate" for the dividend)
        /// </summary>
        [DataMember(Name = "divCash")]
        public decimal DivCash { get; set; }

        /// <summary>
        /// The factor used to adjust prices when a company splits, reverse splits, or pays a distribution
        /// </summary>
        [DataMember(Name = "splitFactor")]
        public decimal SplitFactor { get; set; }
    }
}

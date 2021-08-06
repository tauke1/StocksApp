using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.StocksApiClients.Exceptions
{
    public class BadApiRequestException : Exception
    {
        public int StatusCode { get; }
        public string ResponseContect { get; }
        public override string Message { get { return $"Stocks History API returned not successful\nHttpCode: {StatusCode}\nContent: {ResponseContect}"; } }
        public BadApiRequestException(int statusCode, string responseContent)
        {
            if (responseContent == null)
                throw new ArgumentNullException(nameof(responseContent));

            StatusCode = statusCode;
            ResponseContect = responseContent;
        }
    }
}

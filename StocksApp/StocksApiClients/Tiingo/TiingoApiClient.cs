using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StocksApp.Serializers.Json;
using StocksApp.StocksApiClients.Exceptions;
using StocksApp.StocksApiClients.Models;
using StocksApp.StocksApiClients.Models.Enums;
using StocksApp.StocksApiClients.Tiingo.Configs;
using StocksApp.StocksApiClients.Tiingo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace StocksApp.StocksApiClients.Tiingo
{
    public class TiingoApiClient : ITiingoApiClient
    {
        private readonly string _baseUrl;
        private readonly string _token;
        private readonly int _timeoutInSeconds;
        private readonly IJsonSerializer _jsonSerializer;
        private const string API_NAME = "Tiingo";
        private readonly List<KeyValuePair<DateInterval, int>> _validIntervals = new List<KeyValuePair<DateInterval, int>>
        {
            new KeyValuePair<DateInterval, int>(DateInterval.Day, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Week, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Month, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Year, 1),
        };

        public TiingoApiClient(IOptions<TiingoConfiguration> configurationOptions, IJsonSerializer jsonSerializer)
        {
            if (configurationOptions == null)
                throw new ArgumentNullException(nameof(configurationOptions));
            if (configurationOptions.Value == null)
                throw new ArgumentException(
                    $"Property {nameof(configurationOptions.Value)} must not be null", nameof(configurationOptions));
            if (configurationOptions.Value.BaseUrl == null)
                throw new ArgumentException(
                    $"Property {nameof(configurationOptions.Value)}.{nameof(configurationOptions.Value.BaseUrl)} must not be null", nameof(configurationOptions));
            if (configurationOptions.Value.Token == null)
                throw new ArgumentException(
                    $"Property {nameof(configurationOptions.Value)}.{nameof(configurationOptions.Value.Token)} must not be null", nameof(configurationOptions));
            if(configurationOptions.Value.TimeoutInSeconds < 0)
                throw new ArgumentException(
                    $"Property {nameof(configurationOptions.Value)}.{nameof(configurationOptions.Value.TimeoutInSeconds)} must be a positive integer", nameof(configurationOptions));

            if (jsonSerializer == null)
                throw new ArgumentNullException(nameof(jsonSerializer));

            TiingoConfiguration configuration = configurationOptions.Value;
            _baseUrl = configuration.BaseUrl;
            _token = configuration.Token;
            _jsonSerializer = jsonSerializer;
            _timeoutInSeconds = configuration.TimeoutInSeconds;
        }

        public override string ToString()
        {
            return API_NAME;
        }
        public async Task<IList<StockInfo>> GetStocksHistoryAsync(string ticker, DateRange dateRange = null, SpecificDateInterval dateInterval = null)
        {
            if (ticker == null)
                throw new ArgumentNullException(nameof(ticker));
            if (ticker.Trim() == string.Empty)
                throw new ArgumentException("Argument cannot be empty or contain only space characters", nameof(ticker));

            string route = $"tiingo/daily/{ticker}/prices";
            var queryParametersDict = new Dictionary<string, string>();

            if (dateRange != null)
            {
                if (dateRange.StartDate.HasValue)
                {
                    DateTime startDate = dateRange.StartDate.Value;
                    queryParametersDict["startDate"] = startDate.ToString("yyyy-MM-dd");
                }

                if (dateRange.EndDate.HasValue)
                {
                    DateTime endDate = dateRange.EndDate.Value;
                    queryParametersDict["endDate"] = endDate.ToString("yyyy-MM-dd");
                }
            }

            if (dateInterval != null)
            {
                string dateIntervalString = DateIntervalToString(dateInterval);
                queryParametersDict["interval"] = dateIntervalString;
            }

            queryParametersDict["format"] = "json";
            List<TiingoStockInfo> tiingoStockInfos = await MakeRequestAndGetRawResponseAsync<List<TiingoStockInfo>>(route, HttpMethod.Get, queryParametersDict);
            return tiingoStockInfos.Select(s => new StockInfo
            {
                Close = s.Close,
                Date = s.Date,
                High = s.High,
                Low = s.Low,
                Open = s.Open
            }).ToList();
        }

        public IList<KeyValuePair<DateInterval, int>> GetValidIntervals()
        {
            return _validIntervals.ToList();
        }

        private async Task<T> MakeRequestAndGetRawResponseAsync<T>(string route, HttpMethod method, IDictionary<string, string> queryParameters)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            if (route.Trim() == string.Empty)
                throw new ArgumentException("Argument cannot be empty or contain only space characters", nameof(route));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (queryParameters == null)
                queryParameters = new Dictionary<string, string>();

            queryParameters.Add(new KeyValuePair<string, string>("token", _token));
            string queryString = string.Empty;
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach ((string key, string value) in queryParameters)
                query[key] = value;

            queryString = query.ToString();
            string url = $"{_baseUrl}/{route}?{queryString}";
            var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Method = method;
            request.RequestUri = new Uri(url);

            using HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_timeoutInSeconds) };
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new BadApiRequestException((int)response.StatusCode, responseBody);

            return _jsonSerializer.Deserialize<T>(responseBody);
        }

        private string DateIntervalToString(SpecificDateInterval specificDateInterval)
        {
            if (specificDateInterval == null)
                throw new ArgumentNullException(nameof(specificDateInterval));

            DateInterval dateInterval = specificDateInterval.Interval;
            string intervalShort = null;
            switch (dateInterval)
            {
                case DateInterval.Day:
                    intervalShort = "daily";
                    break;
                case DateInterval.Month:
                    intervalShort = "monthly";
                    break;
                case DateInterval.Week:
                    intervalShort = "weekly";
                    break;
                case DateInterval.Year:
                    intervalShort = "annualy";
                    break;
                default:
                    throw new InvalidOperationException($"{dateInterval} interval not supported by Tiingo End-Of-Day API");
            }

            HashSet<KeyValuePair<DateInterval, int>> validIntervalsSet = _validIntervals.ToHashSet();
            if (!validIntervalsSet.Contains(new KeyValuePair<DateInterval, int>(dateInterval, specificDateInterval.Shift)))
                throw new InvalidOperationException($"Date interval {specificDateInterval.Shift} {dateInterval} not supported by Tiingo End-Of-Day API");

            string dateIntervalString = intervalShort;
            return dateIntervalString;
        }
    }
}

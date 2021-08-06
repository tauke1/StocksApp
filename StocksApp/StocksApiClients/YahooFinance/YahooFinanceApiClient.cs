using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StocksApp.Serializers.Сsv;
using StocksApp.StocksApiClients.Exceptions;
using StocksApp.StocksApiClients.Models;
using StocksApp.StocksApiClients.Models.Enums;
using StocksApp.StocksApiClients.YahooFinance.Configs;
using StocksApp.StocksApiClients.YahooFinance.Models;
using StocksApp.StocksApiClients.YahooFinance.Stores;
using StocksApp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace StocksApp.StocksApiClients.YahooFinance
{
    public class YahooFinanceApiClient : IYahooFinanceApiClient
    {
        private readonly CookieAndCrumbStore _cookieAndCrumbStore;
        private readonly ICsvSerializer _csvSerializer;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly int _timeoutSeconds;
        private readonly string _baseUrl;
        private const string HISTORY_EVENT = "history";
        private const string API_NAME = "Yahoo Finance";
        private readonly List<KeyValuePair<DateInterval, int>> _validIntervals = new List<KeyValuePair<DateInterval, int>>
        {
            new KeyValuePair<DateInterval, int>(DateInterval.Day, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Day, 5),
            new KeyValuePair<DateInterval, int>(DateInterval.Week, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Month, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Month, 3)
        };

        public YahooFinanceApiClient(IOptions<YahooFinanceConfiguration> configurationOptions, ICsvSerializer csvSerializer,
            IDateTimeUtility dateTimeUtility)
        {
            if (dateTimeUtility == null)
                throw new ArgumentNullException(nameof(dateTimeUtility));
            if (configurationOptions == null)
                throw new ArgumentNullException(nameof(configurationOptions));
            if (configurationOptions.Value == null)
                throw new ArgumentException(
                    $"{nameof(configurationOptions.Value)} must not be null", nameof(configurationOptions));
            if (configurationOptions.Value.BaseUrl == null)
                throw new ArgumentException(
                    $"Property {nameof(configurationOptions.Value)}.{nameof(configurationOptions.Value.BaseUrl)} must not be null", nameof(configurationOptions));
            if (configurationOptions.Value.ScrapeUrl == null)
                throw new ArgumentException(
                    $"Property {nameof(configurationOptions.Value)}.{nameof(configurationOptions.Value.ScrapeUrl)} must not be null", nameof(configurationOptions));
            if (configurationOptions.Value.TimeoutSeconds < 0)
                throw new ArgumentException(
                    $"Property {nameof(configurationOptions.Value)}.{nameof(configurationOptions.Value.TimeoutSeconds)} must be a positive integer", nameof(configurationOptions));

            YahooFinanceConfiguration configuration = configurationOptions.Value;
            _csvSerializer = csvSerializer;
            _baseUrl = configuration.BaseUrl;
            _timeoutSeconds = configuration.TimeoutSeconds;
            _dateTimeUtility = dateTimeUtility;
            _cookieAndCrumbStore = new CookieAndCrumbStore(configuration.ScrapeUrl, _timeoutSeconds);
        }

        public override string ToString()
        {
            return API_NAME;
        }

        public IList<KeyValuePair<DateInterval, int>> GetValidIntervals()
        {
            return _validIntervals.ToList();
        }

        public Task<IList<StockInfo>> GetStocksHistoryAsync(string ticker, DateRange dateRange = null, SpecificDateInterval dateInterval = null)
        {
            if (ticker == null)
                throw new ArgumentNullException(nameof(ticker));
            if (ticker.Trim() == string.Empty)
                throw new ArgumentException("Argument cannot be empty or contain only space characters", nameof(ticker));

            // one retry used for refresh crumb in case of it's expired
            return GetStocksHistoryAsyncWithRetry(ticker, dateRange, dateInterval, false);
        }

        private async Task<IList<StockInfo>> GetStocksHistoryAsyncWithRetry(string ticker, DateRange dateRange = null, SpecificDateInterval dateInterval = null, bool retryUsed = false)
        {
            if (ticker == null)
                throw new ArgumentNullException(nameof(ticker));
            if (ticker.Trim() == string.Empty)
                throw new ArgumentException("Argument cannot be empty or contain only space characters", nameof(ticker));

            // if retried, it's mean that we meen refresh cookie and crumb
            bool needRefreshCookieAndCrumb = retryUsed;
            (string _, string crumb) = await _cookieAndCrumbStore.GetCookieAndCrumbAsync(ticker, needRefreshCookieAndCrumb);
            var queryParametersDict = new Dictionary<string, string>();
            queryParametersDict["crumb"] = crumb;
            if (dateRange != null)
            {
                if (dateRange.StartDate.HasValue)
                {
                    DateTime startDate = dateRange.StartDate.Value;
                    long startDateEpoch = _dateTimeUtility.GetEpochTime(startDate);
                    queryParametersDict["period1"] = startDateEpoch.ToString();
                }

                if (dateRange.EndDate.HasValue)
                {
                    DateTime endDate = dateRange.EndDate.Value;
                    long endDateEpoch = _dateTimeUtility.GetEpochTime(endDate);
                    queryParametersDict["period2"] = endDateEpoch.ToString();
                }
            }

            if (dateInterval != null)
            {
                string dateIntervalString = DateIntervalToString(dateInterval);
                queryParametersDict["interval"] = dateIntervalString;
            }

            queryParametersDict["events"] = HISTORY_EVENT;
            try
            {
                IEnumerable<YahooFinanceStockInfo>  stockInfos = await MakeRequestAsync<YahooFinanceStockInfo>($"finance/download/{ticker}", HttpMethod.Get, queryParametersDict);
                return stockInfos.Select(s => new StockInfo
                {
                    Close = s.Close,
                    Date = s.Date,
                    High = s.High,
                    Low = s.Low,
                    Open = s.Open
                }).ToList();
            }
            catch (BadApiRequestException ex)
            {
                if (ex.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    if (retryUsed)
                        throw;

                    return await GetStocksHistoryAsyncWithRetry(ticker, dateRange, dateInterval, true);
                }

                throw;
            }
        }

        private async Task<IEnumerable<T>> MakeRequestAsync<T>(string route, HttpMethod method, IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            if (route.Trim() == string.Empty)
                throw new ArgumentException("Argument cannot be empty or contain only space characters", nameof(route));
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            string resposne = await MakeRequestAndGetRawResponseAsync(route, method, queryParameters);
            IEnumerable<T> collection = _csvSerializer.Deserialize<T>(resposne);
            return collection;
        }

        private async Task<string> MakeRequestAndGetRawResponseAsync(string route, HttpMethod method, IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            if (route == null)
                throw new ArgumentNullException(nameof(route));
            if (route.Trim() == string.Empty)
                throw new ArgumentException("Argument cannot be empty or contain only space characters", nameof(route));
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            string queryString = string.Empty;
            if (queryParameters != null)
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                foreach ((string key, string value) in queryParameters)
                    query[key] = value;

                queryString = query.ToString();
            }

            string url = $"{_baseUrl}/{route}?{queryString}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Method = method;
            using HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_timeoutSeconds) };
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new BadApiRequestException((int)response.StatusCode, responseBody);

            return responseBody;
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
                    intervalShort = "d";
                    break;
                case DateInterval.Month:
                    intervalShort = "mo";
                    break;
                case DateInterval.Week:
                    intervalShort = "wk";
                    break;
                default:
                    throw new InvalidOperationException($"{dateInterval} interval not supported by Yahoo Finance API");
            }

            HashSet<KeyValuePair<DateInterval, int>> validIntervalsSet = _validIntervals.ToHashSet();
            if (!validIntervalsSet.Contains(new KeyValuePair<DateInterval, int>(dateInterval, specificDateInterval.Shift)))
                throw new InvalidOperationException($"Date interval {specificDateInterval.Shift} {dateInterval} not supported by Yahoo Finance API");

            string dateIntervalString = $"{specificDateInterval.Shift}{intervalShort}";
            return dateIntervalString;
        }
    }
}

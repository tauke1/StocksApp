using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StocksApp.Serializers.Сsv;
using StocksApp.StocksApiClients.Exceptions;
using StocksApp.StocksApiClients.Models;
using StocksApp.StocksApiClients.Models.Enums;
using StocksApp.StocksApiClients.YahooFinance.Configs;
using StocksApp.StocksApiClients.YahooFinance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace StocksApp.StocksApiClients.YahooFinance
{
    public class YahooFinanceApiClient : IYahooFinanceApiClient
    {
        private readonly ICsvSerializer _csvSerializer;
        private readonly string _scrapeUrl;
        private readonly string _baseUrl;
        private readonly int _timeoutInSeconds;
        private const string HISTORY_EVENT = "history";
        private const string COOKIE_HEADER_NAME = "Set-Cookie";
        private const string API_NAME = "Yahoo Finance";
        private readonly Regex _regexCrumb = new Regex("CrumbStore\":{\"crumb\":\"(?<crumb>.+?)\"}",
                            RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private readonly List<KeyValuePair<DateInterval, int>> _validIntervals = new List<KeyValuePair<DateInterval, int>>
        {
            new KeyValuePair<DateInterval, int>(DateInterval.Minute, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Minute, 2),
            new KeyValuePair<DateInterval, int>(DateInterval.Minute, 5),
            new KeyValuePair<DateInterval, int>(DateInterval.Minute, 30),
            new KeyValuePair<DateInterval, int>(DateInterval.Minute, 60),
            new KeyValuePair<DateInterval, int>(DateInterval.Minute, 90),
            new KeyValuePair<DateInterval, int>(DateInterval.Hour, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Day, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Day, 5),
            new KeyValuePair<DateInterval, int>(DateInterval.Week, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Month, 1),
            new KeyValuePair<DateInterval, int>(DateInterval.Month, 3)
        };

        public YahooFinanceApiClient(IOptions<YahooFinanceConfiguration> configurationOptions, ICsvSerializer csvSerializer)
        {
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
            if (configurationOptions.Value.TimeoutInSeconds < 0)
                throw new ArgumentException(
                    $"Property {nameof(configurationOptions.Value)}.{nameof(configurationOptions.Value.TimeoutInSeconds)} must be a positive integer", nameof(configurationOptions));

            YahooFinanceConfiguration configuration = configurationOptions.Value;
            _csvSerializer = csvSerializer;
            _baseUrl = configuration.BaseUrl;
            _scrapeUrl = configuration.ScrapeUrl;
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
                throw new ArgumentException("Argument cannot be empty or contain only space characters",nameof(ticker));

            (string _, string crumb) = await GetCookieAndCrumbAsync(ticker).ConfigureAwait(false);
            var queryParametersDict = new Dictionary<string, string>();
            queryParametersDict["crumb"] = crumb;
            if (dateRange != null)
            {
                if (dateRange.StartDate.HasValue)
                {
                    DateTime startDate = dateRange.StartDate.Value;
                    queryParametersDict["period1"] = ((DateTimeOffset)startDate).ToUnixTimeSeconds().ToString();
                }

                if (dateRange.EndDate.HasValue)
                {
                    DateTime endDate = dateRange.EndDate.Value;
                    queryParametersDict["period2"] = ((DateTimeOffset)endDate).ToUnixTimeSeconds().ToString();
                }
            }

            if (dateInterval != null)
            {
                string dateIntervalString = DateIntervalToString(dateInterval);
                queryParametersDict["interval"] = dateIntervalString;
            }

            queryParametersDict["events"] = HISTORY_EVENT;
            IEnumerable<YahooFinanceStockInfo> stockInfos = await MakeRequestAsync<YahooFinanceStockInfo>($"finance/download/{ticker}", HttpMethod.Get, queryParametersDict);
            return stockInfos.Select(s => new StockInfo
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
            using HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_timeoutInSeconds) };
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new BadApiRequestException((int)response.StatusCode, responseBody);

            return responseBody;
        }

        private async Task<(string, string)> GetCookieAndCrumbAsync(string ticker)
        {
            if (ticker == null)
                throw new ArgumentNullException(nameof(ticker));
            if (ticker.Trim() == string.Empty)
                throw new ArgumentException("Argument cannot be empty or contain only space characters", nameof(ticker));

            string url = string.Format(_scrapeUrl, ticker);
            using HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_timeoutInSeconds) };
            using (HttpResponseMessage response = await httpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                HttpHeaders httpHeaders = response.Headers;
                IEnumerable<string> cookieHeaderValues;
                if (!response.Headers.TryGetValues(COOKIE_HEADER_NAME, out cookieHeaderValues))
                    throw new Exception($"{COOKIE_HEADER_NAME} header not found");

                string cookie = cookieHeaderValues.Single();
                string content = await response.Content.ReadAsStringAsync();
                string crumb = GetCrumbs(content);
                return (cookie, crumb);
            }

        }

        private string GetCrumbs(string html)
        {
            if (html == null)
                throw new ArgumentNullException(nameof(html));
            if (html.Trim() == string.Empty)
                throw new ArgumentException("Argument cannot be empty or contain only space characters", nameof(html));

            string crumb = null;
            MatchCollection matches = _regexCrumb.Matches(html);
            if (matches.Count == 0)
                throw new Exception("Cant find crumbs in given html");

            if (matches.Count > 0)
            {
                crumb = matches[0].Groups["crumb"].Value;
                //fixed unicode character 'SOLIDUS'
                if (crumb.Length != 11)
                    crumb = crumb.Replace("\\u002F", "/");
            }

            return crumb;
        }

        private string DateIntervalToString(SpecificDateInterval specificDateInterval)
        {
            if (specificDateInterval == null)
                throw new ArgumentNullException(nameof(specificDateInterval));

            DateInterval dateInterval = specificDateInterval.Interval;
            string intervalShort = null;
            switch (dateInterval)
            {
                case DateInterval.Minute:
                    intervalShort = "m";
                    break;
                case DateInterval.Hour:
                    intervalShort = "h";
                    break;
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

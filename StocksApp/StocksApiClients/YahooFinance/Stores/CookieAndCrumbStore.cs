using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StocksApp.StocksApiClients.YahooFinance.Stores
{
    public class CookieAndCrumbStore
    {
        private const string COOKIE_HEADER_NAME = "Set-Cookie";
        private readonly Regex _regexCrumb = new Regex("CrumbStore\":{\"crumb\":\"(?<crumb>.+?)\"}",
                            RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private readonly string _baseUrl;
        private readonly int _timeoutSeconds;
        private string _cookie;
        private string _crumb;
        private object locker = new object();
        public CookieAndCrumbStore(string baseUrl, int timeoutSeconds)
        {
            if (baseUrl == null)
                throw new ArgumentNullException(nameof(baseUrl));
            if (baseUrl.Trim() == string.Empty)
                throw new ArgumentException("Argument must not be an empty string or contains only whitespace chars", nameof(baseUrl));
            if (timeoutSeconds <= 0)
                throw new ArgumentException("Argument must be a positive integer", nameof(timeoutSeconds));

            _timeoutSeconds = timeoutSeconds;
            _baseUrl = baseUrl;
        }

        public async Task<(string, string)> GetCookieAndCrumbAsync(string ticker)
        {
            if (ticker == null)
                throw new ArgumentNullException(nameof(ticker));
            if (ticker.Trim() == string.Empty)
                throw new ArgumentException("Argument cannot be empty or contain only space characters", nameof(ticker));

            // this will make thread-safe update of _cookie and _crumb fields  
            if (Monitor.TryEnter(locker, new TimeSpan(0, 0, _timeoutSeconds)))
            {
                try
                {
                    if (_cookie == null || _crumb == null)
                    {
                        string url = string.Format(_baseUrl, ticker);
                        using HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_timeoutSeconds) };
                        using (HttpResponseMessage response = await httpClient.GetAsync(url))
                        {
                            response.EnsureSuccessStatusCode();
                            HttpHeaders httpHeaders = response.Headers;
                            IEnumerable<string> cookieHeaderValues;
                            if (!response.Headers.TryGetValues(COOKIE_HEADER_NAME, out cookieHeaderValues))
                                throw new InvalidOperationException($"{COOKIE_HEADER_NAME} header not found");

                            string cookie = cookieHeaderValues.Single();
                            string content = await response.Content.ReadAsStringAsync();
                            string crumb = GetCrumbs(content);
                            _cookie = cookie;
                            _crumb = crumb;
                        }
                    }

                    return (_cookie, _crumb);
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            }

            throw new TimeoutException("Cant update crumb and cookie due to locker timeout issue");
        }

        public void ClearCookieAndCrumb()
        {
            _cookie = null;
            _crumb = null;
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
    }
}

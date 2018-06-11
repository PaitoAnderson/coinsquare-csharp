using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace CoinsquareAPI
{
    public interface ICoinsquare
    {
        Task Login(string username, string password, string token = null);
        Task<GetUserInformationResponse> GetUserInformation();
        Task Logout();

        Task<QuoteResponse[]> GetQuotes();

        /// <param name="point_interval">10mins, 1hour, or 1day</param>
        /// <returns>
        /// Oldest to Newest
        /// [0] = Unix Timestamp GMT
        /// [1] = Bid?
        /// [2] = Ask?
        /// [3] = 
        /// [4] = Chart Display?
        /// [5] = Volume?
        /// </returns>
        Task<long[][]> GetChart(string symbol, string symbol_base = "BTC", string point_interval = "10mins");
        Task<GetBalancesResponse> GetBalances();
        Task<GetTransactionsResponse> GetTransactions(string symbol, string category = "quick_trade", int page = 0, int page_size = 10);
        Task<GetQuickTradeResponse> GetQuickTrade(string symbol_from, string symbol_to, decimal amount_from);
        Task<QuickTradeResponse> QuickTrade(string symbol_from, string symbol_to, decimal amount_from, decimal amount_to);
    }

    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string token { get; set; }
    }

    public class LoginResponse
    {
        public string akey { get; set; }
        public string base_currency { get; set; }
        public long date_time { get; set; }
        public string email { get; set; }
        public string flags { get; set; }
        public string id { get; set; }
        public string key_audit { get; set; }
        public string key_referral { get; set; }
        public int otp_status { get; set; }
        public long sid { get; set; }
        public int unread { get; set; }
        public string username { get; set; }
        public bool verified_cell { get; set; }
        public bool verified_profile { get; set; }
        public string xkey { get; set; }
    }

    public class GetUserInformationResponse
    {
        public string akey { get; set; }
        public string base_currency { get; set; }
        public long date_time { get; set; }
        public string email { get; set; }
        public string flags { get; set; }
        public string id { get; set; }
        public string key_audit { get; set; }
        public string key_referral { get; set; }
        public long otp_status { get; set; }
        public long sid { get; set; }
        public int unread { get; set; }
        public string username { get; set; }
        public bool verified_cell { get; set; }
        public bool verified_profile { get; set; }
        public string xkey { get; set; }
    }

    public class QuotesResponse
    {
        public QuoteResponse[] quotes { get; set; }
    }

    public class QuoteResponse
    {
        public string ticker { get; set; }
        public string @base { get; set; }
        public string status { get; set; }
        public decimal last { get; set; }
        public decimal volume { get; set; }
        public decimal? volbase { get; set; }
        public decimal? bid { get; set; }
        public decimal? ask { get; set; }
        public decimal low24 { get; set; }
        public decimal high24 { get; set; }

        // Issue with Exponents (-1.71476e-05, 2.09722e-06)
        // public decimal ret24 { get; set; }
    }

    public class GetBalancesResponse
    {
        public GetBalancesAccountsResponse accounts { get; set; } = new GetBalancesAccountsResponse();
    }

    public class GetBalancesAccountsResponse
    {
        public GetBalancesCurrencyResponse USD { get; set; }
        public GetBalancesCurrencyResponse EUR { get; set; }
        public GetBalancesCurrencyResponse GBP { get; set; }
        public GetBalancesCurrencyResponse CAD { get; set; }
        public GetBalancesCurrencyResponse AUD { get; set; }
        public GetBalancesCurrencyResponse JPY { get; set; }
        public GetBalancesCurrencyResponse CHF { get; set; }
        public GetBalancesCurrencyResponse XAU { get; set; }
        public GetBalancesCurrencyResponse XAG { get; set; }
        public GetBalancesCurrencyResponse BTC { get; set; }
        public GetBalancesCurrencyResponse LTC { get; set; }
        public GetBalancesCurrencyResponse BCH { get; set; }
        public GetBalancesCurrencyResponse DOGE { get; set; }
        public GetBalancesCurrencyResponse ETH { get; set; }
        public GetBalancesCurrencyResponse DASH { get; set; }
        public GetBalancesCurrencyResponse RCOIN { get; set; }
        public GetBalancesCurrencyResponse CSC { get; set; }
        public GetBalancesCurrencyResponse CNY { get; set; }
        [JsonProperty("XAG.ML")]
        public GetBalancesCurrencyResponse XAGML { get; set; }
        [JsonProperty("XAG.X")]
        public GetBalancesCurrencyResponse XAGX { get; set; }
        [JsonProperty("XAU.I")]
        public GetBalancesCurrencyResponse XAUI { get; set; }
    }

    public class GetBalancesCurrencyResponse
    {
        public string tic { get; set; }
        public decimal bal { get; set; }
        public decimal avl { get; set; }
        public decimal val { get; set; }
        public string dep { get; set; }
        //"wit": "",
        //"nopen": "",
        public int fav { get; set; }
        public int sca { get; set; }
        public int flg { get; set; }
    }

    public class GetTransactionsResponse
    {
        public string category { get; set;}
        public int? offset { get; set; }
        public int page { get; set; }
        public string symbol { get; set; }
        public int transaction_count { get; set; }
        public GetTransctionsRowResponse[] transactions { get; set; }
    }

    public class GetTransctionsRowResponse
    {
        public long date { get; set; }
        public decimal from_amount { get; set; }
        public string from_currency { get; set; }
        public decimal from_lid { get; set; }
        public decimal to_amount { get; set; }
        public string to_currency { get; set; }
        public decimal to_lid { get; set; }
    }

    public class GetQuickTradeRequest
    {
        public string symbol_from { get; set; }
        public string symbol_to { get; set; }
        public decimal amount_from { get; set; }
    }

    public class GetQuickTradeResponse
    {
        public decimal base_cost { get; set; }
        public decimal btc_cost { get; set; }
        public decimal btc_proceeds { get; set; }
        public decimal coin_fee { get; set; }
        public decimal coin_proceeds { get; set; }
        public decimal coin_request { get; set; }
        public string message { get; set; }
        public decimal price_all_in { get; set; }
        public decimal price_market { get; set; }
        public string symbol_from { get; set; }
        public string symbol_to { get; set; }
    }

    public class QuickTradeRequest
    {
        public string symbol_from { get; set; }
        public string symbol_to { get; set; }
        public decimal amount_from { get; set; }
        public decimal amount_to { get; set; }
    }

    public class QuickTradeResponse
    {
        public decimal base_cost { get; set; }
        public decimal btc_cost { get; set; }
        public decimal btc_proc { get; set; }
        public decimal coin_fee { get; set; }
        public decimal coin_proceeds { get; set; }
        public string message { get; set; }
        public string symbol_from { get; set; }
        public string symbol_to { get; set; }
    }

    public class Coinsquare : ICoinsquare
    {
        private static string BaseUrl = @"https://coinsquare.com/api/v1";

        private static HttpClient Client = new HttpClient();
        private static HttpClient AuthClient;

        private string akey { get; set; }
        private long sid { get; set; }
        private string xkey { get; set; }

        public async Task Login(string username, string password, string token = null)
        {
            await SetCookie();

            var url = @"/auth/user/login";
            var timestamp = GetCurrentTimestamp();
            var loginRequest = new LoginRequest
            {
                username = username,
                password = password,
                token = token ?? string.Empty
            };
            var parameters = JsonConvert.SerializeObject(loginRequest);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}{url}?cacheBuster={timestamp}"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "x-cs-nonce", timestamp.ToString() },
                    { "x-cs-parameters", Convert.ToBase64String(Encoding.UTF8.GetBytes(parameters)) },
                    { "x-cs-signature", string.Empty }
                },
            };

            var results = await AuthClient.SendAsync(request);
            results.EnsureSuccessStatusCode();

            var loginResponse = await results.Deserialize<LoginResponse>();
            akey = loginResponse.akey;
        }

        public async Task<GetUserInformationResponse> GetUserInformation()
        {
            var url = @"/auth/user/information";
            var timestamp = GetCurrentTimestamp();
            var signature = GetSignature("POST", url, timestamp, string.Empty);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}{url}?cacheBuster={timestamp}"),
                Method = HttpMethod.Get,
                Headers =
                {
                    { "x-cs-nonce", timestamp.ToString() },
                    { "x-cs-parameters", string.Empty },
                    { "x-cs-signature", signature }
                },
            };

            var results = await AuthClient.SendAsync(request);
            results.EnsureSuccessStatusCode();

            var response = await results.Deserialize<GetUserInformationResponse>();
            sid = response.sid;
            xkey = response.xkey;

            return response;
        }

        public async Task Logout()
        {
            var url = @"/auth/user/logout";
            var timestamp = GetCurrentTimestamp();
            var signature = GetSignature("POST", url, timestamp, string.Empty);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}{url}?cacheBuster={timestamp}"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "x-cs-nonce", timestamp.ToString() },
                    { "x-cs-parameters", string.Empty },
                    { "x-cs-signature", signature }
                },
            };

            var results = await AuthClient.SendAsync(request);
            results.EnsureSuccessStatusCode();
        }

        public async Task<QuoteResponse[]> GetQuotes()
        {
            var results = await Client.GetAsync($"{BaseUrl}/data/quotes");
            results.EnsureSuccessStatusCode();
            return (await results.Deserialize<QuotesResponse>()).quotes;
        }

        public async Task<long[][]> GetChart(string symbol, string symbol_base, string point_interval)
        {
            var results = await Client.GetAsync($"{BaseUrl}/data/chart/{symbol}/{symbol_base}/{point_interval}");
            results.EnsureSuccessStatusCode();
            return await results.Deserialize<long[][]>();
        }

        public async Task<GetBalancesResponse> GetBalances()
        {
            var url = @"/auth/account/balances";
            var timestamp = GetCurrentTimestamp();
            var signature = GetSignature("GET", url, timestamp, string.Empty);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}{url}?cacheBuster={timestamp}"),
                Method = HttpMethod.Get,
                Headers =
                {
                    { "x-cs-nonce", timestamp.ToString() },
                    { "x-cs-parameters", string.Empty },
                    { "x-cs-signature", signature }
                },
            };

            var results = await AuthClient.SendAsync(request);
            results.EnsureSuccessStatusCode();
            return await results.Deserialize<GetBalancesResponse>();
        }

        public async Task<GetTransactionsResponse> GetTransactions(string symbol, string category, int page, int page_size)
        {
            var url = $"/kingslanding/auth/transactions/ledger?year=-1&category={category}&symbol={symbol}&page={page}&page_size={page_size}";
            var timestamp = GetCurrentTimestamp();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}{url}?cacheBuster={timestamp}"),
                Method = HttpMethod.Get,
                Headers =
                {
                    { "x-cs-sid", sid.ToString() },
                    { "x-cs-xkey", xkey }
                },
            };

            var results = await AuthClient.SendAsync(request);
            results.EnsureSuccessStatusCode();
            return await results.Deserialize<GetTransactionsResponse>();
        }

        public async Task<GetQuickTradeResponse> GetQuickTrade(string symbol_from, string symbol_to, decimal amount_from)
        {
            var url = @"/auth/orders/quicktrade";
            var timestamp = GetCurrentTimestamp();
            var getQuickTradeRequest = new GetQuickTradeRequest
            {
                symbol_from = symbol_from,
                symbol_to = symbol_to,
                amount_from = amount_from
            };

            var parameters = JsonConvert.SerializeObject(getQuickTradeRequest);
            var signature = GetSignature("GET", url, timestamp, parameters);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}{url}?cacheBuster={timestamp}"),
                Method = HttpMethod.Get,
                Headers =
                {
                    { "x-cs-nonce", timestamp.ToString() },
                    { "x-cs-parameters", Convert.ToBase64String(Encoding.UTF8.GetBytes(parameters)) },
                    { "x-cs-signature", signature }
                },
            };

            var results = await AuthClient.SendAsync(request);
            results.EnsureSuccessStatusCode();
            return await results.Deserialize<GetQuickTradeResponse>();
        }

        public async Task<QuickTradeResponse> QuickTrade(string symbol_from, string symbol_to, decimal amount_from, decimal amount_to)
        {
            var url = @"/auth/orders/quicktrade";
            var timestamp = GetCurrentTimestamp();
            var quickTradeRequest = new QuickTradeRequest
            {
                symbol_from = symbol_from,
                symbol_to = symbol_to,
                amount_from = amount_from,
                amount_to = amount_to
            };
            var parameters = JsonConvert.SerializeObject(quickTradeRequest);
            var signature = GetSignature("POST", url, timestamp, parameters);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}{url}?cacheBuster={timestamp}"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "x-cs-nonce", timestamp.ToString() },
                    { "x-cs-parameters", Convert.ToBase64String(Encoding.UTF8.GetBytes(parameters)) },
                    { "x-cs-signature", signature }
                },
            };

            var results = await AuthClient.SendAsync(request);
            results.EnsureSuccessStatusCode();
            return await results.Deserialize<QuickTradeResponse>();
        }

        private async Task SetCookie()
        {
            // Setup Auth Client
            var authClientHandler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            AuthClient = new HttpClient(authClientHandler);

            var url = "/auth/cookie";
            var timestamp = GetCurrentTimestamp();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}{url}?cacheBuster={timestamp}"),
                Method = HttpMethod.Get,
                Headers =
                {
                    { "x-cs-nonce", timestamp.ToString() },
                    { "x-cs-parameters", string.Empty },
                    { "x-cs-signature", string.Empty }
                },
            };

            var results = await AuthClient.SendAsync(request);
            results.EnsureSuccessStatusCode();

            // Fix cookie domain
            var cookies = GetAllCookies(authClientHandler).Where(x => x.Domain == "coinsquare.com");
            foreach (var cookie in cookies)
            {
                authClientHandler.CookieContainer.Add(new Uri("https://coinsquare.com/"), new Cookie("SKEY", cookie.Value));
            }
        }

        private string GetSignature(string method, string url, long timestamp, string parameters)
        {
            url = url.Replace("/auth", string.Empty);
            var hmacsha386 = new HMACSHA384(Encoding.UTF8.GetBytes(akey));
            var signatureString = $"{method}:{url}:{timestamp}:{parameters}";
            var signature = hmacsha386.ComputeHash(Encoding.UTF8.GetBytes(signatureString));
            var signatureHex = BitConverter.ToString(signature).Replace("-", "").ToLower();
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(signatureHex));
        }

        private List<Cookie> GetAllCookies(HttpClientHandler authClientHandler)
        {
            var cookies = new List<Cookie>();
            var table = (Hashtable)authClientHandler.CookieContainer.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, authClientHandler.CookieContainer, new object[] { });
            foreach (object pathList in table.Values)
            {
                var lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) cookies.Add(c);
            }
            return cookies;
        }

        public static int GetCurrentTimestamp()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }

    public static class HttpClientExtensionMethods
    {
        public static async Task<T> Deserialize<T>(this HttpResponseMessage response)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(resultString);
        }
    }
}
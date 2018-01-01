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
        Task Logout();

        Task<QuoteResponse[]> GetQuotes();
        Task<GetBalancesResponse> GetBalances();
        Task<GetLedgerResponse> GetLedger(string symbol, int page = 0, int rows_per_page = 10);
        Task<GetQuickTradeResponse> GetQuickTrade(string symbol_from, string symbol_to, decimal amount_from);
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
        public int unread{ get; set; } 
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
        public decimal ret24 { get; set; }
    }

    public class GetBalancesResponse
    {
        public GetBalancesAccountsResponse accounts { get; set; }
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

    public class GetLedgerRequest
    {
        public string symbol { get; set; }
        public int page { get; set; }
        public int rows_per_page { get; set; }
    }

    public class GetLedgerResponse
    {
        public int offset { get; set; }
        public int page { get; set; }
        public decimal previous_balance { get; set; }
        public int row_count { get; set; }
        public GetLedgerRowResponse[] rows { get; set; }
    }

    public class GetLedgerRowResponse
    {
        public decimal amount { get; set; }
        public decimal balance { get; set; }
        public string category { get; set; }
        public long date { get; set; }
        public string description { get; set; }
        public long lid { get; set; }
        public int num_type { get; set; }
        public string operation { get; set; }
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

    public class Coinsquare : ICoinsquare
    {
        private static string BaseUrl = @"https://coinsquare.io/api/v1";

        private static HttpClient Client = new HttpClient();

        private static HttpClientHandler AuthClientHandler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };
        private static HttpClient AuthClient = new HttpClient(AuthClientHandler);

        private string Key { get; set; }

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
            Key = loginResponse.akey;
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
            return (await results.Deserialize<GetBalancesResponse>());
        }

        public async Task<GetLedgerResponse> GetLedger(string symbol, int page = 0, int rows_per_page = 10)
        {
            var url = @"/auth/account/ledger";
            var timestamp = GetCurrentTimestamp();
            var getLedgerRequest = new GetLedgerRequest
            {
                symbol = symbol,
                page = page,
                rows_per_page = rows_per_page
            };
            var parameters = JsonConvert.SerializeObject(getLedgerRequest);
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
            return (await results.Deserialize<GetLedgerResponse>());
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
            return (await results.Deserialize<GetQuickTradeResponse>());
        }

        private async Task SetCookie()
        {
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
            var cookie = GetAllCookies().SingleOrDefault(x => x.Domain == "coinsquare.io");
            if (cookie != null)
            {
                AuthClientHandler.CookieContainer.Add(new Uri("https://coinsquare.io/"), new Cookie("SKEY", cookie.Value));
            }
        }

        private string GetSignature(string method, string url, long timestamp, string parameters)
        {
            url = url.Replace("/auth", string.Empty);
            var hmacsha386 = new HMACSHA384(Encoding.UTF8.GetBytes(Key));
            var signatureString = $"{method}:{url}:{timestamp}:{parameters}";
            var signature = hmacsha386.ComputeHash(Encoding.UTF8.GetBytes(signatureString));
            var signatureHex = BitConverter.ToString(signature).Replace("-", "").ToLower();
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(signatureHex));
        }

        private List<Cookie> GetAllCookies()
        {
            var cookies = new List<Cookie>();
            var table = (Hashtable)AuthClientHandler.CookieContainer.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, AuthClientHandler.CookieContainer, new object[] { });
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

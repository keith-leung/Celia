using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.SDK
{
    public abstract class ApiHelperBase
    {
        protected readonly string _appId;
        protected readonly string _appSecret;
        protected readonly string _authApiHost;

        protected readonly HttpClient _httpClient;

        public ApiHelperBase(string appId, string appSecret, string authApiHost)
        {
            this._appId = appId ?? throw new ArgumentNullException(nameof(appId));
            this._appSecret = appSecret ?? throw new ArgumentNullException(nameof(appSecret));
            this._authApiHost = authApiHost ?? throw new ArgumentNullException(nameof(authApiHost));

            _httpClient = new HttpClient();
        }

        protected async Task<JObject> HttpGetAsync(string path)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            return await HttpGetCore(path);
        }

        protected async Task<JObject> HttpGetCore(string path)
        {
            Uri uri = new Uri(new Uri(this._authApiHost), path);
            var response = await _httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        return JObject.Parse(result);
                    }
                    catch (Newtonsoft.Json.JsonReaderException je)
                    {
                        Console.WriteLine(je.Message);
                        JObject jresult = new JObject();
                        jresult.Add("result", result);

                        return jresult;
                    }
                }

                return null;
            }
            else
            {
                throw new Exception(response.ToString());
            }
        }

        protected async Task<JObject> HttpPostAsync(string path, JToken kvRequest)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            return await HttpPostCore(path, kvRequest);
        }

        protected async Task<JObject> HttpPostCore(string path, JToken kvRequest)
        {
            Uri uri = new Uri(new Uri(this._authApiHost), path);
            HttpContent httpContent = new StringContent(kvRequest.ToString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        return JObject.Parse(result);
                    }
                    catch (Newtonsoft.Json.JsonReaderException je)
                    {
                        Console.WriteLine(je.Message);
                        JObject jresult = new JObject();
                        jresult.Add("result", result);

                        return jresult;
                    }
                }

                return null;
            }
            else
            {
                throw new Exception(response.ToString());
            }
        }

        protected Task<JObject> TokenHttpGetAsync(string accessToken, string path)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            return HttpGetCore(path);
        }

        protected Task<JObject> TokenHttpPostAsync(string accessToken, string path, JObject kvRequest)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            return HttpPostCore(path, kvRequest);
        }
    }
}

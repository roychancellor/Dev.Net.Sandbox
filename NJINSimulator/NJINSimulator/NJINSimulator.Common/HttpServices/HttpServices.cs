using Newtonsoft.Json.Linq;
using NJINSimulator.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NJINSimulator.Common.HttpServices
{
    public sealed class HttpServices
    {
        private static readonly HttpClient _client = new HttpClient();

        static HttpServices() { }
        private HttpServices() { }

        public static HttpClient Client
        {
            get
            {
                return _client;
            }
        }

        public static async Task<bool> Send(string toUrl, string toSend)
        {
            if (toUrl.IsNullOrEmpty() || toSend.IsNullOrEmpty())
            {
                return false;
            }
            try
            {
                HttpContent content = new StringContent(toSend, Encoding.UTF8, "application/xml");
                var response = await _client.PostAsync(toUrl, content);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

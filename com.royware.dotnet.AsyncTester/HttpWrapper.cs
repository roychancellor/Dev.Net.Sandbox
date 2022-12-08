using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace com.royware.dotnet.AsyncTester
{
    public class HttpWrapper
    {
        private static HttpClient _client;

        public HttpWrapper()
        {
            _client = new HttpClient(new HttpClientHandler());
        }
        
        public async Task<string> GetPayloadFromAPI(int i, string URL = "https://docs.microsoft.com/dotnet")
        {
            Console.WriteLine($"GetPayloadFromAPI for message {i}: right before await GetStringAsync");
            var response = await _client.GetStringAsync(URL);
            Console.WriteLine($"GetPayloadFromAPI for message {i}: right after await GetStringAsync.");
            var toReturn = response.Substring(0, 15);
            Console.WriteLine($"GetPayloadFromAPI for message {i}: Returning response: {toReturn}.");
            return toReturn;
        }
    }
}

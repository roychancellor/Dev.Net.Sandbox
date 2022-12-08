using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.royware.dotnet.AsyncTester
{
    public class APICaller
    {
        private HttpWrapper _httpWrapper;

        public APICaller()
        {
            _httpWrapper = new HttpWrapper();
        }
        
        public string GetResponseFromAPI(int i)
        {
            Console.WriteLine($"GetResponseFromAPI for message {i}: right before GetPayloadFromAPI");
            var response = _httpWrapper.GetPayloadFromAPI(i).Result;
            Console.WriteLine($"GetResponseFromAPI for message {i}: right after GetPayloadFromAPI. Returning response.");
            return response;
        }

        public async Task<string> GetResponseFromAPIAsync(int i)
        {
            Console.WriteLine($"GetResponseFromAPIAsync for message {i}: right before await GetPayloadFromAPIAsync");
            var response = await _httpWrapper.GetPayloadFromAPI(i);
            Console.WriteLine($"GetResponseFromAPIAsync for message {i}: right after await GetPayloadFromAPIAsync. Returning response.");
            return response;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.royware.dotnet.AsyncTester
{
    public class PartialResponseBuilder
    {
        private APICaller _aPICaller;

        public PartialResponseBuilder()
        {
            _aPICaller = new APICaller();
        }

        public PartialResponseBuilder(APICaller aPICaller)
        {
            _aPICaller = aPICaller ?? new APICaller();
        }

        public string BuildPartialResponse(int i)
        {
            Console.WriteLine($"BuildPartialResponse for message {i}: right before GetResponseFromAPI");
            var response = _aPICaller.GetResponseFromAPI(i);
            Console.WriteLine($"BuildPartialResponse for message {i}: right after GetResponseFromAPI. Returning response.");
            return response;
        }

        public async Task<string> BuildPartialResponseAsync(int i)
        {
            Console.WriteLine($"BuildPartialResponseAsync for message {i}: right before await GetResponseFromAPIAsync");
            var response = await _aPICaller.GetResponseFromAPIAsync(i);
            Console.WriteLine($"BuildPartialResponseAsync for message {i}: right after await GetResponseFromAPIAsync. Returning response.");
            return response;
        }
    }
}

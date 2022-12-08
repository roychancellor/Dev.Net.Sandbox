using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.royware.dotnet.AsyncTester
{
    public class ResponseBuilder
    {
        private string _response;
        private PartialResponseBuilder _prb;

        public ResponseBuilder()
        {
            _prb = new PartialResponseBuilder(new APICaller());
        }
        
        public string BuildResponse(int i)
        {
            Console.WriteLine($"BuildResponse for message {i}: right before BuildPartialResponse");
            _response = _prb.BuildPartialResponse(i);
            Console.WriteLine($"BuildResponse for message {i}: right after BuildPartialResponse. Returning response.");
            return _response; 
        }

        public async Task<string> BuildResponseAsync(int i)
        {
            Console.WriteLine($"BuildResponseAsync for message {i}: right before await for BuildPartialResponseAsync");
            _response = await _prb.BuildPartialResponseAsync(i);
            Console.WriteLine($"BuildResponseAsync for message {i}: right after await for BuildPartialResponseAsync. Returning response.");
            return _response;
        }
    }
}

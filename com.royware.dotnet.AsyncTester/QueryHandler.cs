using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.royware.dotnet.AsyncTester
{
    public class QueryHandler
    {
        private string _response;
        private ResponseBuilder _responseBuilder;

        public QueryHandler()
        {
            _responseBuilder = new ResponseBuilder();
        }

        public QueryHandler(ResponseBuilder responseBuilder)
        {
            _responseBuilder = responseBuilder ?? new ResponseBuilder();
        }
        
        public string HandleQueryMessage(string message, int i)
        {
            Console.WriteLine($"HandleQueryMessage for message {i}: right before BuildResponse");
            _response = _responseBuilder.BuildResponse(i);
            Console.WriteLine($"HandleQueryMessage for message {i}: right after BuildResponse. Returning message and response.");

            return $"Original message:\n{message}\nResponse:\n{_response}";
        }

        public async Task<string> HandleQueryMessageAsync(string message, int i)
        {
            Console.WriteLine($"HandleQueryMessageAsync for message {i}: right before await for BuildResponseAsync");
            _response = await _responseBuilder.BuildResponseAsync(i);
            Console.WriteLine($"HandleQueryMessageAsync for message {i}: right after await for BuildResponseAsync. Returning message and response.");

            return $"Original message:\n{message}\nResponse:\n{_response}";
        }
    }
}

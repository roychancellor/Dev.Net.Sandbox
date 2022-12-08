using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.royware.dotnet.AsyncTester
{
    public class MessageSender
    {
        public bool Send(string message, int i)
        {
            Console.WriteLine($"Sending message {i}\n{message}");
            return true;
        }
    }
}

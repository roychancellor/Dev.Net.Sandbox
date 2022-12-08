using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.royware.dotnet.AsyncTester
{
    public class ServiceRunner
    {
        private MessageHandler _messageHandler;
        private int _totalRuns;

        public ServiceRunner()
        {
            _messageHandler = new MessageHandler();
            _totalRuns = 5;
        }

        public ServiceRunner(int totalTasks)
        {
            _totalRuns = totalTasks > 0 ? totalTasks : 5;
            _messageHandler = new MessageHandler(_totalRuns);
        }

        public bool Run()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < _totalRuns; i++)
            {
                string message = $"Incoming Message {i}!";
                Console.WriteLine($"======> Calling HandleMessage {i}");
                var result = _messageHandler.HandleMessage(message, i);
                Console.WriteLine($"<====== Back from HandleMessage {i} which returned {result}"); 
            }
            sw.Stop();
            Console.WriteLine($"Elapsed Time of Run (sync): {sw.ElapsedMilliseconds} ms. Returning true.");
            return true;
        }

        public bool RunHybrid()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < _totalRuns; i++)
            {
                string message = $"Incoming Message {i}!";
                Console.WriteLine($"======> Calling HandleMessage {i}");
                var result = _messageHandler.HandleMessageHybrid(message, i);
                Console.WriteLine($"<====== Back from HandleMessage {i} which returned {result}");
            }
            sw.Stop();
            Console.WriteLine($"Elapsed Time of Run (sync): {sw.ElapsedMilliseconds} ms. Returning true.");
            return true;
        }

        public async Task<bool> RunAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < _totalRuns; i++)
            {
                string message = $"Incoming Message {i}!";
                Console.WriteLine($"======> Before await HandleMessageAsync {i}");
                var result = await _messageHandler.HandleMessageAsync(message, i);
                Console.WriteLine($"<====== After await HandleMessageAsync {i} which returned {result}\n");
            }
            sw.Stop();
            Console.WriteLine($"Elapsed Time of RunAsync: {sw.ElapsedMilliseconds} ms. Returning true.");
            return true;
        }
    }
}

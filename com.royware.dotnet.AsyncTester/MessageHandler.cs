using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.royware.dotnet.AsyncTester
{
    public class MessageHandler
    {
        private QueryHandler _queryHandler;
        private MessageSender _messageSender;
        private Stopwatch _sw;
        private int _countOfTasks;
        private int _totalTasks;

        public MessageHandler() : this(0)
        {
        }

        public MessageHandler(int totalTasks)
        {
            _queryHandler = new QueryHandler(new ResponseBuilder());
            _messageSender = new MessageSender();
            _countOfTasks = 0;
            _totalTasks = totalTasks > 0 ? totalTasks : 5;
            _sw = new Stopwatch();
            _sw.Start();
        }

        public bool HandleMessage(string message, int i)
        {
            Console.WriteLine($"HandleMessage with message {i}:\n{message}");
            try
            {
                if (Thread.CurrentThread.Name == null)
                {
                    Thread.CurrentThread.Name = $"HandleMessage Spawning thread {i}";
                }
                Console.WriteLine($"HandleMessage about to spawn a new Task for ProcessMessage {i}. The spawning thread is {Thread.CurrentThread.Name}");
                Task.Factory.StartNew(() => ProcessMessage(message, i));
                Console.WriteLine($"HandleMessage spawned a new Task for ProcessMessage {i}. Returning true.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HandleMessage:\n{ex}");
                return false;
            }
        }

        public bool ProcessMessage(string message, int i)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = $"ProcessMessage Thread {i}";
            }
            Console.WriteLine($"ProcessMessage for {i}. About to call HandleQueryMessage SYNCHRONOUSLY. The spawned Thread Id is {Thread.CurrentThread.Name}");
            var response = _queryHandler.HandleQueryMessage(message, i);
            Console.WriteLine($"ProcessMessage for {i}. Returned from SYNCHRONOUS call to HandleQueryMessage.");
            Console.WriteLine($"Calling Send...");
            var sendResponse = _messageSender.Send(response, i);
            Console.WriteLine($"Send returned {sendResponse}. Returning.");
            _countOfTasks++;
            if (_countOfTasks == _totalTasks)
            {
                _sw.Stop();
                Console.WriteLine($"The elapsed time of SYNC processing is {_sw.ElapsedMilliseconds} ms");
            }
            return sendResponse;
        }

        public bool HandleMessageHybrid(string message, int i)
        {
            Console.WriteLine($"HandleMessageHybrid with message {i}:\n{message}");
            try
            {
                if (Thread.CurrentThread.Name == null)
                {
                    Thread.CurrentThread.Name = $"HandleMessageHybrid spawning thread {i}";
                }
                Console.WriteLine($"HandleMessageHybrid about to spawn a new awaited Task for ProcessMessageAsync {i}. The spawning thread is {Thread.CurrentThread.Name}");
                Task.Factory.StartNew(async () => await ProcessMessageAsync(message, i));
                Console.WriteLine($"HandleMessageHybrid spawned a new awaited Task for ProcessMessageAsync {i}. Returning true.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HandleMessage:\n{ex}");
                return false;
            }
        }

        public async Task<bool> HandleMessageAsync(string message, int i)
        {
            Console.WriteLine($"HandleMessageAsync called with message {i}:\n{message}");
            try
            {
                Console.WriteLine($"HandleMessageAsync {i} right before await ProcessMessageAsync.");
                var result = await ProcessMessageAsync(message, i);
                Console.WriteLine($"HandleMessageAsync {i} right after await ProcessMessageAsync. Returning result.");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HandleMessage:\n{ex}");
                return false;
            }
        }

        public async Task<bool> ProcessMessageAsync(string message, int i)
        {
            Thread.CurrentThread.Name = Thread.CurrentThread.Name ?? $"ProcessMessage Thread {i}";
            Console.WriteLine($"ProcessMessage for {i}. About to call HandleQueryMessage ASYNCHRONOUSLY. Thread Id is {Thread.CurrentThread.Name}");
            var response = await _queryHandler.HandleQueryMessageAsync(message, i);
            Console.WriteLine($"ProcessMessage for {i}. Returned from ASYNCHRONOUS call to HandleQueryMessage.");
            Console.WriteLine($"Calling Send...");
            var sendResponse = _messageSender.Send(response, i);
            Console.WriteLine($"Send returned {sendResponse}. Returning.");
            _countOfTasks++;
            if (_countOfTasks == _totalTasks)
            {
                _sw.Stop();
                Console.WriteLine($"The elapsed time of ASYNC processing is {_sw.ElapsedMilliseconds} ms. Press any key to end...");
            }
            return sendResponse;
        }
    }
}

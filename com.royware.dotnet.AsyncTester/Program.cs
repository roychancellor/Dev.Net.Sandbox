using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.royware.dotnet.AsyncTester
{
    public class Program
    {
        private static ServiceRunner _runner;
        
        public static void Main(string[] args)
        {
            int runs = 5;
            if (args.Length > 0 && !int.TryParse(args[0], out runs))
            {
                Console.WriteLine($"Invalid command line argument: {args[0]}. Must be a positive integer number of runs. Structure of args is 'nnn <--a, --s, or --h>'. Ending.");
                return;
            }

            RunMode runMode = RunMode.Sync;
            if (args.Length > 1)
            {
                switch (args[1])
                {
                    case "--a":
                        runMode = RunMode.Async;
                        break;
                    case "--h":
                        runMode = RunMode.Hybrid;
                        break;
                }
            }

            _runner = new ServiceRunner(runs);
            
            bool result;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.Write($"======> Starting Main by calling _runner.");
            switch (runMode)
            {
                case RunMode.Sync:
                    Console.WriteLine($"Run().");
                    result = _runner.Run();
                    break;
                case RunMode.Async:
                    Console.WriteLine($"RunAsync().");
                    result = _runner.RunAsync().Result;
                    break;
                case RunMode.Hybrid:
                    Console.WriteLine($"RunHybrid().");
                    result = _runner.RunHybrid();
                    break;
                default:
                    result = false;
                    break;
            }
            Console.WriteLine($"<====== Finished Main with result {result}");
            sw.Stop();
            Console.WriteLine($"The elapsed time of Main was {sw.ElapsedMilliseconds} ms. Press any key to end....");
            Console.Read();
        }

        public enum RunMode
        {
            Sync,
            Async,
            Hybrid,
        }
    }
}

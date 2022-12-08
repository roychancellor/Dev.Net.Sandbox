using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncForEach
{
    public class Program
    {
        static void Main(string[] args)
        {
            AsyncTester.Run();
        }
    }

    public class AsyncTester
    {
        public static void Run()
        {
            Console.WriteLine("======> Calling GetUrlContentLengthsAsync...");
            var results = GetUrlContentLengthsAsync(10).Result;
            Console.WriteLine("<====== Back from call to GetUrlContentLengthsAsync. Writing results...");
            int i = 1;
            foreach (var result in results)
            {
                Console.WriteLine($"Result {i}: {result}");
                i++;
            }
        }

        public static async Task<IEnumerable<string>> GetUrlContentLengthsAsync(int numTimes)
        {
            var tasks = new List<Task<string>>();

            for (int i = 1; i <= numTimes; i++)
            {
                tasks.Add(GetUrlContentLengthAsync(i));
            }
            Console.WriteLine("In GetUrlContentLengthsAsync, all Tasks created. About to call await Task.WhenAll");
            
            return await Task.WhenAll(tasks);
        }

        public static async Task<string> GetUrlContentLengthAsync(int i)
        {
            var client = new HttpClient();

            Task<string> getStringTask = client.GetStringAsync("https://docs.microsoft.com/dotnet");

            IndependentWorkBeforeAwait(i);

            Console.WriteLine($"Calling 'await getStringTask' for Task {i}");
            string contents = await getStringTask;
            Console.WriteLine($"After await statement for Task {i}");

            IndependentWorkAfterAwait(i);

            return $"Thread {i}: {contents.Length}";
        }

        public static void IndependentWorkBeforeAwait(int i)
        {
            Console.WriteLine($"Doing Independent Work BEFORE the await on Task {i}...");
        }

        public static void IndependentWorkAfterAwait(int i)
        {
            Console.WriteLine($"Doing Independent Work AFTER the await on Task {i}...");
        }
    }
}

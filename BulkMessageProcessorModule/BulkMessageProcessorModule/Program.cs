using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var loader = new BulkFileLoader();
            var result = loader.Load();
            Console.WriteLine("The source file load result: " + result);
        }
    }
}

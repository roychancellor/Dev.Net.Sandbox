using IOSimulator.NSB.OUT.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOSimulator.NSB.OUT
{
    internal class NSBOUTSimulator
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*******************************************");
            Console.WriteLine("************ NSB OUT SIMULATOR ************");
            Console.WriteLine("*******************************************");
            Console.WriteLine("====> STARTING TESTS");
            NSBOUTLogic.Run();
            Console.WriteLine("<==== TESTING FINISHED");
        }
    }
}

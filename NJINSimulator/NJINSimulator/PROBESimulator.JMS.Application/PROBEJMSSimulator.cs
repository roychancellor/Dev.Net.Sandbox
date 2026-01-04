using NJINSimulator.Common.ActiveMQ;
using PROBESimulator.Probes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROBESimulator.Application
{
    internal class PROBEJMSSimulator
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("******************************************************");
                Console.WriteLine("*************** PROBE JMS SIMULATOR ******************");
                Console.WriteLine("******************************************************");
                Console.WriteLine(">>>>> Starting PROBE JMS Simulator Application");
                Console.Write("Configuring ActiveMQ Services (if not already configured)...");
                var amq = AMQServices.Instance;
                Console.WriteLine("DONE.\n");

                Console.Write("Configuring and Initializing PROBE Services...");
                var probeServices = PROBEServicesSingleton.ProbeServices;
                Console.WriteLine("DONE.\n");

                Console.WriteLine("\n<<<<< PROBE JMS Services are now active and listening for messages.");

                Console.WriteLine("Enter 0 to Exit.");
                var keepGoing = true;
                while (keepGoing)
                {
                    var keyPress = Console.ReadKey().KeyChar;
                    keepGoing = keyPress == '0' ? false : true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown:\n{ex}");
                Console.WriteLine("\nPress any key to exit");
                Console.Read();
            }
        }
    }
}

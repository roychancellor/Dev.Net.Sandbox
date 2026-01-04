using NJINSimulator.Common.ActiveMQ;
using NJINSimulator.Common.Config;
using NJINSimulator.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJINSimulator.Application
{
    internal class NJINSimulator
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("******************************************************");
                Console.WriteLine("*************** NJIN SERVICES SIMULATOR **************");
                Console.WriteLine("******************************************************");
                Console.WriteLine(">>>>> Starting NJIN Simulator Application");
                Console.Write("Configuring ActiveMQ Services (if not already configured)...");
                var amq = AMQServices.Instance;
                Console.WriteLine("DONE.\n");

                Console.WriteLine($"Getting App Config for NJIN services...");
                var njinConfig = NJINSimulatorAppConfigProvider.Instance.NjinConfig;
                //Console.WriteLine($"Done with config settings:\n{NJINSimulatorAppConfigProvider.Instance.PrintConfig()}");
                
                Console.WriteLine("Configuring and Initializing NJIN Services...");
                var njinServices = NJINServicesSingleton.Instance.NjinServices;
                Console.WriteLine($"njinServices count = {njinServices.Count}");
                //Console.WriteLine($"njinServices:\n{NJINServicesSingleton.Instance.PrintServices()}");
                Console.WriteLine("DONE.\n");

                Console.WriteLine("\n<<<<< NJIN Services are now active and listening for messages.");

                Console.WriteLine("Enter 0 to Exit.");
                var keepGoing = true;
                while (keepGoing)
                {
                    var keyPress = Console.ReadKey().KeyChar;
                    
                    
                    keepGoing = keyPress != '0';
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

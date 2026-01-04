using IOSimulator.NSB.OUT.Models;
using NJINSimulator.Common.Config;
using NJINSimulator.Common.FileIO;
using NJINSimulator.Common.Models;
using NJINSimulator.Common.Serialization;
using NJINSimulator.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IOSimulator.NSB.OUT.Logic
{
    public class NSBOUTLogic
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const string NSBOUT = "NSB.OUT";
        
        public NSBOUTLogic() { }

        public static void Run()
        {
            try
            {
                _logger.Info($"===> {NSBOUT}: Starting Tests");
                // Get the App config
                _logger.Trace("Getting app config");
                var config = NSBOUTSimulatorAppConfigProvider.Instance.NsbOutConfig;
                if (config == null || config.IOInterfaceURL.IsNullOrEmpty() || config.TestConfigPath.IsNullOrEmpty())
                {
                    var msg = $"{NSBOUT} Run: Unable to get the IO Interface URL and/or the Test config Path";
                    throw new Exception(msg);
                }

                // Get the test config
                _logger.Trace($"Getting test config from file '{config.TestConfigPath}'");
                var testConfigXML = FileIOWrapper.ReadTextFile(config.TestConfigPath);
                if (testConfigXML.IsNullOrEmpty())
                {
                    var msg = $"{NSBOUT} Run: Unable to get the test configuration from file at '{config.TestConfigPath}'";
                    throw new Exception(msg);
                }
                _logger.Trace($"Deserializing test config");
                var testConfig = Serialization.Deserialize<TestConfiguration>(testConfigXML);
                if (testConfig == null)
                {
                    var msg = $"{NSBOUT} Run: Unable to deserialize the test configuration from {testConfigXML}";
                    throw new Exception(msg);
                }

                // Build TW to POST
                _logger.Trace($"Building TWs to POST to IO Interface");
                var toPost = BuildTransactionalWrappers(testConfig.NumberOfTests, testConfig.InboundCorrelationIdBase, testConfig.VerboseTW);

                // POST TWs to IO Interface
                _logger.Trace($"POSTing TWs to IO Interface");
                var sendResult = SendToIOInterface(toPost, config.IOInterfaceURL, testConfig.DelayBetweenTestsMS).Result;
                var testResult = sendResult == true ? "SUCCESS" : "FAILURE";

                _logger.Info($"<=== {NSBOUT}: Finished Tests with result '{testResult}'");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error encountered: {ex.Message}");
                throw ex;
            }
        }

        private static List<string> BuildTransactionalWrappers(int numToBuild, string BaseIBCID = "TEST", bool verbose = false)
        {
            var toReturn = new List<string>();
            var TW = new TransactionalWrapper();
            try
            {
                for (int i = 0; i < numToBuild; i++)
                {
                    TW.Verbose = verbose;
                    TW.InboundCorrelationID = $"{BaseIBCID}{i + 1}";
                    var ser = Serialization.Serialize(TW);
                    toReturn.Add(ser);
                }
                return toReturn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task<bool> SendToIOInterface(List<string> toSend, string URL, int delay)
        {
            if (toSend == null)
            {
                _logger.Warn("No TWs to POST");
                return false;
            }
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    string msg = "===> POST {0} <=== {1}";
                    int i = 0;
                    foreach (var tw in toSend)
                    {
                        var toPost = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress)
                        {
                            Content = new StringContent(tw, Encoding.UTF8, "application/xml")
                        };
                        var result = await client.SendAsync(toPost);
                        result.EnsureSuccessStatusCode();
                        _logger.Info(string.Format(msg, i + 1, result.StatusCode == HttpStatusCode.OK ? "SUCCESS" : "FAILURE"));
                        i++;
                        Thread.Sleep(delay);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"While sending POSTs to IO Interface: {ex.Message}");
                throw ex;
            }
        }
    }
}

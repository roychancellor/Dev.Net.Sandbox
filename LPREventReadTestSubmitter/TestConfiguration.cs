using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPREventReadTestSubmitter
{
    public class TestConfiguration
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        
        public int NumberOfTests { get; set; }
        public int DelayBetweenTests { get; set; }
        public ConfigSource ConfigSource { get; set; }
        public PayloadFormat PayloadFormat { get; set; }

        public static List<TestEventRead> ReadEventReadsFromFile(string testFilePath)
        {
            if (string.IsNullOrEmpty(testFilePath))
            {
                Console.WriteLine("Test file is null or empty. Returning null.");
                return null;
            }

            List<TestEventRead> toReturn = new List<TestEventRead>();
            List<string> tests;
            try
            {
                tests = File.ReadLines(testFilePath).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown reading test file configuration. Ending.\n{ex}");
                throw;
            }
            int testId = 0;
            foreach (var test in tests)
            {
                var testValues = test.Split(",");
                if (testValues.Length == 3)
                {
                    var submitterId = testValues[0];
                    var scannedLPN = testValues[1];
                    var eventId = testValues[2];
                    testId++;
                    var toAdd = new TestEventRead 
                    { 
                        TestId = testId,
                        SubmitterId = submitterId,
                        ScannedLPN = scannedLPN,
                        EventId = eventId,
                    };
                    toReturn.Add(toAdd);
                }
                else
                {
                    Console.WriteLine($"Invalid test: Incorrect number of parameters: {test}. Ignoring.");
                }
            }
            return toReturn;
        }

        public static List<string> SerializeTests(List<TestEventRead> testEventReads)
        {
            List<string> toReturn = new List<string>();

            foreach (var testEventRead in testEventReads)
            {
                var toAdd = Utilities.Serialize(testEventRead);
                if (string.IsNullOrEmpty(toAdd))
                {
                    Console.WriteLine($"Unable to serialize the test event read object. Ignoring.\n{testEventRead}");
                }
                toReturn.Add(toAdd);
            }

            return toReturn;
        }

        public static List<TestResult> RunTests(List<string> serializedEventReadsToPOST, TestConfiguration testConfig)
        {
            List<TestResult> toReturn = new List<TestResult>();

            int testId = 0;
            foreach (var toPost in serializedEventReadsToPOST)
            {
                var postResult = Utilities.PostToAPI(toPost);
                var toAdd = new TestResult();
                testId++;
                toAdd.TestId = testId.ToString();
                toAdd.HttpReturnCode = postResult.Result.StatusCode.ToString();
                toReturn.Add(toAdd);
            }

            return toReturn;
        }

        public static bool WriteTestResultsToFile(List<TestResult> testResults, string resultsFilePath)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tr in testResults)
            {
                sb.AppendLine(tr.ToString());
            }
            try
            {
                var filePath = $"{resultsFilePath.TrimEnd('\\')}\\TestResult_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.txt";
                File.WriteAllText(filePath, sb.ToString());
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to write test results to file. Printing here:\n{sb}");
                return false;
            }
        }
    }

    public enum ConfigSource
    {
        FromFile,
        CreateOnTheFly,
    }
    public enum PayloadFormat
    {
        XML,
        JSON,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPREventReadTestSubmitter
{
    public class TestRunner
    {
        public static void Run(string[] args)
        {
            // parse the arguments: number of tests, file or create, XML or JSON, delay between POSTs; if no args, use defaults

            // if testing from file: read the tests from the test file into a list of EventRead objects
            // if making tests: build a list of EventRead objects
            var testEventReads = TestConfiguration.ReadEventReadsFromFile(@"C:\dev.net\Sandbox\LPREventReadTestSubmitter\Tests\Tests.txt");

            // serialize the list of EventRead objects into a list of strings to POST to the EventRead Submission API
            var serializedTestsToPOST = TestConfiguration.SerializeTests(testEventReads);

            // submit the tests to the API and log the results
            var testConfig = new TestConfiguration
            {
                NumberOfTests = serializedTestsToPOST.Count,
                DelayBetweenTests = 1000,
                PayloadFormat = PayloadFormat.XML,
                ConfigSource = ConfigSource.FromFile
            };
            var testResults = TestConfiguration.RunTests(serializedTestsToPOST, testConfig);

            var result = TestConfiguration.WriteTestResultsToFile(testResults, @"C:\dev.net\Sandbox\LPREventReadTestSubmitter\Tests\");
        }


    }
}

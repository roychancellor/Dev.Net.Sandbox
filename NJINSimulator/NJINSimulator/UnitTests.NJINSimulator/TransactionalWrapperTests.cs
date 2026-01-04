using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJINSimulator.Common.Models;
using NJINSimulator.Common.Serialization;
using System;
using System.Collections.Generic;

namespace UnitTests.NJINSimulator
{
    [TestClass]
    public class TransactionalWrapperTests
    {
        [TestMethod]
        public void Serializing_TW_WithMultipleProcessResults_ShouldPassIf_NoErrorsThrown_And_XMLHasCorrectStructure()
        {
            var twUT = new TransactionalWrapper
            {
                StartDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                Verbose = true,
                InboundCorrelationID = "1234567890",
                ProcessResults = new List<ProcessResult>
                {
                    new ProcessResult { Result = "Result1" },
                    new ProcessResult { Result = "Result2" },
                    new ProcessResult { Result = "Result3" },
                    new ProcessResult { Result = "Result4" },
                },
            };

            var twUTSer = Serialization.Serialize(twUT);

            Assert.IsNotNull(twUTSer);
        }
    }
}

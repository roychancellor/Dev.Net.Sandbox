using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJINSimulator.Common.HttpServices;
using NJINSimulator.Common.Models;
using NJINSimulator.Common.Serialization;
using NJINSimulator.Modules;
using System;

namespace UnitTests.NJINSimulator
{
    [TestClass]
    public class NJINServicesSenderManagerTests
    {
        [TestMethod]
        public void Handle_Success_ShouldPassIf_SendsTWToEndpoint()
        {
            var ut = new NJINServiceSenderManager(NJINServiceNames.SenderManager, "SNDMGR.IN", "http://localhost:9199/njinsimulator/io/api/v1/NSBIn", 50);

            var tw = new TransactionalWrapper
            {
                InboundCorrelationID = "1234567890"
            };

            ut.Handle(tw);
        }
    }
}

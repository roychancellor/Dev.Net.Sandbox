using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJINSimulator.Common.ActiveMQ;
using System;

namespace UnitTests.NJINSimulator
{
    [TestClass]
    public class ActiveMQTests
    {
        [TestMethod]
        public void AMQServices_Instance_Success_ShouldPassIf_NoErrorsThrown_And_InitializesConnectionToAMQ()
        {
            var testInstance = AMQServices.Instance;

            //testInstance.Init();

            Assert.IsNotNull(testInstance);
            Assert.IsNotNull(testInstance.Connection);
            Assert.IsTrue(testInstance.Connection.IsStarted);
            Assert.IsNotNull(testInstance.Session);

            testInstance.Close();
        }

        //[TestMethod]
        //public void AMQServices_Init_Success_ShouldPassIf_NoErrorsThrown_And_InitializesConnectionToAMQ()
        //{
        //    var testInstance = AMQServices.Instance;

        //    testInstance.Init();

        //    Assert.IsNotNull(testInstance.Connection);
        //    Assert.IsTrue(testInstance.Connection.IsStarted);
        //    Assert.IsNotNull(testInstance.Session);

        //    testInstance.Close();
        //}
    }
}

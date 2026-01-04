using Microsoft.VisualStudio.TestTools.UnitTesting;
using PROBESimulator.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Threading;
using UFOSimulator.Processors;

namespace UnitTests.UFOSimulator
{
    [TestClass]
    public class UFOProcessorNJINTPSTests
    {
        [TestMethod]
        public void PROBEProcessorNJINTPS_Process_TestingTheAlgorithm()
        {
            var delay = new Random();
            var currents = new List<int>();
            var probeUT = UFOProcessorNJINTPS.Instance;
            var toProcess = new NJINTPS
            {
                IBCID = "ABC123",
                TimeStamp = DateTime.Now,
                Verbose = true
            };
            for (int i = 0; i < 200; i++)
            {
                toProcess.TimeStamp = DateTime.Now;
                probeUT.Process(toProcess);
                currents.Add(probeUT.Current);
                Thread.Sleep(delay.Next(0,100));
            }
            var tpsReport = probeUT.TPSReport;
        }
    }
}

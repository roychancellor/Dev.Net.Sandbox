using Microsoft.VisualStudio.TestTools.UnitTesting;
using PROBESimulator.Processors;
using System;
using System.Collections.Generic;

namespace Tests.PROBESimulator
{
    [TestClass]
    public class UFOProcessorGeneralTests
    {
        [TestMethod]
        public void MovingAverage_ComputeMovingAverage_Success()
        {
            var maUT = new MovingAverage();
            var deltaTimes = new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 10.0, 9.0, 8.0, 7.0, 6.0, 5.0, 4.0, 3.0, 2.0, 1.0 };
            var movingAverages = new List<double>();
            foreach (var dt in deltaTimes) 
            {
                movingAverages.Add(maUT.ComputeMovingAverage(dt));
            }
            Assert.IsNotNull(movingAverages);
        }
    }
}

using LPREventRead.Monitor;
using LPREventRead.Monitor.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace LPREventRead.Monitor.Tests.Controllers
{
    [TestClass]
    public class MonitorControllerTest
    {
        [TestMethod]
        public void GetIdleMinutes()
        {
            // Arrange
            MonitorController controller = new MonitorController();
            var defaultResult = "15";

            // Act
            string result = controller.IdleMinutes();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.AreNotEqual(defaultResult, result);
        }
    }
}

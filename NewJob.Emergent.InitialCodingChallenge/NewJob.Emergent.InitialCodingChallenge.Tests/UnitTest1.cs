using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NewJob.Emergent.InitialCodingChallenge;

namespace NewJob.Emergent.InitialCodingChallenge.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void VersionCompare_v1_2_v2_2_ShouldPassIf_ReturnsExpectedResult()
        {
            var expected = 0;
            var result = CustomCode.VersionCompare("2", "2");
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void VersionCompare_v1_2_v2_20_ShouldPassIf_ReturnsExpectedResult()
        {
            var expected = 0;
            var result = CustomCode.VersionCompare("2", "2.0");
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void VersionCompare_v1_2_v2_200_ShouldPassIf_ReturnsExpectedResult()
        {
            var expected = 0;
            var result = CustomCode.VersionCompare("2", "2.0.0");
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void VersionCompare_v1_2_v2_2000_ShouldPassIf_ReturnsExpectedResult()
        {
            var expected = 0;
            var result = CustomCode.VersionCompare("2", "2.0.0.0");
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void VersionCompare_v1_2_v2_20000_ShouldPassIf_ReturnsExpectedResult()
        {
            var expected = 0;
            var result = CustomCode.VersionCompare("2", "2.0.0.0.0");
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void VersionCompare_v1_2_v2_20001_ShouldPassIf_ReturnsExpectedResult()
        {
            var expected = -1;
            var result = CustomCode.VersionCompare("2", "2.0.0.0.1");
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void VersionCompare_v1_2_v2_21_ShouldPassIf_ReturnsExpectedResult()
        {
            var expected = -1;
            var result = CustomCode.VersionCompare("2", "2.1");
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void VersionCompare_v1_210_v2_201_ShouldPassIf_ReturnsExpectedResult()
        {
            var expected = 1;
            var result = CustomCode.VersionCompare("2.1.0", "2.0.1");
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void VersionCompare_v1_21001_v2_21010_ShouldPassIf_ReturnsExpectedResult()
        {
            var expected = 1;
            var result = CustomCode.VersionCompare("2.10.0.1", "2.1.0.10");
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
        [TestMethod]
        public void VersionCompare_v1_201_v2_120001_ShouldPassIf_ReturnsExpectedResult()
        {
            var expected = 1;
            var result = CustomCode.VersionCompare("2.0.1", "1.2000.1");
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
    }
}

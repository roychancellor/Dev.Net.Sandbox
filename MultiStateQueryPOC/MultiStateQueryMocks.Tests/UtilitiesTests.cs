using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Common;
using Models.Query;
using MultiStateQueryMocks.Utilities;
using System;

namespace MultiStateQueryMocks.Tests
{
    [TestClass]
    public class UtilitiesTests
    {
        [TestMethod]
        public void Deserialize_SuccessPath_DQStringDeserializesHeader_ShouldPassIf_HeaderObject_ContainsCorrectProperties()
        {
            string toDeser = @"<NLETS><Header><SendORI>AZNLETS49</SendORI><DestORI>AZ</DestORI><ControlField>1234567890</ControlField><MessageKey>DQ</MessageKey></Header><InquiryData><Name>Roy</Name><DOB>2000-01-01</DOB><Sex>M</Sex><OLN>12345678</OLN><OLS>AZ</OLS></InquiryData></NLETS>";
            var deser = Utilities.Utilities.Deserialize<NLETS>(toDeser);

            Assert.IsNotNull(deser);
            Assert.IsNotNull(deser.Header);
            Assert.IsNotNull(deser.InquiryData);
            Assert.IsNotNull(deser.Header.SendORI);
            Assert.IsNotNull(deser.Header.DestORI);
            Assert.IsNotNull(deser.Header.ControlField);
            Assert.IsNotNull(deser.Header.MessageKey);
            Assert.AreEqual("AZNLETS49", deser.Header.SendORI);
            Assert.AreEqual("AZ", deser.Header.DestORI);
            Assert.AreEqual("1234567890", deser.Header.ControlField);
            Assert.AreEqual("DQ", deser.Header.MessageKey);
        }
    }
}

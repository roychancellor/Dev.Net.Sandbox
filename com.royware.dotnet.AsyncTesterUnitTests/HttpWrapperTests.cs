using com.royware.dotnet.AsyncTester;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace com.royware.dotnet.AsyncTesterUnitTests
{
    [TestClass]
    public class HttpWrapperTests
    {
        private string shouldContain = "<!DOCTYPE html>";
        
        [TestMethod]
        public async Task GetPayloadFromAPI_SuccessPath_ShouldPassIf_NoErrorsThrown_And_ReturnsExpectedContent()
        {
            HttpWrapper testHttpWrapper = new HttpWrapper();

            var testResult = await testHttpWrapper.GetPayloadFromAPI(1);

            Assert.IsTrue(testResult.Contains(shouldContain));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task GetPayloadFromAPI_FailurePath_ShouldPassIf_Throws_ArgumentNullException()
        {
            HttpWrapper testHttpWrapper = new HttpWrapper();

            var testResult = await testHttpWrapper.GetPayloadFromAPI(1, null);

            Assert.IsTrue(testResult.Contains(shouldContain));
        }
    }
}

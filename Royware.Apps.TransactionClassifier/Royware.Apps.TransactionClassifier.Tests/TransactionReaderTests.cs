using Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions;
using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Tests
{
    [TestClass]
    public sealed class TransactionReaderTests
    {
        [TestMethod]
        public void WellsFargo_ParseLine_Success_PositiveAmount_ShouldPassIf_ReturnsExpectedObject()
        {
            var transUT = Transactions.Csv_WellsFargo_Success_PositiveAmount;

            var ut = new WellsFargoTransactionReader();

            var result = ut.ParseLine(transUT.Transaction);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<Transaction>(result);
            Assert.AreEqual(transUT.ExpectedTransDate, result.TransactionDate);
            Assert.AreEqual(transUT.ExpectedAmount, result.Amount);
            Assert.AreEqual(transUT.ExpectedDescription, result.Description);
        }

        [TestMethod]
        public void WellsFargo_ParseLine_Success_NegativeAmount_ShouldPassIf_ReturnsExpectedObject()
        {
            var transUT = Transactions.Csv_WellsFargo_Success_NegativeAmount;

            var ut = new WellsFargoTransactionReader();

            var result = ut.ParseLine(transUT.Transaction);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<Transaction>(result);
            Assert.AreEqual(transUT.ExpectedTransDate, result.TransactionDate);
            Assert.AreEqual(transUT.ExpectedAmount, result.Amount);
            Assert.AreEqual(transUT.ExpectedDescription, result.Description);
        }
    }
}

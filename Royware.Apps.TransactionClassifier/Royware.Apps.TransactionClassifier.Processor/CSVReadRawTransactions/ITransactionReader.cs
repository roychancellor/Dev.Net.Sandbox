using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions
{
    public interface ITransactionReader
    {
        List<Models.Transaction> LoadFromFile(FileMetaData fileMetaData);
        Models.Transaction ParseLine(string transaction);
    }
}

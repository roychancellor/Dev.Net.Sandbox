namespace Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions
{
    public interface ITransactionReader
    {
        List<Models.Transaction> LoadFromFile(string fullPathToFile);
        Models.Transaction ParseLine(string transaction);
    }
}

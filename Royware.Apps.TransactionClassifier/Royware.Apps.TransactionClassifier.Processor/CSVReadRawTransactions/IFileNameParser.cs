using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions
{
    public interface IFileNameParser
    {
        bool TryParseFileName(string fullPathToFile, out FileMetaData fileMetaData);
    }
}

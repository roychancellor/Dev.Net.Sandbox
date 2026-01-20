using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions
{
    public class FileNameParser : IFileNameParser
    {
        public bool TryParseFileName(string fullPathToFile, out FileMetaData fileMetaData)
        {
            if (string.IsNullOrEmpty(fullPathToFile))
            {
                fileMetaData = FileMetaData.Build();
                return false;
            }

            var fileName = Path.GetFileName(fullPathToFile);
            var metaDataParts = fileName.Split('_');
            var isValidFileName = metaDataParts.Length >= 3;
            if (!isValidFileName)
            {
                fileMetaData = FileMetaData.Build();
                return false;
            }
            fileMetaData = FileMetaData.Build(metaDataParts[0], metaDataParts[1], metaDataParts[2]);
            fileMetaData.FullPathToFile = fullPathToFile;
            return true;
        }
    }
}

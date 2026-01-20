namespace Royware.Apps.TransactionClassifier.Processor.Models
{
    public class FileMetaData
    {
        public TransactionSources Source { get; set; }
        public Domains Domain { get; set; }
        public AccountTypes AccountType { get; set; }
        public string FullPathToFile { get; set; } = "";

        public static FileMetaData Build()
        {
            return Build("", "", "");
        }
        
        public static FileMetaData Build(string source, string domain, string accountType)
        {
            var resolvedSource = Enum.TryParse(source, out TransactionSources parsedSource) ? parsedSource : TransactionSources.WellsFargo;
            var resolvedDomain = Enum.TryParse(domain, out Domains parsedDomain) ? parsedDomain : Domains.PERSONAL;
            var resolvedAccountType = Enum.TryParse(accountType, out AccountTypes parsedAccountType) ? parsedAccountType : AccountTypes.OurChecking;

            return new FileMetaData
            {
                Source = resolvedSource,
                Domain = resolvedDomain,
                AccountType = resolvedAccountType,
            };
        }
    }
}

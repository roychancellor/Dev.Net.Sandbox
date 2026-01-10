namespace Royware.Apps.TransactionClassifier.Providers.ApplicationSettings
{
    public sealed class AppSettings
    {
        public string OpenAiApiKey { get; set; } = default!;
        public string Model { get; set; } = default!;
        public int BatchSize { get; set; }
        public double ConfidenceThreshold { get; set; }
        public string FullPathToTransactionsFile { get; set; } = "";
        public string DbConnString { get; set; } = "";
        public string ProcInsertSingleTransaction { get; set; } = "";
        public string ProcInsertMultipleTransactions { get; set; } = "";
        public string ProcGetUnresolvedTransactions { get; set; } = "";
        public string ViewGetAllMerchantRules { get; set; } = "";
        public string ProcUpdateMerchantRules { get; set; } = "";
        public string ProcUpdateBatchTransactions { get; set; } = "";
    }
}

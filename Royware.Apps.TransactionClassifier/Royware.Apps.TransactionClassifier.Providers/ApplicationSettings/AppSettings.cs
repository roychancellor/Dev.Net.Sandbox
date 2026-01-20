namespace Royware.Apps.TransactionClassifier.Providers.ApplicationSettings
{
    public sealed class AppSettings
    {
        public string AiRequestUrl { get; set; } = "";
        public string AiModel { get; set; } = default!;
        public int AiMaxOutputTokens { get; set; }
        public int BatchSize { get; set; }
        public double ConfidenceThreshold { get; set; }
        public string FullPathToTransactionsFile { get; set; } = "";
        public string DbConnString { get; set; } = "";
        public string ProcInsertSingleTransaction { get; set; } = "";
        public string ProcInsertMultipleTransactions { get; set; } = "";
        public string ProcGetUnresolvedTransactions { get; set; } = "";
        public string ViewGetAllMerchantRules { get; set; } = "";
        public string ViewGetAllCategories { get; set; } = "";
        public string ProcInsertMerchantRules { get; set; } = "";
        public string ProcUpdateBatchTransactions { get; set; } = "";
        public string FullPathToExportTransactions { get; set; } = "";
    }
}

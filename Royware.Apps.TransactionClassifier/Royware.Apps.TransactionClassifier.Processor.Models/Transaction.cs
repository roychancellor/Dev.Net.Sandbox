namespace Royware.Apps.TransactionClassifier.Processor.Models
{
    public class Transaction
    {
        public long TransactionId { get; set; }
        public byte[] ExternalTransactionHash { get; set; } = [];
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Domain { get; set; } = "";
        public string AccountType { get; set; } = "";
        
        // Resolved during processing
        public string Category { get; set; } = "";
        public string ResolvedMerchant { get; set; } = "";
        public long? MatchedRuleId { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

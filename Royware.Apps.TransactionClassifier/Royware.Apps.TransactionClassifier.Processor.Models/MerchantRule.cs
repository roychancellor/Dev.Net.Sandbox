namespace Royware.Apps.TransactionClassifier.Processor.Models
{
    public class MerchantRule
    {
        public long MerchantRuleId { get; set; }
        public string NormalizedMerchant { get; set; } = "";
        public string Domain { get; set; } = "";
        public string AccountType { get; set; } = "";
        public string Category { get; set; } = "";
        public List<string> RequiredTerms { get; set; } = [];
        public List<string> ExcludedTerms { get; set; } = [];
        public int Priority { get; set; }
        public decimal Confidence { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

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

        public override string ToString()
        {
            return $"NormalizedMerchant: {NormalizedMerchant} | Required Terms: {string.Join(',', RequiredTerms)} | Excluded Terms: {string.Join(',', ExcludedTerms)}";
        }

        public static MerchantRule MappedFrom(MerchantRuleProposal mrp, FileMetaData fmd)
        {
            return new MerchantRule
            {
                NormalizedMerchant = mrp.NormalizedMerchant,
                Domain = fmd.Domain.ToString(),
                AccountType = fmd.AccountType.ToString(),
                Category = mrp.Category,
                RequiredTerms = mrp.RequiredTerms,
                ExcludedTerms = mrp.ExcludedTerms,
                Priority = 1,
                Confidence = mrp.Confidence ?? 1,
                IsActive = true,
                CreatedAt = DateTime.Now,
            };
        }
    }
}

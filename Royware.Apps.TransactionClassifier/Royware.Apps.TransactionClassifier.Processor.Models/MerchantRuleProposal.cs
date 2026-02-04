using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace Royware.Apps.TransactionClassifier.Processor.Models
{
    public class MerchantRuleProposal
    {
        public long TransactionId { get; set; }
        public string NormalizedMerchant { get; set; } = "";
        public string Category { get; set; } = "";
        public List<string> RequiredTerms { get; set; } = [];
        public List<string> ExcludedTerms { get; set; } = [];
        public decimal? Confidence { get; set; }
        public string? Notes { get; set; }
        public string MerchantRuleCorrelation { get; set; } = "";

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{TransactionId} | {NormalizedMerchant} | {Category} | {string.Join(',', RequiredTerms)} | {string.Join(',', ExcludedTerms)} | {Confidence} | {Notes}");
            return sb.ToString();
        }

        public MerchantRuleProposal Clone()
        {
            return new MerchantRuleProposal
            {
                TransactionId = TransactionId,
                NormalizedMerchant = NormalizedMerchant.Clone() as string ?? "",
                Category = Category.Clone() as string ?? "",
                RequiredTerms = [.. RequiredTerms],
                ExcludedTerms = [.. ExcludedTerms],
                Confidence = Confidence,
                Notes = Notes?.Clone() as string ?? "",
                MerchantRuleCorrelation = MerchantRuleCorrelation?.Clone() as string ?? "",
            };
        }
    }
}

using System;
using System.Collections.Generic;
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
    }
}

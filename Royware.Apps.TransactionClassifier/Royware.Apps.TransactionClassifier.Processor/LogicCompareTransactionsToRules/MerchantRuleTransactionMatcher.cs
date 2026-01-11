using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.LogicCompareTransactionsToRules
{
    public class MerchantRuleTransactionMatcher : IMerchantRuleTransactionMatcher
    {
        private static readonly Logger _log = Loggers.Batch;
        private static readonly Logger _traceLog = Loggers.BatchTrace;

        public MerchantRule? MatchTransactionToRule(Transaction tx, List<MerchantRule> rules)
        {
            // Apply deterministic matching logic (RequiredTerms, ExcludedTerms, Domain, AccountType)
            /*
             public string Domain { get; set; } = "";
        public string AccountType { get; set; } = "";
        public string Category { get; set; } = "";
        public List<string> RequiredTerms { get; set; } = [];
        public List<string> ExcludedTerms { get; set; } = [];
             */
            var rulesTheMatch_Domain_AccountType_Category = rules.Where(r => r.Domain.CompareTo(tx.Domain, StringComparison.OrdinalIgnoreCase) == 0 &&
                                                                             r.AccountType.CompareTo(tx.AccountType, StringComparison.OrdinalIgnoreCase) == 0 &&
                                                                             r.Category.CompareTo(tx.Category, StringComparison.OrdinalIgnoreCase) == 0);
            if (!rulesTheMatch_Domain_AccountType_Category.Any())
            {
                return default;
            }
            var matchedRule = rulesTheMatch_Domain_AccountType_Category
                             .FirstOrDefault(r => r.RequiredTerms.All(t => tx.Description.Contains(t)) &&
                                                 !r.ExcludedTerms.Any(e => tx.Description.Contains(e)));
            return matchedRule;
        }
    }
}

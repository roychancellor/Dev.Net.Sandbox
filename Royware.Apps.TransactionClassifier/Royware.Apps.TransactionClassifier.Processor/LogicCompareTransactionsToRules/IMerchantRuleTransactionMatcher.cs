using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.LogicCompareTransactionsToRules
{
    public interface IMerchantRuleTransactionMatcher
    {
        MerchantRule? MatchTransactionToRule(Transaction tx, List<MerchantRule> rules);
        MerchantRule? GetBestRule(Transaction tx, IEnumerable<MerchantRule> rules);
    }
}

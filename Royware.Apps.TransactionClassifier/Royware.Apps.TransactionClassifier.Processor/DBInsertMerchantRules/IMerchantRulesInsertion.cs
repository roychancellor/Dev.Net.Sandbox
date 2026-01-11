using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBInsertMerchantRules
{
    public interface IMerchantRulesInsertion
    {
        Task<int> InsertMerchantRules(List<MerchantRule> merchantRules);
    }
}

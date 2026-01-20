using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBRepository
{
    public interface IMerchantRulesRepository
    {
        Task<List<MerchantRule>> RetrieveAllMerchantRules();
        Task<int> InsertMerchantRules(List<MerchantRule> merchantRules);
    }
}

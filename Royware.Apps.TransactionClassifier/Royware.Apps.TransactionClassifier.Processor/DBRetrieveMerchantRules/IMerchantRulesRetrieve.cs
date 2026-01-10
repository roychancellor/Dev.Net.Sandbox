using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBRetrieveMerchantRules
{
    public interface IMerchantRulesRetrieve
    {
        Task<List<MerchantRule>> RetrieveActiveMerchantRules();
    }
}

using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.DBRepository;
using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBRetrieveMerchantRules
{
    public class MerchantRulesRetriever : IMerchantRulesRetrieve
    {
        private static readonly Logger _log = Loggers.Batch;
        private readonly IMerchantRulesRepository _repo;

        public MerchantRulesRetriever(IMerchantRulesRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<MerchantRule>> RetrieveActiveMerchantRules()
        {
            try
            {
                return await _repo.RetrieveAllMerchantRules();
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"While retrieving active merchant rules");
                throw;
            }
        }
    }
}

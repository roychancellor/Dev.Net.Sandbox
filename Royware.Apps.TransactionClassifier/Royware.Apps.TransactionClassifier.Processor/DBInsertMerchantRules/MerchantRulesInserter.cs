using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.DBRepository;
using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.DBInsertMerchantRules
{
    public class MerchantRulesInserter : IMerchantRulesInsertion
    {
        private static readonly Logger _log = Loggers.Batch;
        private readonly IMerchantRulesRepository _repo;

        public MerchantRulesInserter(IMerchantRulesRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> InsertMerchantRules(List<MerchantRule> merchantRules)
        {
            if (merchantRules == null || merchantRules.Count == 0)
            {
                _log.Error($"Passed in merchant rules list to insert is null or empty");
                return 0;
            }

            try
            {
                _log.Info($"Inserting merchant rules | TOTAL POSSIBLE: {merchantRules.Count}");
                var numInserted = await _repo.InsertMerchantRules(merchantRules);
                var caveatText = merchantRules.Count != numInserted ? " (the procedure will not insert duplicate rules)" : "";
                _log.Info($"Rules inserted | ACTUAL COUNT: {numInserted}{caveatText}");

                return numInserted;
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"While inserting merchant rules");
                throw;
            }
        }
    }
}

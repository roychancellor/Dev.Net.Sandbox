using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.LogicGenerateUnmatchedRules
{
    public interface IMerchantRulesGeneration
    {
        string PrepareAIRequestPayload(List<Transaction> batchTransactions, List<Category> categories);
        Task<List<MerchantRuleProposal>> GetMerchantRuleProposalsAsync(string requestAsJson,
                                                                       CancellationToken cancellationToken);
        List<MerchantRule> HumanReview(List<MerchantRuleProposal> candidateRules, FileMetaData fileMeta, List<Transaction> currentBatch);
    }
}

using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.LogicGenerateUnmatchedRules
{
    public interface IMerchantRulesGeneration
    {
        object PrepareAIPayload(List<Transaction> unmatched, List<string> knownCategories);
        List<MerchantRule> CallAIForCandidateRules(object payload);
        Task<List<MerchantRuleProposal>> GetMerchantRuleProposalsAsync(List<Transaction> transactions,
                                                                       List<Category> knownCategories,
                                                                       CancellationToken cancellationToken);
        List<MerchantRule> HumanReview(List<MerchantRuleProposal> candidateRules);
    }
}

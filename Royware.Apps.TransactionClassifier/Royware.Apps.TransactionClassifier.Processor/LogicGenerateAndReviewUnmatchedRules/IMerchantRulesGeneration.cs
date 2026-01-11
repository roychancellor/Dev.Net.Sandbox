using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.LogicGenerateUnmatchedRules
{
    public interface IMerchantRulesGeneration
    {
        object PrepareAIPayload(List<Transaction> unmatched, List<string> knownCategories);
        List<MerchantRule> CallAIForCandidateRules(object payload);
        List<MerchantRule> HumanReview(List<MerchantRule> candidateRules);
    }
}

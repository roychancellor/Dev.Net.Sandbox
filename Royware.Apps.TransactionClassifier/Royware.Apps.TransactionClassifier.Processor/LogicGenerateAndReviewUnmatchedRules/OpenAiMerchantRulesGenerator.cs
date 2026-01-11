using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.LogicGenerateUnmatchedRules
{
    public class OpenAiMerchantRulesGenerator : IMerchantRulesGeneration
    {
        public List<MerchantRule> CallAIForCandidateRules(object payload)
        {
            throw new NotImplementedException();
        }

        public List<MerchantRule> HumanReview(List<MerchantRule> candidateRules)
        {
            throw new NotImplementedException();
        }

        public object PrepareAIPayload(List<Transaction> unmatched, List<string> knownCategories)
        {
            throw new NotImplementedException();
        }
    }
}

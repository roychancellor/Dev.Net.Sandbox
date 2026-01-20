using NLog;
using Royware.Apps.TransactionClassifier.Logging;
using Royware.Apps.TransactionClassifier.Processor.Models;

namespace Royware.Apps.TransactionClassifier.Processor.LogicCompareTransactionsToRules
{
    public class MerchantRuleTransactionMatcher : IMerchantRuleTransactionMatcher
    {
        private static readonly Logger _log = Loggers.Batch;
        private static readonly Logger _traceLog = Loggers.BatchTrace;

        public MerchantRule? MatchTransactionToRule(Transaction tx, List<MerchantRule> activeRules)
        {
            // ELIGIBLE rules
            var eligibleRules = activeRules
                               .Where(r => IsMatch(tx, r))
                               .Select(r => new { Rule = r, Specificity = ComputeSpecificityScore(tx, r) })
                               .ToList();

            if (eligibleRules.Count == 0)
            {
                return null;
            }

            // Select winner deterministically
            var winner = eligibleRules
                        .OrderByDescending(x => x.Specificity)
                        .ThenByDescending(x => x.Rule.Priority)
                        .ThenBy(x => x.Rule.MerchantRuleId)
                        .FirstOrDefault()?
                        .Rule;

            return winner;
        }

        private static bool IsMatch(Transaction tx, MerchantRule r)
        {
            // Domain / AccountType / Category must match exactly
            if (!r.Domain.Equals(tx.Domain, StringComparison.OrdinalIgnoreCase) ||
                !r.AccountType.Equals(tx.AccountType, StringComparison.OrdinalIgnoreCase) ||
                !r.Category.Equals(tx.Category, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // All required terms must appear and no excluded terms may appear
            if (!r.RequiredTerms.All(t => tx.Description.Contains(t, StringComparison.OrdinalIgnoreCase)) ||
                 r.ExcludedTerms.Any(e => tx.Description.Contains(e, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            return true;
        }

        private static int ComputeSpecificityScore(Transaction tx, MerchantRule r)
        {
            int score = 0;

            // Count required terms that matched
            score += r.RequiredTerms.Count(t => tx.Description.Contains(t, StringComparison.OrdinalIgnoreCase));

            // Count excluded terms that correctly did NOT match
            score += r.ExcludedTerms.Count(e => !tx.Description.Contains(e, StringComparison.OrdinalIgnoreCase));

            return score;
        }
    }
}

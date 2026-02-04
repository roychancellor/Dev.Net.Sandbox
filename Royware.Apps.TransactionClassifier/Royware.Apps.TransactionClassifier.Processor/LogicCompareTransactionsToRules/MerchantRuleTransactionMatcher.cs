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
                               .Where(r => IsApplicable(tx, r))
                               .Select(r => new { Rule = r, Specificity = CalculateSpecificity(r) })
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

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        // 1️⃣ Checks if rule applies (for unresolved TXs)
        public static bool IsApplicable(Transaction tx, MerchantRule r)
        {
            if (!DomainMatches(tx, r)) return false;
            if (!AccountTypeMatches(tx, r)) return false;
            if (!RequiredTermsMatch(tx, r)) return false;
            if (!ExcludedTermsMatch(tx, r)) return false;
            // NOTE: Category is ignored for unresolved transactions
            return true;
        }

        // 2️⃣ Checks if rule is consistent with a resolved transaction
        public static bool IsConsistent(Transaction tx, MerchantRule r)
        {
            if (!IsApplicable(tx, r)) return false;
            if (tx.IsResolved && !CategoryMatches(tx, r)) return false;
            return true;
        }

        // --- Helper methods ---
        private static bool DomainMatches(Transaction tx, MerchantRule r)
            => string.Equals(r.Domain, tx.Domain, StringComparison.OrdinalIgnoreCase);

        private static bool AccountTypeMatches(Transaction tx, MerchantRule r)
            => string.Equals(r.AccountType, tx.AccountType, StringComparison.OrdinalIgnoreCase);

        private static bool CategoryMatches(Transaction tx, MerchantRule r)
            => string.Equals(r.Category, tx.Category, StringComparison.OrdinalIgnoreCase);

        private static bool RequiredTermsMatch(Transaction tx, MerchantRule r)
            => r.RequiredTerms.All(term => tx.Description.Contains(term, StringComparison.OrdinalIgnoreCase));

        private static bool ExcludedTermsMatch(Transaction tx, MerchantRule r)
            => r.ExcludedTerms.All(term => !tx.Description.Contains(term, StringComparison.OrdinalIgnoreCase));

        private static int CalculateSpecificity(MerchantRule r)
        {
            int score = 0;
            score += r.RequiredTerms.Count * 10;   // Required terms are more important
            score += r.ExcludedTerms.Count * 5;    // Excluded terms matter too
            if (!string.IsNullOrWhiteSpace(r.Domain)) score += 10;  // Domain-specific bonus
            if (!string.IsNullOrWhiteSpace(r.AccountType)) score += 5; // Account type bonus
            return score;
        }

        // Get the best matching rule for a transaction
        public MerchantRule? GetBestRule(Transaction tx, IEnumerable<MerchantRule> rules)
        {
            var applicableRules = rules
                .Where(r => IsApplicable(tx, r))
                .ToList();

            if (applicableRules.Count == 0)
                return null; // No match, send to AI pipeline

            // Rank by specificity score, then creation date
            var ranked = applicableRules
                .OrderByDescending(r => CalculateSpecificity(r))
                .ThenByDescending(r => r.CreatedAt)
                .ToList();

            // Winner
            var topRule = ranked.First();

            // Optional: log conflicts if multiple rules with different categories exist
            var conflicting = ranked.Skip(1)
                .Where(r => !string.Equals(r.Category, topRule.Category, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (conflicting.Count > 0)
            {
                _log.Warn($"Conflict detected for TX '{tx.Description}': " +
                    $"TopRule={topRule.Category}, Conflicts={string.Join(", ", conflicting.Select(r => r.Category))}");
            }

            return topRule;
        }

        // Assign category from best rule to resolved transaction
        public void ApplyBestRule(Transaction tx, IEnumerable<MerchantRule> rules)
        {
            var bestRule = GetBestRule(tx, rules);
            if (bestRule != null)
            {
                tx.ApplyMerchantRule(bestRule);
            }
        }
    }
}

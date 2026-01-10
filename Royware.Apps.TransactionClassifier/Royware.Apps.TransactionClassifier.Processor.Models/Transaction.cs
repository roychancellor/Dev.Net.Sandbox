using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Royware.Apps.TransactionClassifier.Processor.Models
{
    public partial class Transaction
    {
        public long TransactionId { get; set; }
        public byte[] ExternalTransactionHash { get; set; } = [];
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Domain { get; set; } = "";
        public string AccountType { get; set; } = "";
        
        // Resolved during processing
        public string Category { get; set; } = "";
        public string ResolvedMerchant { get; set; } = "";
        public long? MatchedRuleId { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }

        [GeneratedRegex(@"[^\w\s]")]
        private static partial Regex RemovePunctuationRegex();

        [GeneratedRegex(@"[\b\d+\b]")]
        private static partial Regex RemoveDigitsRegex();

        [GeneratedRegex(@"[\s+]")]
        private static partial Regex RemoveWhitespaceRegex();

        public void SetHash()
        {
            ExternalTransactionHash = SHA256.HashData(Encoding.UTF8.GetBytes(TransAsString()));
        }

        public string TransAsString()
        {
            return $"{Domain}|{AccountType}|{TransactionDate:yyyy-MM-dd}|{Amount}|{Description}"; // Description is AFTER normalization
        }

        public bool IsProcessable()
        {
            return !(
                        string.IsNullOrEmpty(Description) ||
                        TransactionDate == DateTime.MinValue ||
                        string.IsNullOrEmpty(Domain) ||
                        string.IsNullOrEmpty(AccountType)
                    );
        }

        public void Normalize()
        {
            var raw = Description;
            if (string.IsNullOrWhiteSpace(raw))
            {
                return;
            }

            var normalized = raw.ToUpperInvariant();

            normalized = RemovePunctuationRegex().Replace(normalized, "");

            normalized = RemoveDigitsRegex().Replace(normalized, "");

            normalized = RemoveWhitespaceRegex().Replace(normalized, "").Trim();

            Description = normalized;
        }
    }
}

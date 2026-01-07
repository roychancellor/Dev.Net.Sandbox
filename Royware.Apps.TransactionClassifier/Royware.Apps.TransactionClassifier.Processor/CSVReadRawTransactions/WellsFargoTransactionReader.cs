using Royware.Apps.TransactionClassifier.Processor.Models;
using System.Text.RegularExpressions;

namespace Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions
{
    public partial class WellsFargoTransactionReader : AbstractTransactionReader
    {
        [GeneratedRegex("(?<Date>[0-9]{2}\\/[0-9]{2}\\/[0-9]{4}),(?<Amount>(\\-)?[0-9.]+),\\*,(?<CheckNumber>[\\w\\s\\d]{0,}),(?<Description>[\\w\\d\\s\\W]+)")]
        private static partial Regex WellsFargoTransactionRegex();

        public override Transaction ParseLine(string transaction)
        {
            var toReturn = new Transaction();

            if (string.IsNullOrEmpty(transaction))
            {
                Log.Error($"The transaction string is null or empty");
                return toReturn;
            }

            // 05/09/2025,-39.66,*,,APS electric pmt PAYMENTS 250507 7848151000 CHANCELLOR,ROBYN
            transaction = transaction.Replace("\"", "");
            var matches = WellsFargoTransactionRegex().Matches(transaction);
            if (matches.Count == 0)
            {
                Log.Error($"The transaction does not match the expected pattern | TRANS: {transaction}");
                return toReturn;
            }

            var transGroups = matches[0].Groups;
            var dateToParse = transGroups[TransParts.Date.ToString()].Value;
            var amountToParse = transGroups[TransParts.Amount.ToString()].Value;
            if (!DateTime.TryParse(dateToParse, out DateTime transDate))
            {
                Log.Error($"Unable to parse the transaction date | TO PARSE: {dateToParse}");
                return toReturn;
            }
            if (!decimal.TryParse(amountToParse, out decimal amount))
            {
                Log.Error($"Unable to parse the transaction amount | TO PARSE: {amountToParse}");
                return toReturn;
            }

            toReturn.TransactionDate = transDate;
            toReturn.Amount = amount;
            toReturn.Description = transGroups[TransParts.Description.ToString()].Value;

            return toReturn;
        }
    }

    internal enum TransParts
    {
        Date = 0,
        Amount = 1,
        Asterisk = 2,
        CheckNumber = 3,
        Description = 4,
    }
}

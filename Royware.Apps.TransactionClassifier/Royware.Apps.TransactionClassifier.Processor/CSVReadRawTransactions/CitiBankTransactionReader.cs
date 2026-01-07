using Royware.Apps.TransactionClassifier.Processor.Models;
using System.Text.RegularExpressions;

namespace Royware.Apps.TransactionClassifier.Processor.CSVReadRawTransactions
{
    public partial class CitiBankTransactionReader : AbstractTransactionReader
    {
        //Status,Date,Description,Debit,Credit,Member Name
        //Cleared,12/02/2025,"AMAZON MKTPL*BB8YM4QX0 Amzn.com/billWA",25.00,,
        //Cleared,12/02/2025,"WILDBERRIES MESA AZ",36.32,,ROBYN CHANCELLOR
        //Cleared,12/08/2025,"ONLINE PAYMENT, THANK YOU",,-1889.35,ROY S CHANCELLOR
        [GeneratedRegex("(?<Status>[\\w]+),(?<Date>[0-9]{2}\\/[0-9]{2}\\/[0-9]{4}),(?<Description>[\\w\\d\\s\\W]+),(?<Debit>[0-9.]{0,}),(?<Credit>([0-9.\\-]){0,}),(?<Person>[\\w\\s]{0,})")]
        private static partial Regex CitiBankTransactionRegex();

        public CitiBankTransactionReader(IFileNameParser fileNameParser) : base(fileNameParser)
        {
        }

        public override Transaction ParseLine(string transaction)
        {
            var toReturn = new Transaction();

            if (string.IsNullOrEmpty(transaction))
            {
                Log.Error($"The transaction string is null or empty");
                return toReturn;
            }
            if (transaction.StartsWith("Status,Date,Description"))
            {
                return toReturn;
            }

            transaction = transaction.Replace("\"", "");
            var matches = CitiBankTransactionRegex().Matches(transaction);
            if (matches.Count == 0)
            {
                Log.Error($"The CitiBank transaction does not match the expected pattern | TRANS: {transaction}");
                return toReturn;
            }

            var transGroups = matches[0].Groups;
            var dateToParse = transGroups[TransParts.Date.ToString()].Value;
            string amountToParse = $"{transGroups[TransParts.Debit.ToString()].Value}{transGroups[TransParts.Credit.ToString()].Value}";
            if (!DateTime.TryParse(dateToParse, out DateTime transDate))
            {
                Log.Error($"Unable to parse the CitiBank transaction date | TO PARSE: {dateToParse}");
                return toReturn;
            }
            if (!decimal.TryParse(amountToParse, out decimal amount))
            {
                Log.Error($"Unable to parse the CitiBanktransaction amount | TO PARSE: {amountToParse}");
                return toReturn;
            }

            toReturn.TransactionDate = transDate;
            toReturn.Amount = amount;
            toReturn.Description = transGroups[TransParts.Description.ToString()].Value;

            return toReturn;
        }
    }
}

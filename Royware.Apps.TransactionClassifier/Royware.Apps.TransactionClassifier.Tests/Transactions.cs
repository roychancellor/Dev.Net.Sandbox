namespace Royware.Apps.TransactionClassifier.Tests
{
    public class Transactions
    {
        public const string CSV_WELLS_FARGO_SUCCESS_POSITIVE_AMOUNT = @"""12/23/2025"",""1600.00"",""*"","""",""ONLINE TRANSFER FROM ELIJAH HOME INVESTMENTS LLC BUSINESS CHECKING XXXXXX7652 REF #IB0W72Q3Z7 ON 12/23/25""";
        public const string CSV_WELLS_FARGO_SUCCESS_NEGATIVE_AMOUNT = @"""12/22/2025"",""-200.00"",""*"","""",""ATM WITHDRAWAL AUTHORIZED ON 12/22 1004 W Chandler Blvd Chandler AZ 0004327 ATM ID 5738V CARD 1371""";
        public static readonly TransactionTest Csv_WellsFargo_Success_PositiveAmount = new()
        {
            ExpectedTransDate = DateTime.Parse("12/23/2025"),
            ExpectedAmount = decimal.Parse("1600.00"),
            ExpectedDescription = @"ONLINE TRANSFER FROM ELIJAH HOME INVESTMENTS LLC BUSINESS CHECKING XXXXXX7652 REF #IB0W72Q3Z7 ON 12/23/25",
            Transaction = CSV_WELLS_FARGO_SUCCESS_POSITIVE_AMOUNT,
        };
        public static readonly TransactionTest Csv_WellsFargo_Success_NegativeAmount = new()
        {
            ExpectedTransDate = DateTime.Parse("12/22/2025"),
            ExpectedAmount = decimal.Parse("-200.00"),
            ExpectedDescription = @"ATM WITHDRAWAL AUTHORIZED ON 12/22 1004 W Chandler Blvd Chandler AZ 0004327 ATM ID 5738V CARD 1371",
            Transaction = CSV_WELLS_FARGO_SUCCESS_NEGATIVE_AMOUNT,
        };
    }

    public class TransactionTest
    {
        public DateTime ExpectedTransDate { get; set; }
        public decimal ExpectedAmount { get; set; }
        public string? ExpectedDescription { get; set; }
        public string Transaction { get; set; } = "";
    }
}

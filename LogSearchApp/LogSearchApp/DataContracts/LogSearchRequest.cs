using System.Text;

namespace LogSearchApp.DataContracts
{
    public class LogSearchRequest
    {
        // The keyword to search for in the message or any other relevant fields
        public string Keyword { get; set; } = "";

        // The field in the JSON log to filter by, e.g. "UserId"
        public string Field { get; set; } = "";

        // The value to match in the specified field
        public string Value { get; set; } = "";

        // The start date for the date range filter
        public DateTime? StartDate { get; set; }

        // The end date for the date range filter
        public DateTime? EndDate { get; set; }

        public override string ToString()
        {
            var fieldValues = new List<string>
            {
                $"{nameof(Keyword)}: {Keyword}",
                $"{nameof(Field)}: {Field}",
                $"{nameof(Value)}: {Value}",
                $"{nameof(StartDate)}: {StartDate?.ToString("yyyy-MM-dd HH:mm:ss")}",
                $"{nameof(EndDate)}: {EndDate?.ToString("yyyy-MM-dd HH:mm:ss")}",
            };
            return string.Join('|', fieldValues);
        }
    }
}

using LogSearchApp.Models;

namespace LogSearchApp.DataContracts
{
    public class LogSearchResult
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public static LogSearchResult From(LogEntry logEntry)
        {
            return new LogSearchResult
            {
                Id = logEntry.Id,
                Message = logEntry.Message,
                Timestamp = logEntry.Timestamp
            };
        }
    }
}

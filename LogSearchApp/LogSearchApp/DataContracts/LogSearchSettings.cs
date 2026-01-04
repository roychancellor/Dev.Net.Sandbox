namespace LogSearchApp.DataContracts
{
    public class LogSearchSettings
    {
        public List<string> LogFileDirectories { get; set; } = [];
        public string LogFileSearchPattern { get; set; } = "";
        public int MaxResults { get; set; }
    }
}
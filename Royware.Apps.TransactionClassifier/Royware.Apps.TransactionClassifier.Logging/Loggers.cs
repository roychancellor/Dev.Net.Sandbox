using NLog;

namespace Royware.Apps.TransactionClassifier.Logging
{
    public static class Loggers
    {
        public static Logger App { get; set; }
        public static Logger Batch { get; set; }
        public static Logger BatchTrace { get; set; }

        static Loggers()
        {
            App = LogManager.GetLogger(LoggerTypes.Application.ToString());
            Batch = LogManager.GetLogger(LoggerTypes.Batch.ToString());
            BatchTrace = LogManager.GetLogger(LoggerTypes.BatchTrace.ToString());
        }
    }

    internal enum LoggerTypes
    {
        Application = 0,
        Batch,
        BatchTrace,
    }
}

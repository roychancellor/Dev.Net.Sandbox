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

        public static void Validate()
        {
            var namedTargets = LogManager.Configuration?.ConfiguredNamedTargets;
            if (namedTargets == null || namedTargets.Count == 0)
            {
                throw new InvalidDataException($"NLog configuration is invalid or not loaded. Check that nlog.config exists in deployment folder.");
            }

            var allLoggers = Enum.GetNames<LoggerTypes>();
            if (allLoggers.Length != namedTargets.Count)
            {
                throw new InvalidDataException($"NLog configuration does not contain a definition for all {nameof(LoggerTypes)} loggers.");
            }

            foreach (var loggerName in allLoggers)
            {
                if (!namedTargets.Any(t => t.Name.Equals($"{loggerName}Logger")))
                {
                    throw new InvalidDataException($"NLog configuration is does not contain a logger with name {loggerName}.");
                }
            }
        }
    }

    internal enum LoggerTypes
    {
        Application = 0,
        Batch,
        BatchTrace,
    }
}

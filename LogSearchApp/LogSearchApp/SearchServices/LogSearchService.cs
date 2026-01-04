using Microsoft.Extensions.Options;
using System.Text.Json;
using LogSearchApp.Models;
using NLog;
using LogSearchApp.DataContracts;
using System.Reflection;
using LogSearchApp.Extensions;

namespace LogSearchApp.SearchServices
{
    public class LogSearchService
    {
        private static readonly Logger _log = LogManager.GetLogger(LoggerType.Request.ToString());
        private readonly List<string> _logFileDirectories;
        private readonly int _maxResults;
        private readonly string _logFileSearchPattern;
        private readonly Dictionary<string, string> _idToFileMapping;
        private readonly int _maxIdToFileMappingSize = 1000; // Max number of entries in the dictionary (adjustable)
        private readonly Queue<string> _idOrderQueue; // Queue to track the order of IDs

        public LogSearchService(IOptionsMonitor<LogSearchSettings> options)
        {
            _logFileDirectories = options.CurrentValue.LogFileDirectories;
            _maxResults = options.CurrentValue.MaxResults;
            _logFileSearchPattern = options.CurrentValue.LogFileSearchPattern;
            _idToFileMapping = [];
            _idOrderQueue = [];
        }

        #region Search Methods
        public async Task<List<LogSearchResult>> SearchAsync(LogSearchRequest searchRequest)
        {
            var id = Guid.NewGuid().ToString().Replace("-", "")[..10];
            _log.Info($"====> Search starting | ID: {id}");
            _log.Trace($"Search Request:\n{searchRequest}".PrefixWith(id));
            var results = new List<LogSearchResult>();

            var logFilesToSearch = GetLogFilesFromDirectories()
                                  .OrderBy(f => File.GetLastWriteTime(f))
                                  .ToList();

            if (logFilesToSearch.Count == 0)
            {
                _log.Warn($"No log files found in the specified directories\n{string.Join('\n', _logFileDirectories)}".PrefixWith(id));
                LogSearchComplete(id);
                return results;
            }

            _log.Info($"Found {logFilesToSearch.Count} to search".PrefixWith(id));
            _log.Info($"Searching files now".PrefixWith(id));

            var numberOfFilesOutsideSearchDateRange = 0;
            bool isPastSearchEnd = false;
            DateTime lastValidTimestamp = DateTime.MinValue;

            foreach (var logFile in logFilesToSearch)
            {
                var lastModified = File.GetLastWriteTime(logFile);

                if (IsModifiedBeforeStartDate(searchRequest, lastModified))
                {
                    numberOfFilesOutsideSearchDateRange++;
                    continue;
                }
                if (IsModifiedAfterEndDate(searchRequest, lastModified))
                {
                    if (!isPastSearchEnd)
                    {
                        isPastSearchEnd = true;
                        lastValidTimestamp = lastModified;
                    }
                    else if (lastModified > lastValidTimestamp)
                    {
                        numberOfFilesOutsideSearchDateRange++;
                        continue;
                    }
                }

                _log.Trace($"Searching file | {logFile}");
                var lines = await File.ReadAllLinesAsync(logFile);
                foreach (var line in lines)
                {
                    try
                    {
                        var logEntry = JsonSerializer.Deserialize<LogEntry>(line);
                        if (logEntry == null) continue;

                        bool foundMatches = true;
                        if (TimestampIsBeforeStartDate(searchRequest, logEntry)) foundMatches = false;
                        if (TimestampIsAfterEndDate(searchRequest, logEntry)) foundMatches = false;
                        if (ShouldSearchByFieldValue(searchRequest))
                        {
                            foundMatches = SearchByField(searchRequest, logEntry, foundMatches);
                        }
                        else if (ShouldSearchByKeyword(searchRequest))
                        {
                            foundMatches = SearchAllFields(searchRequest, logEntry, foundMatches);
                        }

                        AddResultIfMatch(results, logEntry, foundMatches);

                        // Add the ID to File mapping if a match is found
                        if (foundMatches && !_idToFileMapping.ContainsKey(logEntry.Id))
                        {
                            AddToIdToFileMapping(logEntry.Id, logFile); // Add mapping to dictionary and queue
                        }

                        if (results.Count >= _maxResults)
                        {
                            _log.Warn($"Reached a maximum number of search results ({_maxResults}). Returning results.");
                            LogSearchComplete(id);
                            return results;
                        }
                    }
                    catch (JsonException jex)
                    {
                        _log.Error(jex, $"While searching log files - JSON exception thrown");
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, $"While searching log files - exception thrown");
                    }
                }
            }

            _log.Info($"Total files available to search: {logFilesToSearch.Count}".PrefixWith(id));
            _log.Info($"Total files searched: {logFilesToSearch.Count - numberOfFilesOutsideSearchDateRange}".PrefixWith(id));
            _log.Info($"Total files rejected outside the search time range: {numberOfFilesOutsideSearchDateRange}".PrefixWith(id));
            LogSearchComplete(id);

            return results;
        }

        public async Task<LogEntry?> GetEntryByIdAsync(string id)
        {
            _log.Info($"====> Search by ID starting | ID: {id}");

            // Look up the file directly from the ID to file mapping
            if (!_idToFileMapping.ContainsKey(id))
            {
                _log.Warn($"No file found for the specified ID: {id}");
                LogSearchComplete(id);
                return null;
            }

            var logFile = _idToFileMapping[id];
            _log.Info($"Found file to search for ID | Log File: {logFile}".PrefixWith(id));

            var lines = await File.ReadAllLinesAsync(logFile);
            foreach (var line in lines)
            {
                try
                {
                    var logEntry = JsonSerializer.Deserialize<LogEntry>(line);
                    if (logEntry?.Id == id)
                    {
                        _log.Info($"Found log entry with specified ID | Log File: {logFile}".PrefixWith(id));
                        LogSearchComplete(id);
                        return logEntry;
                    }
                }
                catch (JsonException jex)
                {
                    _log.Error(jex, $"While searching log files by ID - JSON exception thrown | ID: {id}".PrefixWith(id));
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"While searching log files by ID - exception thrown | ID: {id}".PrefixWith(id));
                }
            }

            _log.Warn($"Log entry with ID {id} not found.".PrefixWith(id));
            LogSearchComplete(id);
            return null;
        }
        #endregion

        #region Helper Methods
        private void AddToIdToFileMapping(string id, string logFile)
        {
            _idToFileMapping[id] = logFile;
            _idOrderQueue.Enqueue(id);

            // If the dictionary exceeds the maximum size, remove the oldest entry
            if (_idToFileMapping.Count > _maxIdToFileMappingSize)
            {
                var oldestId = _idOrderQueue.Dequeue();
                _idToFileMapping.Remove(oldestId);
            }
        }

        private static void AddResultIfMatch(List<LogSearchResult> results, LogEntry logEntry, bool foundMatches)
        {
            if (foundMatches)
            {
                _log.Trace($"Found match | Number of matches: {results.Count + 1}");
                results.Add(LogSearchResult.From(logEntry));
            }
        }

        private static bool ShouldSearchByKeyword(LogSearchRequest searchRequest)
        {
            return !string.IsNullOrEmpty(searchRequest.Keyword);
        }

        private static bool ShouldSearchByFieldValue(LogSearchRequest searchRequest)
        {
            return !string.IsNullOrEmpty(searchRequest.Field) && !string.IsNullOrEmpty(searchRequest.Value);
        }

        private static bool IsModifiedAfterEndDate(LogSearchRequest searchRequest, DateTime lastModified)
        {
            return searchRequest.EndDate.HasValue && lastModified > searchRequest.EndDate.Value;
        }

        private static bool IsModifiedBeforeStartDate(LogSearchRequest searchRequest, DateTime lastModified)
        {
            return searchRequest.StartDate.HasValue && lastModified < searchRequest.StartDate.Value;
        }

        private static bool TimestampIsAfterEndDate(LogSearchRequest searchRequest, LogEntry logEntry)
        {
            return searchRequest.EndDate.HasValue && logEntry.Timestamp > searchRequest.EndDate.Value;
        }

        private static bool TimestampIsBeforeStartDate(LogSearchRequest searchRequest, LogEntry logEntry)
        {
            return searchRequest.StartDate.HasValue && logEntry.Timestamp < searchRequest.StartDate.Value;
        }

        private static void LogSearchComplete(string id)
        {
            _log.Info($"<==== Search complete | ID: {id}");
        }

        private List<string> GetLogFilesFromDirectories()
        {
            var logFilesToSearch = new List<string>();
            foreach (var logFileDirectory in _logFileDirectories)
            {
                if (Directory.Exists(logFileDirectory))
                {
                    var logFilesThisDir = Directory.GetFiles(logFileDirectory, _logFileSearchPattern, SearchOption.AllDirectories);
                    logFilesToSearch.AddRange(logFilesThisDir);
                }
            }
            return logFilesToSearch;
        }

        private static bool SearchAllFields(LogSearchRequest request, LogEntry logEntry, bool foundMatches)
        {
            bool keywordFound = false;

            foreach (var prop in logEntry.GetType().GetProperties())
            {
                var value = prop.GetValue(logEntry)?.ToString();
                if (!string.IsNullOrEmpty(value) &&
                    value.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase))
                {
                    keywordFound = true;
                    break;
                }
            }

            if (!keywordFound)
                foundMatches = false;
            return foundMatches;
        }

        private static bool SearchByField(LogSearchRequest request, LogEntry logEntry, bool foundMatches)
        {
            var property = logEntry.GetType()
                                   .GetProperty(request.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null ||
                property.GetValue(logEntry)?.ToString()?.Contains(request.Value, StringComparison.OrdinalIgnoreCase) != true)
            {
                foundMatches = false;
            }

            return foundMatches;
        }
        #endregion
    }
}

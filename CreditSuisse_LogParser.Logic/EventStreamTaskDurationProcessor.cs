using System;
using System.Collections.Concurrent;
using CreditSuisse_LogParser.Logic.Model;
using Serilog;

namespace CreditSuisse_LogParser.Logic
{
    public class EventStreamTaskDurationProcessor : IEventStreamTaskDurationProcessor
    {
        private readonly ConcurrentDictionary<string, LogInfoEntry> eventInfoDict;
        private readonly int minDurationForFlagging;

        public EventStreamTaskDurationProcessor(int minDurationForFlagging)
        {
            this.minDurationForFlagging = minDurationForFlagging;
            eventInfoDict = new ConcurrentDictionary<string, LogInfoEntry>();
            Events = new ConcurrentQueue<LogEventInfo>();
        }

        public ConcurrentQueue<LogEventInfo> Events { get; set; }

        public void Visit(LogEvent logEntry)
        {
            if (eventInfoDict.TryGetValue(logEntry.Id, out var logInfoEntry))
            {
                if (UpdateInternalLogEntryIndex(logEntry, logInfoEntry))
                {
                    return;
                }
            }

            // Case when new event arrives - log it so later the duration can be calculated.
            Log.Debug("Found new event: {logEntry}", logEntry);
            AddInternalLogEntryIndex(logEntry);
        }

        private void AddInternalLogEntryIndex(LogEvent logEntry)
        {
            eventInfoDict.TryAdd(logEntry.Id, new LogInfoEntry
            {
                Start = logEntry.TimeStamp
            });
        }

        private bool UpdateInternalLogEntryIndex(LogEvent logEntry, LogInfoEntry logInfoEntry)
        {
// If an event of given Id already arrived - calculate the duration and enqueue for further processing.
            lock (logInfoEntry)
            {
                // This condition ensures that the log still exists (it wasn't removed by concurrent task)
                if (!eventInfoDict.TryGetValue(logEntry.Id, out _))
                {
                    return false;
                }

                var duration = Math.Abs(logEntry.TimeStamp - eventInfoDict[logEntry.Id].Start);
                Log.Debug("Found matching event: {logEntry}", logEntry);

                Events.Enqueue(new LogEventInfo
                {
                    Duration = duration,
                    Id = logEntry.Id,
                    Host = logEntry.Host,
                    Type = logEntry.Type,
                    Alert = duration > minDurationForFlagging
                });

                eventInfoDict.TryRemove(logEntry.Id, out _);

                return true;
            }
        }

        internal class LogInfoEntry
        {
            public long Start { get; set; }
        }
    }
}
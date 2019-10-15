using System;
using System.Collections.Concurrent;
using CreditSuisse_LogParser.Logic.Model;

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
                lock (logInfoEntry)
                {
                    if (eventInfoDict.TryGetValue(logEntry.Id, out _))
                    {
                        var duration = Math.Abs(logEntry.TimeStamp - eventInfoDict[logEntry.Id].Start);

                        Events.Enqueue(new LogEventInfo
                        {
                            Duration = duration,
                            Id = logEntry.Id,
                            Host = logEntry.Host,
                            Type = logEntry.Type,
                            Alert = duration > minDurationForFlagging
                        });

                        eventInfoDict.TryRemove(logEntry.Id, out _);
                        return;
                    }
                }
            }

            eventInfoDict.TryAdd(logEntry.Id, new LogInfoEntry
            {
                Start = logEntry.TimeStamp
            });
        }

        internal class LogInfoEntry
        {
            public long Start { get; set; }
        }
    }
}
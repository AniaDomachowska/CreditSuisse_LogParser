using System.Collections.Concurrent;
using CreditSuisse_LogParser.Logic.Model;

namespace CreditSuisse_LogParser.Logic
{
    public interface IEventStreamTaskDurationProcessor
    {
        ConcurrentQueue<LogEventInfo> Events { get; set; }
        void Visit(LogEvent logEntry);
    }
}
using System;

namespace CreditSuisse_LogParser.Logic.Model
{
    public class LogEvent
    {
        public string Id { get; set; }
        public LogEventStateEnum State { get; set; }
        public long TimeStamp { get; set; }
        public string Host { get; set; }
        public string Type { get; set; }
    }

    public enum LogEventStateEnum
    {
        STARTED,
        FINISHED
    }

    public class LogEventMilliseconds
    {
        public string Id { get; set; }
        public string State { get; set; }
        public int TimeStamp { get; set; }
        public string Host { get; set; }
        public string Type { get; set; }
    }
}
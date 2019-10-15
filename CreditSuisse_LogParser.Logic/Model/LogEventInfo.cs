namespace CreditSuisse_LogParser.Logic.Model
{
    public class LogEventInfo
    {
        public string Id { get; set; }
        public long Duration { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public bool Alert { get; set; }
    }
}

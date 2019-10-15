using CreditSuisse_LogParser.Logic;
using CreditSuisse_LogParser.Logic.Model;
using Xunit;

namespace CreditSuisse_LogParser.Tests
{
    public class EventLoggerTests
    {
        [Fact]
        public void ForLogInfo_LogsToDatabase()
        {
            var testee = new EventLogger();

            testee.Log(new LogEventInfo
            {
                Id = "12345",
                Host = "Host name",
                Alert = true,
                Type = "Event type",
                Duration = 4
            });
        }
    }
}
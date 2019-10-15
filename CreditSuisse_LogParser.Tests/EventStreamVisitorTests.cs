using System;
using CreditSuisse_LogParser.Logic;
using CreditSuisse_LogParser.Logic.Model;
using FluentAssertions;
using Xunit;

namespace CreditSuisse_LogParser.Tests
{
    public class EventStreamVisitorTests
    {
        [Fact]
        public void Visit_For_One_Short_Event_Produces_Event_Info_Not_Flagged()
        {
            var testee = CreateSut();
            var logEntry = new LogEvent()
            {
                Id = "123abc",
                Host = "testHost",
                Type = "type",
                State = "STARTED",
                TimeStamp = 10000
            };

            testee.Visit(logEntry);

            logEntry.TimeStamp += 3;

            testee.Visit(logEntry);

            testee.Events.Should().ContainEquivalentOf(new LogEventInfo()
            {
                Id = "123abc",
                Type = logEntry.Type,
                Host = logEntry.Host,
                Alert = false,
                Duration = 3
            });
        }

        [Fact]
        public void Visit_For_One_Long_Event_Produces_Event_Info_Flagged()
        {
            var testee = CreateSut();
            var logEntry = new LogEvent()
            {
                Id = "123abc",
                Host = "testHost",
                Type = "type",
                State = "STARTED",
                TimeStamp = 10000
            };

            testee.Visit(logEntry);

            const int duration = 13;
            logEntry.TimeStamp += duration;

            testee.Visit(logEntry);

            testee.Events.Should().ContainEquivalentOf(new LogEventInfo()
            {
                Id = "123abc",
                Type = logEntry.Type,
                Host = logEntry.Host,
                Alert = true,
                Duration = duration
            });
        }

        private static EventStreamTaskDurationProcessor CreateSut()
        {
            var testee = new EventStreamTaskDurationProcessor(6);
            return testee;
        }
    }
}
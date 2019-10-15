using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using CreditSuisse_LogParser.Logic;
using CreditSuisse_LogParser.Logic.Model;
using FluentAssertions;
using Xunit;

namespace CreditSuisse_LogParser.Tests
{
    public class EventStreamTaskDurationProcessorTests
    {
        private static EventStreamTaskDurationProcessor CreateSut()
        {
            return new EventStreamTaskDurationProcessor(6);
        }

        [Fact]
        public void Visit_For_BulkLogs_Visits_All_Of_Them()
        {
            // Arrange
            var sut = CreateSut();
            var logIds = new[] {"abcdefg", "hijklmnl", "dfasfasf", "3243243d"};
            var logEvents = PrepareLogEventList(logIds);

            // Act
            foreach (var logEntry in logEvents)
            {
                sut.Visit(logEntry);
            }

            // Assert
            sut.Events.Should().HaveCount(logIds.Length);
            sut.Events.Should().Contain(
                element =>
                    logIds.Any(id => element.Id == id));
        }

        private static IEnumerable<LogEvent> PrepareLogEventList(string[] logIds)
        {
            var fixture = new Fixture().Build<LogEvent>()
                .WithAutoProperties();


            var logEvents = logIds.Select(element =>
            {
                return fixture
                    .With(log => log.Id, element)
                    .Create();
            }).Union(logIds.Select(element =>
            {
                return fixture
                    .With(log => log.Id, element)
                    .Create();
            }));
            return logEvents;
        }

        [Fact]
        public void Visit_For_One_Long_Event_Produces_Event_Info_Flagged()
        {
            // Arrange
            var sut = CreateSut();
            var logEntry = new LogEvent
            {
                Id = "123abc",
                Host = "testHost",
                Type = "type",
                State = "STARTED",
                TimeStamp = 10000
            };

            // Act
            sut.Visit(logEntry);

            // Assert
            const int duration = 13;
            logEntry.TimeStamp += duration;

            sut.Visit(logEntry);

            sut.Events.Should().ContainEquivalentOf(new LogEventInfo
            {
                Id = "123abc",
                Type = logEntry.Type,
                Host = logEntry.Host,
                Alert = true,
                Duration = duration
            });
        }

        [Fact]
        public void Visit_For_One_Short_Event_Produces_Event_Info_Not_Flagged()
        {
            // Arrange
            var sut = CreateSut();
            var logEntry = new LogEvent
            {
                Id = "123abc",
                Host = "testHost",
                Type = "type",
                State = "STARTED",
                TimeStamp = 10000
            };

            // Act
            sut.Visit(logEntry);

            // Assert
            logEntry.TimeStamp += 3;

            sut.Visit(logEntry);

            sut.Events.Should().ContainEquivalentOf(new LogEventInfo
            {
                Id = "123abc",
                Type = logEntry.Type,
                Host = logEntry.Host,
                Alert = false,
                Duration = 3
            });
        }
    }
}
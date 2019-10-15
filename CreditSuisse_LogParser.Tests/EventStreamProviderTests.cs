using System;
using System.IO;
using System.Threading.Tasks;
using CreditSuisse_LogParser.Logic;
using CreditSuisse_LogParser.Logic.Model;
using FluentAssertions;
using Xunit;

namespace CreditSuisse_LogParser.Tests
{
    public class EventStreamProviderTests
    {
        [Fact]
        public async Task ForCommonLog_ReadsItFromFile()
        {
            // Arrange
            var tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName,
                @"{""id"":""scsmbstgrb"", ""state"":""STARTED"", ""timestamp"": 1491377495213}");

            var sut = new EventStreamProvider();

            // Act
            await sut.Read(tempFileName);

            // Assert
            sut.Events.Should().ContainEquivalentOf(new LogEvent
            {
                Id = "scsmbstgrb",
                TimeStamp = 1491377495213,
                Host = null,
                Type = null,
                State = "STARTED"
            });

            File.Delete(tempFileName);
        }
    }
}
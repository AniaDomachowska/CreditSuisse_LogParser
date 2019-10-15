using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using CreditSuisse_LogParser.Logic.Model;
using Newtonsoft.Json;

namespace CreditSuisse_LogParser.Logic
{
    public class EventStreamProvider : IEventStreamProvider
    {
        public EventStreamProvider()
        {
            Events = new ConcurrentQueue<LogEvent>();
        }

        public ConcurrentQueue<LogEvent> Events { get; set; }

        public async Task Read(string logFileName)
        {
            using (var fileReader = File.OpenText(logFileName))
            {
                string line;
                while ((line = await fileReader.ReadLineAsync()) != null)
                {
                    var logEntry = JsonConvert.DeserializeObject<LogEvent>(line);
                    Events.Enqueue(logEntry);
                }
            }
        }
    }
}
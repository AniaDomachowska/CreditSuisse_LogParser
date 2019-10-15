using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using CreditSuisse_LogParser.Logic.Model;
using Newtonsoft.Json;
using Serilog;

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
            if (!File.Exists(logFileName))
            {
                Log.Error("File does not exist {logFileName}", logFileName);
                throw new ArgumentException($"File {logFileName} does not exist");
            }

            using (var fileReader = File.OpenText(logFileName))
            {
                Log.Information("Started reading log file: {logFileName}", logFileName);

                string line;
                while ((line = await fileReader.ReadLineAsync()) != null)
                {
                    // TODO: json deserializing can be done faster by parsing the string manually.
                    var logEntry = JsonConvert.DeserializeObject<LogEvent>(line);
                    Events.Enqueue(logEntry);
                }

                Log.Information("Ended reading log file: {logFileName}", logFileName);
            }
        }
    }
}
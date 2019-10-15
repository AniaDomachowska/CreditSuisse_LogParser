using System;
using System.Threading.Tasks;
using CreditSuisse_LogParser.Logic.Model;

namespace CreditSuisse_LogParser.Logic
{
    public class EventLoader
    {
        private readonly IEventLogger eventLogger;
        private readonly IEventStreamProvider eventStreamProvider;
        private readonly IEventStreamTaskDurationProcessor eventStreamTaskDurationProcessor;

        public EventLoader(
            IEventLogger eventLogger,
            IEventStreamProvider eventStreamProvider,
            IEventStreamTaskDurationProcessor eventStreamTaskDurationProcessor)
        {
            this.eventLogger = eventLogger;
            this.eventStreamProvider = eventStreamProvider;
            this.eventStreamTaskDurationProcessor = eventStreamTaskDurationProcessor;
        }

        public void Load(string filePath)
        {
            var readLogTask = eventStreamProvider.Read(filePath);

            var processTask = Task.Run(() => ProcessEvents(readLogTask));
            var logTask = Task.Run(() => LogEvents(processTask));

            Task.WaitAll(processTask, logTask);
        }

        private void LogEvents(IAsyncResult parentTask)
        {
            LogEventInfo logEventInfo = null;
            while (!parentTask.IsCompleted || eventStreamTaskDurationProcessor.Events.TryDequeue(out logEventInfo))
            {
                if (logEventInfo != null)
                {
                    eventLogger.Log(logEventInfo);
                }
            }
        }

        private void ProcessEvents(IAsyncResult parentTask)
        {
            LogEvent logEvent = null;
            while (!parentTask.IsCompleted || eventStreamProvider.Events.TryDequeue(out logEvent))
            {
                if (logEvent != null)
                {
                    eventStreamTaskDurationProcessor.Visit(logEvent);
                }
            }
        }
    }
}
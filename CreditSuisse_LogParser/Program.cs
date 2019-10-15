using System;
using System.IO;
using CreditSuisse_LogParser.Logic;

namespace CreditSuisse_LogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide file name");
                return;
            }

            const string dbName = @"events.db";
            var dbFolder = AppDomain.CurrentDomain.BaseDirectory;

            var dbPath = Path.Combine(dbFolder, dbName);
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }

            using (var eventLogger = new EventLogger(dbFolder, dbName))
            {
                var eventLoader = new EventLoader(
                    eventLogger,
                    new EventStreamProvider(),
                    new EventStreamTaskDurationProcessor(6));

                var filePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, args[0]);
                eventLoader.Load(filePath);

                Console.WriteLine("Finished importing log files. Total imported: {0}", eventLogger.TotalLogged);
            }
        }
    }
}

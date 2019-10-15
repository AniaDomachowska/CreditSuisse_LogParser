using System;
using System.IO;
using System.Threading.Tasks;
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

            using (var eventLogger = new EventLogger())
            {
                var eventLoader = new EventLoader(
                    eventLogger,
                    new EventStreamProvider(),
                    new EventStreamTaskDurationProcessor(6));

                var filePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, args[0]);
                eventLoader.Load(filePath);

            }
        }
    }
}

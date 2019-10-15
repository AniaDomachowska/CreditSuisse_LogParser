using System.Collections.Concurrent;
using System.Threading.Tasks;
using CreditSuisse_LogParser.Logic.Model;

namespace CreditSuisse_LogParser.Logic
{
    public interface IEventStreamProvider
    {
        ConcurrentQueue<LogEvent> Events { get; set; }
        Task Read(string logFileName);
    }
}
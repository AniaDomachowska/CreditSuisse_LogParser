using System;
using CreditSuisse_LogParser.Logic.Model;

namespace CreditSuisse_LogParser.Logic
{
    public interface IEventLogger : IDisposable
    {
        void Log(LogEventInfo logEventInfo);
    }
}
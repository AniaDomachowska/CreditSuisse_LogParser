using System;
using System.IO;
using CreditSuisse_LogParser.Logic.Model;
using LiteDB;

namespace CreditSuisse_LogParser.Logic
{
    public class EventLogger : IEventLogger
    {
        private readonly LiteDatabase database;

        public EventLogger(string dbFolder = null, string dbName = @"events.db")
        {
            var databaseFile = Path.Join(dbFolder ?? AppDomain.CurrentDomain.BaseDirectory, dbName);

            database = new LiteDatabase($"Filename={databaseFile}");
        }


        public void Dispose()
        {
            database.Commit();
            database.Dispose();
        }

        public void Log(LogEventInfo logEventInfo)
        {
            var col = database.GetCollection<LogEventInfo>("logEventInfos");
            col.Insert(logEventInfo);
            Serilog.Log.Debug("Logged event info: {logEventInfo}", logEventInfo);
        }
    }

    public interface IRepository
    {
    }
}
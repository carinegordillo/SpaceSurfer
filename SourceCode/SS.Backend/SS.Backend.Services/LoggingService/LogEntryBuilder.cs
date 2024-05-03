using System;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.LoggingService
{
    public class LogEntryBuilder
    {
        private readonly LogEntry _logEntry;

        public LogEntryBuilder()
        {
            _logEntry = new LogEntry
            {
                timestamp = DateTime.UtcNow // Default timestamp
            };
        }

        public LogEntryBuilder Info()
        {
            _logEntry.level = "Info";
            return this;
        }

        public LogEntryBuilder Debug()
        {
            _logEntry.level = "Debug";
            return this;
        }

        public LogEntryBuilder Warning()
        {
            _logEntry.level = "Warning";
            return this;
        }

        public LogEntryBuilder Error()
        {
            _logEntry.level = "Error";
            return this;
        }

        public LogEntryBuilder Business()
        {
            _logEntry.category = "Business";
            return this;
        }

        public LogEntryBuilder DataStore()
        {
            _logEntry.category = "Data Store";
            return this;
        }

        public LogEntryBuilder Description(string description)
        {
            _logEntry.description = description;
            return this;
        }

        public LogEntryBuilder User(string username)
        {
            _logEntry.username = username;
            return this;
        }

        public LogEntry Build()
        {
            return _logEntry;
        }
    }
}

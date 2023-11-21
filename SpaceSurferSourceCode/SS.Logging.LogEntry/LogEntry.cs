namespace SS.Logging.LogEntry
{
    public class LogEntry
    {
        public DateTime timestamp { get; set; }
        public LogLevel level { get; set; }
        public string username { get; set; }
        public Category category { get; set; }
        public string description { get; set; }

        public LogEntry()
        {
            timestamp = DateTime.UtcNow;
        }

    }

    public enum LogLevel
    {
        Info, Debug, Warning, Error
    }

    public enum Category
    {
        View, Business, Server, Data, DataStore
    }
}
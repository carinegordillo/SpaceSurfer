 
 namespace SS.SharedNamespace{
 public class Response
    {
        public bool HasError { get; set; }

        public string? ErrorMessage { get; set; }

        public int Log_ID { get; set; }
        public LogEntry LogEntry { get; set; }

        public List<LogEntry> LogEntries { get; set; } = new List<LogEntry>();
    }
 }
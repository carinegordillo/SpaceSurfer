namespace SS.Backend.SharedNamespace
{
    public class LogEntry
    {
        public DateTime timestamp { get; set; }
        public string level { get; set; }
        public string? username { get; set; }
        public string category { get; set; }
        public string description { get; set; }


        public int getDescriptionLength
        {
            get
            {
                return description?.Length ?? 0;
            }
        }

    }
}

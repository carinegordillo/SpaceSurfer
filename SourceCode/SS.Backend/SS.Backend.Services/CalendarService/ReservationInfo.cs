namespace SS.Backend.Services.CalendarService
{
    public class ReservationInfo
    {
        public string filePath { get; set; } = string.Empty;
        public string eventName { get; set; } = string.Empty;
        public DateTime? start { get; set; } = null;
        public DateTime? end { get; set; } = null;
        public string description { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
    }
}
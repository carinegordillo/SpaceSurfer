using System.Net.Sockets;

namespace SS.Backend.SharedNamespace
{
    public class CompanyInfoWithID
    {
        public int CompanyID { get; set; } 
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public TimeSpan OpeningHours { get; set; }
        public TimeSpan ClosingHours { get; set; }
        public string? DaysOpen { get; set; }
        public int? CompanyType { get; set; }
    }
}
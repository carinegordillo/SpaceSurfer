using System.Net.Sockets;

namespace SS.Backend.SharedNamespace
{
    public interface ICompanyInfo
    {
        public string? companyName {get; set;}
        public string? address {get;set;}
        public string? openingHours {get;set;}
        public string? closingHours {get;set;}
        public string? daysOpen{get;set;}
    }
}
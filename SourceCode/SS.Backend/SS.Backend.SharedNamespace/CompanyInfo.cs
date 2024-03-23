namespace SS.Backend.SharedNamespace
{
    public class CompanyInfo : ICompanyInfo
    {
        // Only for company/facility managers: 
        public string? companyName {get; set;}
        public string? address {get;set;}
        public string? openingHours {get;set;}
        public string? closingHours {get;set;}
        public string? daysOpen{get;set;}

    }
}
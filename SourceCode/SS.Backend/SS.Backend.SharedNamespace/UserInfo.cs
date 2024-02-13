namespace SS.Backend.SharedNamespace
{
    public class UserInfo : IUserInfo
    {
        public string username { get; set; }
        public DateTime dob { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int role { get; set; }
        public string status { get; set; }
        public string backupEmail { get; set; }


        // Only for company/facility managers: 
        public string companyName {get; set;}
        public string address {get;set;}
        public string openingHours {get;set;}
        public string closingHours {get;set;}
        public string daysOpen{get;set;}
    }
}
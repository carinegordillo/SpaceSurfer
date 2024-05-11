namespace SS.Backend.UserManagement
{
    public class UserRequestModel
    {
        public int RequestId { get; set;}
        public string UserHash { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string RequestType { get; set; }
        public DateTime? ResolveDate { get; set; }
        public string? AdditionalInformation { get; set; }
        public string? UserName { get; set; }
    }
}
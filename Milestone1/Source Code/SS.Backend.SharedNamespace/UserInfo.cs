namespace SS.Backend.SharedNamespace
{
    public class UserInfo : IUserInfo
    {
        public string username { get; set; }
        public DateTime dob { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        // public string hashedUser {get; set;}

    }
}
namespace SS.Backend.SharedNamespace
{
    public interface IUserInfo
    {
        public string username { get; set; }
        public DateTime dob { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int role { get; set; }
        public string status { get; set; }

        public string backupEmail { get; set; }
        // public string hashedUser {get; set;}
    }
}

// namespace SS.Backend.SharedNamespace
// {
//     public interface UserInfo
//     {
//         public string tableName {get;}
//         public (string Type, string Value) username { get; set; }
//         public string firstname { get; set; }
//         public string lastname { get; set; }
//         public string hashedUser {get; set;}

//     }
// }
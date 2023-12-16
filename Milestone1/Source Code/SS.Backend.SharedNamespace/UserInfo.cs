namespace SS.Backend.SharedNamespace
{
    public class UserInfo
    {
        public string email { get; set; }
        public DateTime dob { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        

        //should we have hashed user in this object or separate?
        public string hashedUser {get; set;}

        //use this to determine the age?
        // public int getDescriptionLength
        // {
        //     get
        //     {
        //         return description?.Length ?? 0;
        //     }
        // }

    }
}
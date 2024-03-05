namespace SS.Backend.UserManagement
{

    public class UserProfileModel
    {
        public string HashedUsername { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BackupEmail { get; set; }
        public int? AppRole { get; set; }
    }
}
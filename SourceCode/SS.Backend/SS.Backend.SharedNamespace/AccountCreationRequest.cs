namespace SS.Backend.SharedNamespace
{
    public class AccountCreationRequest
    {
        public UserInfo? UserInfo { get; set; }
        public CompanyInfo? CompanyInfo { get; set; }
        public string? manager_hashedUsername { get; set; }

    }
}
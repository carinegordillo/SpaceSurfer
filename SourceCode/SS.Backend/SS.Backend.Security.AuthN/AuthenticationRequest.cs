namespace SS.Backend.Security.AuthN
{
    public class AuthenticationRequest
    {
        public string UserIdentity { get; set; } = string.Empty;
        public string Proof { get; set; } = string.Empty;
    }
}

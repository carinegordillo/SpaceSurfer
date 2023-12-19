namespace SS.Backend.Security
{
    public class AuthenticationRequest
    {
        public string UserIdentity { get; set; } = string.Empty;
        public string Proof { get; set; } = string.Empty;
    }
}
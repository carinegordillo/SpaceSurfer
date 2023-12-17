namespace SS.Backend.Security.AuthN
{
    public class SSPrincipal
    {
        public string UserIdentity { get; set; } = string.Empty;
        //public IDictionary<string, string> Claims { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}

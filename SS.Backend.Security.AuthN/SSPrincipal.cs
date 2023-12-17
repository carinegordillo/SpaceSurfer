namespace SS.Backend.Security.AuthN
{
    public class SSPrincipal
    {
        public string UserIdentity { get; set; } = string.Empty;
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
        //public string Role { get; set; } = string.Empty;
    }
}

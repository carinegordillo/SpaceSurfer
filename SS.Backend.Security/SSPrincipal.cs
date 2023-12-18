namespace SS.Backend.Security
{

    public class SSPrincipal
    {
        public SSPrincipal()
        {
            UserIdentity = null;
            Claims = new Dictionary<string, string>();
        }

        public string UserIdentity { get; set; } = string.Empty;
        public IDictionary<string, string> Claims { get; set; }
    }
}

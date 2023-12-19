namespace SS.Backend.SharedNamespace
{
    public class Credential
    {
        public string user { get; set; }
        public string pass { get; set; }

        public Credential(string _user, string _pass)
        {
            user = _user;
            pass = _pass;
        }

        public static Credential CreateSAUser()
        {
            return new Credential("sa", "kalynn");
        }
        public static Credential CreateGenUser()
        {
            return new Credential("SS.GenUser", "kalynn");
        }

    }
}
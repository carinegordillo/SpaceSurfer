namespace SS.Backend.ConsoleApp.Model
{
    //better than just passing in the user and proof into authenticator because if things change, you would only need to alter the model, not the individual methods
    public class AuthenticationRequest
    {
        public string UserIdentity { get; set; } = string.Empty;
        public string Proof { get; set; } = string.Empty;
    }
}

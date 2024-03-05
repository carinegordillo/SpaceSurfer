namespace SS.Backend.ConsoleApp.Models
{
    public class AuthenticationResponse
    {
        public bool HasError => Principal is null ? true : false;
        //public bool CanRetry {  get; set; }
        public AppPrincipal? Principal { get; set; }
    }
}

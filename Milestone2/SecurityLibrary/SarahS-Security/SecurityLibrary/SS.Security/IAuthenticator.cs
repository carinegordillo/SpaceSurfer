
namespace SS.Security
{
    public interface IAuthenticator
    {
        SSPrincipal Authenticate(AuthenticationRequest authRequest);

    }
}
using SS.Backend.SharedNamespace;

namespace SS.Backend.Security
{
    public interface IAuthenticator
    {
        Task<(SSPrincipal principal, Response res)> Authenticate(AuthenticationRequest authRequest);

    }
}

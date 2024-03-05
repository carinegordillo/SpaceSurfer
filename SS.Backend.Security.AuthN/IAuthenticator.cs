using SS.Backend.SharedNamespace;

namespace SS.Backend.Security.AuthN
{
    public interface IAuthenticator
    {
        Task<(SSPrincipal principal, Response res)> Authenticate(AuthenticationRequest authRequest);

    }
}

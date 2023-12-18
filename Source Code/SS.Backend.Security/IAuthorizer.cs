// interface

namespace SS.Backend.Security;
public interface IAuthorizer
{
    Task<bool> IsAuthorize (SSPrincipal currentPrincipal, IDictionary<string, string> claims);
}
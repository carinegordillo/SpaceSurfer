// interface

namespace SS.Backend.Security;
public interface IAuthorizer
{
    bool IsAuthorize(SSPrincipal currentPrincipal, IDictionary<string, string> claims);
}
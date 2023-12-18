// interface

namespace SS.Security;
public interface IAuthorizer
{
    bool IsAuthorize(SSPrincipal currentPrincipal, IDictionary<string, string> claims);
}
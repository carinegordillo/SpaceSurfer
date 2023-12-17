// interface

namespace SS.SecurityLibrary;
public interface IAuthorizer
{
    bool IsAuthorize(SSPrincipal currentPrincipal, IDictionary<string, string> claims);
}
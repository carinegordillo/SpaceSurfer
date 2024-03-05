using SS.Backend.ConsoleApp.Models;

namespace SS.Backend.ConsoleApp.Contracts
{
    public interface IAuthorizer
    {
        bool IsAuthorize(AppPrincipal currentPrincipal, IDictionary<string, string> requiredClaims); //securityContext can be an object, for this particular user, i want to know it has the securityContext
    }
}

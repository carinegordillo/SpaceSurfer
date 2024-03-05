using SS.Backend.ConsoleApp.Model;
using SS.Backend.ConsoleApp.Models;

namespace SS.Backend.ConsoleApp.Contracts
{
    public interface IAuthenticator
    {
        // ValueTuple (specifies the identifiers) to return back two values, returns back an object that returns both entities --> (string user, string roleName)
        AppPrincipal Authenticate(AuthenticationRequest authRequest); //userID and proof, should spit out what authorizer needs, could be string (could be the role of the user)

    }

    //Identity --> who you are (AuthN identifies you as an identity, returns back you in the context of an application)
    //Principal --> who you are within a context
}

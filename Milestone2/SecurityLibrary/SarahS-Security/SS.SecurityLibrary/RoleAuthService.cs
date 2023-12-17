namespace SS.SecurityLibrary;

// example code from lecture for UploadService
public class RoleAuthService : IAuthenticator, IAuthorizer
{
    public bool IsAuthorize(SSPrincipal currentPrincipal, IDictionary<string, string> claims)
    {
        if(currentPrincipal.Claims == "Admin")
    }
    
}

// roleAuthService.IsAuthorize(currPrincipal, requiredRole)

public class UploadService
{
    public void Upload()
    {
        var SSAuthService = new SSAuthService();

        var authRequest = new AuthenticationRequest();
        var SSPrincipal = SSAuthService.Authenticate(authRequest);

        if(SSAuthService.IsAuthorize(SSPrincipal))
        {
            // do something
        }
    }
}
namespace SS.SecurityLibrary;

// example code from lecture for UploadService
public class RoleAuthService : IAuthenticator, IAuthorizer
{
    public bool IsAuthorize(ClaimsPrincipal currentPrincipal, IDictionary<string, string> claims)
    {
        if(currentPrincipal.Claims == "Admin")
    }
    
}

// roleAuthService.IsAuthorize(currPrincipal, requiredRole)

private readonly SSAuthService _SSAuthService;
private readonly AuthenticationRequest authRequest;

public UploadService()
{
   _SSAuthService = SSAuthService.Authenticate(authRequest);
    
}
public void Upload(ClaimsPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
{
    if(SSAuthService.IsAuthorize(currentPrincipal, requiredClaims))
    {
        UploadFile();
    }
}

private void UploadFile()
{
    throw new NotImplementedException();
}
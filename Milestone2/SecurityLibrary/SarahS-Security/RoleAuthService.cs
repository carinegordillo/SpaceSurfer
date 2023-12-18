namespace SS.Security;

// example code from lecture for UploadService
public class RoleAuthService : IAuthenticator, IAuthorizer
{
    public bool IsAuthorize(SSPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
    {
        if(currentPrincipal.Claims.ContainsKey("Admin"))
        {
            UploadService.Upload(currentPrincipal, requiredClaims);
        }
    }
}

public class UploadService 
{
    private readonly SSAuthService _SSAuthSerivce;
    private readonly AuthenticationRequest authRequest;

    public UploadService() 
    {
        _SSAuthSerivce = SSAuthService.Authenticate(authRequest);
    }

    public static void Upload(SSPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
    {
        if(SSAuthService.IsAuthorize(currentPrincipal, requiredClaims))
        {
            UploadService.UploadFile();
        }
    }

    private void UploadFile()
    {
        throw new NotImplementedException();
    }
}
using System.Security.Claims;
using System.Security.Principal;

namespace SS.SecurityLibrary;

public class ClaimsPrincipal 
{
	// authN alone
	// Security context -> RoleName
	// ICollection<(string claimName, string claimValue)> Claims {get; set;}
    // claimName: role name -> value: admin

	public string UserIdentity {get;  set;}
	public IDictionary<string, string> Claims {get; set;}
	public string SecurityContext {get;  set;} 

    public bool HasClaim(string type, string value)
    {
		// UserIdentity as ClaimsIdentity type
        var claimsIdentity = UserIdentity as ClaimsIdentity;

        if (claimsIdentity == null)
        {
            return false;
        }

        var claim = claimsIdentity.FindFirst(claimType);

        return claim != null && claim.Value == claimValue;
    }
}

	//AuthZ: who is the user u need to evaluate (against)

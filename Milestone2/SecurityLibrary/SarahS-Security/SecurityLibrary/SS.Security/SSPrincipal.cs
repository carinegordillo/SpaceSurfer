using System;
using System.Security.Claims;
using System.Security.Principal;

namespace SS.Security
{
    public interface IClaimsPrincipal
    {
        string UserIdentity { get; }
        IDictionary<string, string>? Claims { get; }
        string? SecurityContext { get; }
    }
    public class SSPrincipal : ClaimsPrincipal, IClaimsPrincipal
    {
        public string UserIdentity { get; set; }
        public new IDictionary<string, string>? Claims { get; set; }
        public string? SecurityContext { get; set; }
    }
    public static class ClaimsExtensions
    {
        public static bool HasClaim(this IClaimsPrincipal user, string claimType, string claimValue)
        {
            if (user.Claims == null)
            {
                return false;
            }

            if (user.Claims.TryGetValue(claimType, out var actualClaimValue))
            {
                // Check if the claimValue matches the expected value
                return actualClaimValue == claimValue;
            }

            return false;
        }
    }
}

	//AuthZ: who is the user u need to evaluate (against)
    // authN alone
	// Security context -> RoleName
	// ICollection<(string claimName, string claimValue)> Claims {get; set;}
    // claimName: role name -> value: admin



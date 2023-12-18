using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;

using System.Security.Principal;

namespace SS.Security;
public class SSAuthService : IAuthenticator, IAuthorizer
{
	// authenticate method from lecture
	public SSPrincipal? Authenticate(AuthenticationRequest authRequest)
	{
		// validate arguments “Early Exit” (less cpu, ram = faster system)
		// parameter vs argument (parameter = method signature, argument = actual value)
		#region Validate arguments
		if(authRequest is null)
		{
			throw new ArgumentNullException(nameof(authRequest)); 
		}

		if(String.IsNullOrWhiteSpace(authRequest.UserIdentity))
		{
			throw new ArgumentException($"{nameof(authRequest.UserIdentity)} must be valid");
		}

		if(String.IsNullOrWhiteSpace(authRequest.Proof))
		{
			throw new ArgumentException($"{nameof(authRequest.Proof)} must be valid");
		}
		#endregion

		SSPrincipal SSPrincipal = null;

		try
		{
			// Step1: validate with request
			// Step2: Populate app principal object
			
			var claims = new Dictionary<string, string>();
				
		    SSPrincipal = new SSPrincipal()
			{
				UserIdentity = authRequest.UserIdentity,
				Claims = claims
			};
		}

		catch (Exception ex)
		{
			var errorMessage = ex.GetBaseException().Message;
			// logging errorMessage; asynchronous
		}


		return SSPrincipal;
	}

	public bool IsAuthorize(SSPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
	{
		private readonly AuthenticationRequest authRequest;
		Response result = new Response();
		result = Authenticate(authRequest);
		if(authRequest)
		{
			foreach(var claim in requiredClaims)	
			{
				if(!currentPrincipal.Claims.Contains(claim))
				{
					return false;
				}
			}
			return true;
		}
		return false;
		
	}
}


// no AuthZ response
//Dictionary<string, string>() {new (“RoleName”, “Admin”)}
//key - RoleName, value - Admin
// SSAuthService.IsAuthorize(currentPrincipal, new Dictionary<string, string>() {new (Key: “RoleName”,  Value: “Normal”) })

using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;

using System.Security.Principal;

namespace Company.Security;

/*
public class RandomValue
{
	// byte[256] key
	//OTP

	public static byte[] GenerateRandom(int size)
	{
		var rng = RandomNumberGenerator.GetBytes(size);
		
	}
}

public interface IAuthenticator
{
	// purpose: tell sys yes/no authenticated
	(string userID, string roleName) Authenticate(string userID, string proof);
	// RBAC return role and id (value tuple)
}

public interface IAuthorizer
{
	bool IsAuthorize(string userID, string securityContext);
}


if(auth.Authenticate(“”, “”))
{
	if(auth.IsAuthorize())
	{
		// do stuff
	}
}
*/




public class AppAuthService : IAuthenticator, IAuthorizer
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
			throw new ArgumentException($”{nameof(authRequest.UserIdentity)} must be valid”);
		}

		if(String.IsNullOrWhiteSpace(authRequest.Proof))
		{
			throw new ArgumentException($”{nameof(authRequest.Proof)} must be valid”);
		}
		#endregion

		SSPrincipal appPrincipal = null;

		try
		{
			// Step1: validate with request
			// Step2: Populate app principal object
			
			var claims = new Dictionary<string, string>();
				
			appPrincipal = new SSPrincipal()
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


		return appPrincipal;
	}

	public bool IsAuthorize(SSPrincipal currentPrincipal, IDictionary<string, string> predicateClaims)
	{
		
		//Dictionary<string, string>() {new (“RoleName”, “Admin”)}
		//key - RoleName, value - Admin

		foreach(var claim in predicateClaims)	//10
		{
			//90000000 * predicateClaims.Count
			if(!currentPrincipal.Claims.Contains(claim))
			{
				return false;
			}
		}

		return true;
	}
}

// no AuthZ response

// appAuthService.IsAuthorize(currentPrincipal, new Dictionary<string, string>() {new (Key: “RoleName”,  Value: “Normal”) })

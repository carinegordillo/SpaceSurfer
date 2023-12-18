using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;

using System.Security.Principal;

namespace SS.Security;
public class SSAuthService : IAuthenticator, IAuthorizer
{
	// authenticate method from lecture
	
	public SSPrincipal Authenticate(AuthenticationRequest authRequest)
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

		SSPrincipal ssPrincipal = null;

		try
		{
			// Step1: validate with request
			// Step2: Populate app principal object
			
			var claims = new Dictionary<string, string>();
				
		    ssPrincipal = new SSPrincipal()
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

		return ssPrincipal;
	}
	
	/*
	public async Task<(SSPrincipal principal, Response res)> Authenticate(AuthenticationRequest authRequest)
        {
            Response result = new Response();

            #region Validate arguments
            if (authRequest is null)
            {
                throw new ArgumentNullException(nameof(authRequest));
            }

            if (System.String.IsNullOrWhiteSpace(authRequest.UserIdentity))
            {
                throw new ArgumentException($"{nameof(authRequest.UserIdentity)} must be valid");
            }

            if (System.String.IsNullOrWhiteSpace(authRequest.Proof))
            {
                throw new ArgumentException($"{nameof(authRequest.UserIdentity)} must be valid");
            }
            #endregion

            string username = authRequest.UserIdentity;
            string proof = authRequest.Proof;

            try
            {
                // create and execute sql command to read the hashedOTP from the DB
                SqlCommand readCommand = gensql.GenerateReadHashedOTPQuery(authRequest.UserIdentity);
                result = await sqldao.ReadSqlResult(readCommand).ConfigureAwait(false);
                string dbOTP = (string)result.ValuesRead[0][0];
                string dbSalt = (string)result.ValuesRead[0][1];
                DateTime timestamp = (DateTime)result.ValuesRead[0][2];
                TimeSpan timeElapsed = DateTime.UtcNow - timestamp;

                // compare the otp stored in DB with user inputted otp
                string HashedProof = hasher.HashData(proof, dbSalt);
                if (dbOTP == HashedProof)
                {
                    if (timeElapsed.TotalMinutes > 2)
                    {
                        result.HasError = true;
                        result.ErrorMessage = "OTP has expired.";
                        return (null, result);
                    }
                    else
                    {
                        // they match and not expired, so get the roles from the DB for that user
                        SqlCommand readRolesQuery = gensql.GenerateReadRolesQuery(username);
                        result = await sqldao.ReadSqlResult(readRolesQuery).ConfigureAwait(false);
                        if (result.ValuesRead.Count > 0)
                        {
                            string roles = (string)result.ValuesRead[0][0];

                            // populate the principal
                            SSPrincipal principal = new SSPrincipal();
                            principal.UserIdentity = username;
                            principal.Claims.Add("Roles", roles);

                            result.HasError = false;

                            return (principal, result);
                        }
                        else
                        {
                            result.HasError = true;
                            result.ErrorMessage = "No roles found for the user.";
                            return (null, result);
                        }
                    }

                }
                else
                {
                    result.HasError = true;
                    result.ErrorMessage = "Failed to authenticate.";
                    return (null, result);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                return (null, result);
            }

        }
	*/
	public bool IsAuthorize(SSPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
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
}


// no AuthZ response
//Dictionary<string, string>() {new (“RoleName”, “Admin”)}
//key - RoleName, value - Admin
// SSAuthService.IsAuthorize(currentPrincipal, new Dictionary<string, string>() {new (Key: “RoleName”,  Value: “Normal”) })

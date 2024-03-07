/*
using Company.Security;
using SS.Backend.ConsoleApp.Contracts;
using SS.Backend.ConsoleApp.Model;
using SS.Backend.ConsoleApp.Models;

namespace SS.Backend.ConsoleApp
{
    public class AppAuthService : IAuthenticator, IAuthorizer
    {

        public AppPrincipal Authenticate(AuthenticationRequest authRequest)
        {
            #region Validate arguments
            //--> parameter vs argument: parameter is method signature, argument is actual value
            //1. value has to be not null
            if (authRequest is null)
            {
                throw new ArgumentNullException(nameof(authRequest));
            }
            //2. this can't be null
            if (String.IsNullOrWhiteSpace(authRequest.UserIdentity))
            {
                throw new ArgumentException($"{nameof(authRequest.UserIdentity)} must be valid");
            }
            //3. this can't be null
            if (String.IsNullOrWhiteSpace(authRequest.Proof))
            {
                throw new ArgumentException($"{nameof(authRequest.UserIdentity)} must be valid");
            }
            #endregion //have errors thrown here and not below because here you can't recover from a user being stupid, don't bother trying to fix it, have the user fix it

            AppPrincipal appPrincipal = null;
            try //always need a try catch because it protects against failures, makes it more useable
            {

                //step 1: validate auth request



                //step 2: populate appPrincipal object --> only populated if everything checks out
                var claims = new Dictionary<string, string>();

                appPrincipal = new AppPrincipal()
                {
                    UserIdentity = authRequest.UserIdentity,
                    Claims = claims
                };

            }
            catch (Exception ex)
            {
                var errorMessage = ex.GetBaseException().Message; //base exception gets the first error that was triggered
                // logging errorMessage --> async
            }

            return appPrincipal;
        }

        public bool IsAuthorize(AppPrincipal currentPrincipal, IDictionary<string, string> requiredClaims) //principal contains identity WITH permission
        {


            //requiredClaims has this --> Dictionary<string,string>() {new ("RoleName", "Admin"}
            //key - RoleName, value - Admin

            foreach (var claim in requiredClaims)
            {
                if (!currentPrincipal.Claims.Contains(claim))
                {
                    return false;
                }
            }

            return true;


        }
    }
}
*/
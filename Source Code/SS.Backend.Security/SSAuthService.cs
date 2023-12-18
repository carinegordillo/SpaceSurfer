using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Security
{
    public class SSAuthService : IAuthenticator, IAuthorizer
    {
        
        private readonly GenOTP genotp;
        private readonly Hashing hasher;
        private readonly SqlDAO sqldao;

        public SSAuthService(GenOTP genotp, Hashing hasher, SqlDAO sqldao)
        {
            this.genotp = genotp;
            this.hasher = hasher;
            this.sqldao = sqldao;
        }

        /// <summary>
        /// This method sends the OTP back in the system to whereever it was called and also saves the hashed OTP to the data store
        /// </summary>
        /// <param name="authRequest">Request to authenticate, holds UserIdentity and Proof</param>
        /// <returns>OTP in plaintext and the Response object</returns>
        /*
        public async Task<(string otp, Response res)> SendOTP_and_SaveToDB(AuthenticationRequest authRequest)
        {
            var builder = new CustomSqlCommandBuilder();
            Response result = new Response();
            string user = authRequest.UserIdentity;

            try
            {
                // check if user is registered, ie if user has a role
                var selectCommand = builder
                    .BeginSelectAll()
                    .From("UserRoles")
                    .Where($"Username = '{user}'")
                    .Build();
                result = await sqldao.ReadSqlResult(selectCommand).ConfigureAwait(false);
                if (result.ValuesRead == null || result.ValuesRead.Count == 0)
                {
                    result.HasError = true;
                    result.ErrorMessage = "User does not exist.";
                    return (null, result);
                }

                // generate the otp and salt
                string otp = genotp.generateOTP();
                string salt = hasher.GenSalt();

                // hash the otp
                string hashedOTP = hasher.HashData(otp, salt);

                // check if username already tried authenticating before, ie username exists in otp table
                var otpRead = builder
                    .BeginSelectAll()
                    .From("OTP")
                    .Where($"Username = '{user}'")
                    .Build();
                result = await sqldao.ReadSqlResult(otpRead).ConfigureAwait(false);
                if (result.ValuesRead == null || result.ValuesRead.Count == 0)
                {
                    // create and execute the sql command to insert the username, otp, salt, and timestamp to the DB
                    var parameters = new Dictionary<string, object>
                    {
                        { "OTP", hashedOTP },
                        { "Salt", salt },
                        { "Timestamp", DateTime.UtcNow },
                        { "Username", user }
                    };
                    var insertCommand = builder
                        .BeginInsert("OTP")
                        .Columns(parameters.Keys)
                        .Values(parameters.Keys)
                        .AddParameters(parameters)
                        .Build();
                    result = await sqldao.SqlRowsAffected(insertCommand).ConfigureAwait(false);

                    // user and otp is returned so it can be used by the manager to send the otp to the user
                    return (otp, result);
                }
                else
                {
                    var parameters = new Dictionary<string, object>
                    {
                        { "OTP", hashedOTP },
                        { "Salt", salt },
                        { "Timestamp", DateTime.UtcNow },
                        { "Username", user }
                    };
                    var updateCommand = builder
                        .BeginUpdate("OTP")
                        .Set(parameters)
                        .Where("Username = @username")
                        .AddParameters(parameters)
                        .Build();

                    //update the otp and salt in table for that user
                    result = await sqldao.SqlRowsAffected(updateCommand).ConfigureAwait(false);

                    // user and otp is returned so it can be used by the manager to send the otp to the user
                    return (otp, result);
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                return (null, result);
            }
        }

        /// <summary>
        /// This method authenticates a user by validating their OTP. If valid, it populates the claims sent in the SSPrincipal object
        /// </summary>
        /// <param name="authRequest">Request to authenticate, holds UserIdentity and Proof</param>
        /// <returns>SSPrincipal object which contains UserIdentity and Claims as well as the Response object</returns>
        */
        
        public async Task<(SSPrincipal principal, Response res)> Authenticate(AuthenticationRequest authRequest)
        {
            var builder = new CustomSqlCommandBuilder();
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

            string hashedUsername = authRequest.UserIdentity;
            string proof = authRequest.Proof;

            try
            {
                // create and execute sql command to read the hashedOTP from the DB
                var selectCommand = builder
                    .BeginSelectAll()
                    .From("OTP")
                    .Where($"Username = '{hashedUsername}'")
                    .Build();
                result = await sqldao.ReadSqlResult(selectCommand).ConfigureAwait(false);
                string dbOTP = (string)result.ValuesRead[0][1];
                string dbSalt = (string)result.ValuesRead[0][2];
                DateTime timestamp = (DateTime)result.ValuesRead[0][3];
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
                        var readRoles = builder
                            .BeginSelectAll()
                            .From("UserRoles")
                            .Where($"Username = '{hashedUsername}'")
                            .Build();
                        result = await sqldao.ReadSqlResult(readRoles).ConfigureAwait(false);
                        if (result.ValuesRead.Count > 0)
                        {
                            string role = (string)result.ValuesRead[0][1];

                            // populate the principal
                            SSPrincipal principal = new SSPrincipal();
                            principal.UserIdentity = hashedUsername;
                            principal.Claims.Add("Role", role);

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

        /// <summary>
        /// This method authorizes a user by checking if their claims are contained in the required claims
        /// </summary>
        /// <param name="currentPrincipal">The current principal of the user</param>
        /// <param name="requiredClaims">The required claims to check that the user has</param>
        /// <returns>A boolean denoting if the user is authorized</returns>
        public async Task<bool> IsAuthorize(SSPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
        {
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
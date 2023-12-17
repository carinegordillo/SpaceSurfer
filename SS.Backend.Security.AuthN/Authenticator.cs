using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.Security.AuthN
{
    public class Authenticator : IAuthenticator
    {
        private readonly GenOTP genotp;
        private readonly Hashing hasher;
        private readonly GenSql gensql;
        private readonly SqlDAO sqldao;

        public Authenticator(GenOTP genotp, Hashing hasher, GenSql gensql, SqlDAO sqldao)
        {
            this.genotp = genotp;
            this.hasher = hasher;
            this.gensql = gensql;
            this.sqldao = sqldao;
        }
        public async Task<(string username, string otp, Response res)> SendOTP_and_SaveToDB(AuthenticationRequest authRequest)
        {
            Response result = new Response();
            try
            {
                string user = authRequest.UserIdentity;
                string otp = genotp.generateOTP();

                string salt = hasher.GenSalt();

                // hash the otp
                string hashedOTP = hasher.HashData(otp, salt);

                // create and execute the sql command to insert the username, otp, salt, and timestamp to the DB
                SqlCommand insertCommand = gensql.GenerateInsertQuery(user, hashedOTP, salt);
                sqldao.SqlRowsAffected(insertCommand);

                // user and otp is returned so it can be used by the manager to send the otp to the user
                result.HasError = false;
                return (user, otp, result);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                return (null, null, result);
            }
        }
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

                // compare the otp stored in DB with user inputted otp
                string HashedProof = hasher.HashData(proof, dbSalt);
                if (dbOTP == HashedProof)
                {
                    // they match, so get the roles from the DB for that user
                    SqlCommand readRolesQuery = gensql.GenerateReadRolesQuery(username);
                    result = await sqldao.ReadSqlResult(readRolesQuery).ConfigureAwait(false);
                    if (result.ValuesRead.Count > 0)
                    {
                        // Assuming Roles is a varchar(100), modify accordingly if needed
                        string roles = (string)result.ValuesRead[0][0];

                        // populate the principal
                        SSPrincipal principal = new SSPrincipal();
                        principal.UserIdentity = username;
                        principal.Role = roles; // ------------------- THIS IS TEMPORARY, i couldn't figure out the dictionary stuff, so right now i have it as role based and not claims

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
    }
}

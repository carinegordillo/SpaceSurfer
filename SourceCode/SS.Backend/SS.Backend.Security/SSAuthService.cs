using Microsoft.IdentityModel.Tokens;
using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SS.Backend.Security
{
    public class SSAuthService : IAuthenticator, IAuthorizer //add interfaces for the objects
    {
        private readonly GenOTP genotp;
        private readonly Hashing hasher;
        private readonly SqlDAO sqldao;
        private readonly Logger log;

        public SSAuthService(GenOTP genotp, Hashing hasher, SqlDAO sqldao, Logger log)
        {
            this.genotp = genotp;
            this.hasher = hasher;
            this.sqldao = sqldao;
            this.log = log;
        }

        /// <summary>
        /// This method sends the OTP back in the system to whereever it was called and also saves the hashed OTP to the data store
        /// </summary>
        /// <param name="authRequest">Request to authenticate, holds UserIdentity and Proof</param>
        /// <returns>OTP in plaintext and the Response object</returns>
        public async Task<(string otp, Response res)> SendOTP_and_SaveToDB(AuthenticationRequest authRequest)
        {
            var builder = new CustomSqlCommandBuilder();
            Response result = new Response();
            string user = authRequest.UserIdentity;

            var getHash = builder
                .BeginSelectAll()
                .From("userHash")
                .Where($"username = '{user}'")
                .Build();
            result = await sqldao.ReadSqlResult(getHash);
            string user_hash = (string)result.ValuesRead?.Rows[0]?["hashedUsername"];

            try
            {
                // check if user is registered, ie if user has a role
                var selectCommand = builder
                    .BeginSelectAll()
                    .From("userProfile")
                    .Where($"hashedUsername = '{user_hash}'")
                    .Build();
                result = await sqldao.ReadSqlResult(selectCommand);
                if (result.ValuesRead == null || result.ValuesRead.Rows.Count == 0)
                {
                    result.HasError = true;
                    result.ErrorMessage = $"User: '{user}' does not exist.";
                    LogEntry entry = new()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = user_hash,
                        category = "Data Store",
                        description = "Unregistered user tried to authenticate."
                    };
                    await log.SaveData(entry);

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
                    .Where($"Username = '{user_hash}'")
                    .Build();
                result = await sqldao.ReadSqlResult(otpRead);
                if (result.ValuesRead == null || result.ValuesRead.Rows.Count == 0)
                {
                    // create and execute the sql command to insert the username, otp, salt, and timestamp to the DB
                    var parameters = new Dictionary<string, object>
                    {
                        { "OTP", hashedOTP },
                        { "Salt", salt },
                        { "Timestamp", DateTime.UtcNow },
                        { "Username", user_hash }
                    };
                    var insertCommand = builder
                        .BeginInsert("OTP")
                        .Columns(parameters.Keys)
                        .Values(parameters.Keys)
                        .AddParameters(parameters)
                        .Build();
                    result = await sqldao.SqlRowsAffected(insertCommand);

                    // user and otp is returned so it can be used by the manager to send the otp to the user
                    LogEntry entry = new()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = user_hash,
                        category = "Data Store",
                        description = "Successfully sent OTP to manager."
                    };
                    await log.SaveData(entry);

                    return (otp, result);
                }
                else
                {
                    var parameters = new Dictionary<string, object>
                    {
                        { "OTP", hashedOTP },
                        { "Salt", salt },
                        { "Timestamp", DateTime.UtcNow },
                        { "Username", user_hash }
                    };
                    var updateCommand = builder
                        .BeginUpdate("OTP")
                        .Set(parameters)
                        .Where("Username = @username")
                        .AddParameters(parameters)
                        .Build();

                    //update the otp and salt in table for that user
                    result = await sqldao.SqlRowsAffected(updateCommand);

                    // user and otp is returned so it can be used by the manager to send the otp to the user
                    LogEntry entry = new()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = user_hash,
                        category = "Data Store",
                        description = "Successfully sent OTP to manager."
                    };
                    await log.SaveData(entry);

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

            string user = authRequest.UserIdentity;
            string proof = authRequest.Proof;

            var getHash = builder
                .BeginSelectAll()
                .From("userHash")
                .Where($"username = '{user}'")
                .Build();
            result = await sqldao.ReadSqlResult(getHash);
            string user_hash = (string)result.ValuesRead?.Rows[0]?["hashedUsername"];

            try
            {
                // create and execute sql command to read the hashedOTP from the DB
                var selectCommand = builder
                    .BeginSelectAll()
                    .From("OTP")
                    .Where($"Username = '{user_hash}'")
                    .Build();
                result = await sqldao.ReadSqlResult(selectCommand);
                string dbOTP = (string)result.ValuesRead?.Rows[0]?["OTP"];
                string dbSalt = (string)result.ValuesRead?.Rows[0]?["Salt"];
                DateTime timestamp = (DateTime)result.ValuesRead?.Rows[0]?["Timestamp"];
                TimeSpan timeElapsed = DateTime.UtcNow - timestamp;

                // compare the otp stored in DB with user inputted otp
                string HashedProof = hasher.HashData(proof, dbSalt);
                if (dbOTP == HashedProof)
                {
                    if (timeElapsed.TotalMinutes > 2)
                    {
                        result.HasError = true;
                        result.ErrorMessage = "OTP has expired.";
                        LogEntry entry = new()
                        {
                            timestamp = DateTime.UtcNow,
                            level = "Error",
                            username = user_hash,
                            category = "Data Store",
                            description = "User tried to authenticate with an expired OTP."
                        };
                        await log.SaveData(entry);

                        return (null, result);
                    }
                    else
                    {
                        // they match and not expired, so get the roles from the DB for that user
                        var readRoles = builder
                            .BeginSelectAll()
                            .From("userProfile")
                            .Where($"hashedUsername = '{user_hash}'")
                            .Build();
                        result = await sqldao.ReadSqlResult(readRoles);
                        if (result.ValuesRead.Rows.Count > 0)
                        {
                            string role = result.ValuesRead?.Rows[0]?["appRole"].ToString();

                            // populate the principal
                            SSPrincipal principal = new SSPrincipal();
                            principal.UserIdentity = user_hash;
                            principal.Claims.Add("Role", role);

                            result.HasError = false;

                            LogEntry entry = new()
                            {
                                timestamp = DateTime.UtcNow,
                                level = "Info",
                                username = user_hash,
                                category = "Data Store",
                                description = "Successful authentication."
                            };
                            await log.SaveData(entry);

                            return (principal, result);
                        }
                        else
                        {
                            result.HasError = true;
                            result.ErrorMessage = "No roles found for the user.";
                            LogEntry entry = new()
                            {
                                timestamp = DateTime.UtcNow,
                                level = "Error",
                                username = user_hash,
                                category = "Data Store",
                                description = "Failure to authenticate."
                            };
                            await log.SaveData(entry);

                            return (null, result);
                        }
                    }

                }
                else
                {
                    result.HasError = true;
                    result.ErrorMessage = "Failed to authenticate.";
                    LogEntry entry = new()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = user_hash,
                        category = "Data Store",
                        description = "Failure to authenticate."
                    };
                    await log.SaveData(entry);

                    return (null, result);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                LogEntry entry = new()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = user_hash,
                    category = "Data Store",
                    description = "Failure to authenticate."
                };
                await log.SaveData(entry);

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
            if (currentPrincipal?.Claims == null)
            {
                return false; // If user claims are null, authorization fails
            }

            foreach (var requiredClaim in requiredClaims)
            {
                if (currentPrincipal.Claims.TryGetValue(requiredClaim.Key, out var claimValue))
                {
                    // Special handling for roles, assuming they might be stored as comma-separated values
                    if (requiredClaim.Key == "Role")
                    {
                        var roles = claimValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(role => role.Trim())
                                            .ToList();

                        // Check if any of the required roles are present
                        var requiredRoles = requiredClaim.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                            .Select(role => role.Trim());

                        if (!requiredRoles.All(requiredRole => roles.Contains(requiredRole)))
                        {
                            return false; // If any required role is missing, authorization fails
                        }
                    }
                    else if (claimValue != requiredClaim.Value)
                    {
                        return false; // For non-role claims, fail if the claim doesn't match the required value
                    }
                }
                else
                {
                    return false; // Fail if the required claim is missing
                }
            }

            return true; // Passes all checks
            /*
            if (currentPrincipal?.Claims == null)
            {
                // If user claims are null, authorization fails
                return false;
            }

            foreach (var claim in requiredClaims)
            {
                if (!currentPrincipal.Claims.Contains(claim))
                {
                    return false;
                }
            }
            return true;
            */

        }

        public SSPrincipal MapToSSPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                return null;
            }

            var ssPrincipal = new SSPrincipal
            {
                UserIdentity = claimsPrincipal.Identity?.Name,
                Claims = new Dictionary<string, string>()
            };

            foreach (var claim in claimsPrincipal.Claims)
            {
                ssPrincipal.Claims.Add(claim.Type, claim.Value);
            }

            return ssPrincipal;
        }


        public async Task<string> GenerateAccessToken(string username, IDictionary<string, string> roles)
        {
            var builder = new CustomSqlCommandBuilder();
            Response result = new Response();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("g3LQ4A6$h#Z%2&t*BKs@v7GxU9$FqNpDrn");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Iss, "https://spacesurfers.auth.com/"),
                new Claim(JwtRegisteredClaimNames.Aud, "spacesurfers"),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Value));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                //Expires = DateTime.UtcNow.AddHours(1),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GetSubjectFromToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);

            var subject = token.Subject;
            return subject;
        }

        public List<string> GetRolesFromToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);

            var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "role")?.Value;
            return roleClaim != null ? new List<string> { roleClaim } : new List<string>();
        }

        public string GetExpTimeFromToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);

            string expirationTime = token.ValidTo.ToString("yyyy-MM-ddTHH:mm:ssZ");
            return expirationTime;
        }

        public async Task<(string newToken, Response res)> RefreshToken(string username, IDictionary<string, string> roles)
        {
            try
            {
                string newToken = await GenerateAccessToken(username, roles);

                return (newToken, new Response { HasError = false });
            }
            catch (Exception ex)
            {
                return (null, new Response { HasError = true, ErrorMessage = ex.Message });
            }
        }

        public async Task<string> HashToken(string token)
        {
            Response result = new Response();

            try
            {
                var getCmd = builder
                    .BeginSelectAll()
                    .From("TokenSalt")
                    .Build();
                result = await sqldao.ReadSqlResult(getCmd);
                string tokenSalt = (string)result.ValuesRead?.Rows[0]?["Salt"];
                string tokenHash = hasher.HashData(token, tokenSalt);
                return tokenHash;
            }
            catch (Exception ex)
            {
                return (null, new Response { HasError = true, ErrorMessage = ex.Message });
            }
        }

    }
}

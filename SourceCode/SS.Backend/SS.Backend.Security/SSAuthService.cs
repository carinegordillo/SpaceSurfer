using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Security.Claims;
using System.Security;
using System.Text;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
            Response result = new();
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

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                    return (null, result);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
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
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return (null, result);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
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
            Response result = new();

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

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                        return (null, result);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        if (result.ValuesRead.Rows.Count > 0)
                        {
                            string role = result.ValuesRead?.Rows[0]?["appRole"].ToString();

                            // populate the principal
                            SSPrincipal principal = new();
                            principal.UserIdentity = user_hash;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
                            principal.Claims.Add("Role", role);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

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

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                            return (null, result);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                    return (null, result);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
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

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return (null, result);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }

        }

        /// <summary>
        /// This method authorizes a user by checking if their claims are contained in the required claims
        /// </summary>
        /// <param name="currentPrincipal">The current principal of the user</param>
        /// <param name="requiredClaims">The required claims to check that the user has</param>
        /// <returns>A boolean denoting if the user is authorized</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> IsAuthorize(SSPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

        }

        public SSPrincipal? MapToSSPrincipal(ClaimsPrincipal claimsPrincipal)
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

        public string CreateJwt(HttpRequest Request, SSPrincipal principal)
        {
            var header = new JwtHeader();
            var payload = new JwtPayload()
            {
                Iss = Request.Host.Host,
                Sub = principal.UserIdentity,
                Aud = "spacesurfers",
                Iat = DateTime.UtcNow.Ticks,
                Exp = DateTime.UtcNow.AddHours(1).Ticks,
                Claims = principal.Claims
            };

            var serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            var encodedHeader = Base64UrlEncode(JsonSerializer.Serialize(header, serializerOptions));
            var encodedPayload = Base64UrlEncode(JsonSerializer.Serialize(payload, serializerOptions));

            using (var hash = new HMACSHA256(Encoding.UTF8.GetBytes("simple-key")))
            {
                // String to Byte[]
                var signatureInput = $"{encodedHeader}.{encodedPayload}";
                var signatureInputBytes = Encoding.UTF8.GetBytes(signatureInput);

                // Byte[] to String
                var signatureDigestBytes = hash.ComputeHash(signatureInputBytes);
                var encodedSignature = WebEncoders.Base64UrlEncode(signatureDigestBytes);

                var jwt = new Jwt()
                {
                    Header = header,
                    Payload = payload,
                    Signature = encodedSignature
                };

                return jwt.ToJson();
            }
        }

        private string Base64UrlEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return WebEncoders.Base64UrlEncode(bytes);
        }

        public string CreateIdToken(SSPrincipal principal)
        {
            var user = principal.UserIdentity;
            var idt = new IdToken { Username = user };

            return idt.ToJson();
        }

        public SSPrincipal ValidateToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("g3LQ4A6$h#Z%2&t*BKs@v7GxU9$FqNpDrn");
            var signingKey = new SymmetricSecurityKey(key);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                // Use the existing method to map ClaimsPrincipal to SSPrincipal
                var ssPrincipal = MapToSSPrincipal(principal);

#pragma warning disable CS8603 // Possible null reference return.
                return ssPrincipal;
#pragma warning restore CS8603 // Possible null reference return.
            }
            catch (Exception ex)
            {
                // Log or handle the exception if needed
                throw new SecurityTokenException("Token validation failed", ex);
            }
        }


    }
}

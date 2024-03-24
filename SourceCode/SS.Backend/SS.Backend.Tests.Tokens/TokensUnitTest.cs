// using System;
// using System.IdentityModel.Tokens.Jwt;
// using System.Linq;
// using System.Text;
// using System.Diagnostics;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using SS.Backend.Security;
// using SS.Backend.SharedNamespace;
// using SS.Backend.DataAccess;
// using SS.Backend.Services.LoggingService;
// namespace SS.Backend.Tests.Tokens;

// [TestClass]
// public class UnitTest1
// {
//     [TestMethod]
//     public async Task GeneratedToken_Valid()
//     {
//         // Arrange
//         Response result = new Response();
//         var builder = new CustomSqlCommandBuilder();
//         var baseDirectory = AppContext.BaseDirectory;
//         var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//         var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
//         ConfigService configService = new ConfigService(configFilePath);
//         GenOTP genotp = new GenOTP();
//         Hashing hasher = new Hashing();
//         SqlDAO dao = new SqlDAO(configService);
//         SqlLogTarget target = new SqlLogTarget(dao);
//         Logger log = new Logger(target);
//         string jwtSecret = "g3LQ4A6$h#Z%2&t*BKs@v7GxU9$FqNpDrn";
//         SSAuthService auth = new SSAuthService(genotp, hasher, dao, log, jwtSecret);
//         AuthenticationRequest request = new AuthenticationRequest();
//         SSPrincipal principal = new SSPrincipal();
//         Stopwatch timer = new Stopwatch();

//         var parameters = new Dictionary<string, object>
//         {
//             { "hashedUsername", "test@email" },
//             { "firstName", "benny" },
//             { "lastName", "bennington" },
//             { "backupEmail", "backup@email" },
//             { "appRole", "1" }
//         };
//         var insertCommand = builder
//             .BeginInsert("userProfile")
//             .Columns(parameters.Keys)
//             .Values(parameters.Keys)
//             .AddParameters(parameters)
//             .Build();
//         await dao.SqlRowsAffected(insertCommand);

//         var parameters2 = new Dictionary<string, object>
//         {
//             { "OTP", "previousOTP" },
//             { "Salt", "testSalt" },
//             { "Timestamp", "2023-03-20 03:10:21.470" },
//             { "Username", "test@email" }
//         };
//         var insertCommand2 = builder
//             .BeginInsert("OTP")
//             .Columns(parameters2.Keys)
//             .Values(parameters2.Keys)
//             .AddParameters(parameters2)
//             .Build();
//         await dao.SqlRowsAffected(insertCommand2);

//         request.UserIdentity = "test@email";
//         request.Proof = null;

//         (string otp, result) = await auth.SendOTP_and_SaveToDB(request);

//         request.Proof = otp;

//         var currentPrincipal = new SSPrincipal
//         {
//             UserIdentity = request.UserIdentity,
//             Claims = new Dictionary<string, string>
//             {
//                 {"Role", "Admin"}
//             }
//         };
//         //var requiredClaims = new Dictionary<string, string> {{ "Role", "Admin" }};
//         (principal, result) = await auth.Authenticate(request);


//         // Act
//         timer.Start();
//         var tokenString = auth.GenerateAccessToken(currentPrincipal.UserIdentity, currentPrincipal.Claims);
//         var tokenHandler = new JwtSecurityTokenHandler();
//         var token = tokenHandler.ReadJwtToken(tokenString);
//         timer.Stop();

//         // Assert
//         Assert.IsNotNull(token);
//         Assert.AreEqual(currentPrincipal.UserIdentity, token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value);

//         // Check for roles
//         //List<string> tokenRoles = new List<string>();
//         if(currentPrincipal.Claims != null && currentPrincipal.Claims.TryGetValue("Role", out var rolesString))
//         {
//             var tokenRoles = rolesString.Split(',').Select(role => role.Trim()).ToList();

//             var expectedRoles = rolesString.Split(',').Select(r => r.Trim());

//             // Check if tokenRoles contains all expectedRoles
//             Assert.IsTrue(expectedRoles.All(expectedRole => tokenRoles.Contains(expectedRole)), "All roles should be included in the token");
//         }
//         else
//         {
//             Assert.Fail("Claims are null or do not contain 'Role'");
//         }
//         Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);
//     }

// }

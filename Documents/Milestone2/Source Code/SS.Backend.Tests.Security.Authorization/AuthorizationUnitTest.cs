using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Diagnostics;

namespace SS.Backend.Tests.Security.Authorization
{

    [TestClass]
    public class AuthorizationUnitTest
    {

        [TestMethod]
        //IsAuthorize - Success (true)
        public async Task IsAuthorize_PassAsync()
        {
            // Arrange
            Response result = new Response();
            var builder = new CustomSqlCommandBuilder();
            string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
            GenOTP genotp = new GenOTP();
            Hashing hasher = new Hashing();
            SqlDAO dao = new SqlDAO(configService);
            SqlLogTarget target = new SqlLogTarget(dao);
            Logger log = new Logger(target);
            SSAuthService auth = new SSAuthService(genotp, hasher, dao, log);
            AuthenticationRequest request = new AuthenticationRequest();
            SSPrincipal principal = new SSPrincipal();
            Stopwatch timer = new Stopwatch();
            var currentPrincipal = new SSPrincipal
            {
                UserIdentity = "John",
                Claims = new Dictionary<string, string>
            {
                {"Role", "Admin"}
            }
            };

            var requiredClaims = new Dictionary<string, string>
            {
                {"Role", "Admin"}
            };

            // Act
            timer.Start();
            bool res = await auth.IsAuthorize(currentPrincipal, requiredClaims);
            timer.Stop();

            // Assert
            Assert.IsTrue(res, "The user has all the required claims, so IsAuthorize should return true.");
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);
        }

        [TestMethod]
        //IsAuthorize - Failed Required Claims(false)
        public async Task IsAuthorize_FailedRequiredClaimsAsync()
        {
            // Arrange
            Response result = new Response();
            var builder = new CustomSqlCommandBuilder();
            string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
            GenOTP genotp = new GenOTP();
            Hashing hasher = new Hashing();
            SqlDAO dao = new SqlDAO(configService);
            SqlLogTarget target = new SqlLogTarget(dao);
            Logger log = new Logger(target);
            SSAuthService auth = new SSAuthService(genotp, hasher, dao, log);
            AuthenticationRequest request = new AuthenticationRequest();
            SSPrincipal principal = new SSPrincipal();
            Stopwatch timer = new Stopwatch();

            var currentPrincipal = new SSPrincipal
            {
                UserIdentity = "Jane",
                Claims = new Dictionary<string, string>
            {
                {"Role", "GenUser"}
            }
            };

            var requiredClaims = new Dictionary<string, string>
            {
                {"Role", "Admin"}
            };

            // Act
            timer.Start();
            bool res = await auth.IsAuthorize(currentPrincipal, requiredClaims);
            timer.Stop();

            // Assert
            Assert.IsFalse(res, "The user is missing a required claim, so IsAuthorize should return false.");
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);
        }

        [TestMethod]
        //IsAuthorize - Failed Null Claims (false)
        public async Task IsAuthorize_FailedNullClaimsAsync()
        {
            // Arrange
            Response result = new Response();
            var builder = new CustomSqlCommandBuilder();
            string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
            GenOTP genotp = new GenOTP();
            Hashing hasher = new Hashing();
            SqlDAO dao = new SqlDAO(configService);
            SqlLogTarget target = new SqlLogTarget(dao);
            Logger log = new Logger(target);
            SSAuthService auth = new SSAuthService(genotp, hasher, dao, log);
            AuthenticationRequest request = new AuthenticationRequest();
            SSPrincipal principal = new SSPrincipal();
            Stopwatch timer = new Stopwatch();

            var currentPrincipal = new SSPrincipal
            {
                UserIdentity = "Jimmy",
                Claims = null
            };

            var requiredClaims = new Dictionary<string, string>
            {
                {"Role", "Admin"}
            };

            // Act
            timer.Start();
            bool res = await auth.IsAuthorize(currentPrincipal, requiredClaims);
            timer.Stop();

            // Assert
            Assert.IsFalse(res, "The user claims are null, so IsAuthorize should return false.");
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);
        }
    }
}
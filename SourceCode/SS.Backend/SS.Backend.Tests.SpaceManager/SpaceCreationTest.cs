global using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
// using System.Data.SqlClient;
// using SS.Backend.Services.AccountCreationService;
using SS.Backend.SpaceManager;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using SS.Backend.Services.LoggingService;

namespace SS.Backend.Tests.SpaceCreationTest
{
    [TestClass]
    public class SpaceCreationTest
    {
        private SpaceCreation? _spaceCreation;
        private ISpaceManagerDao? _spaceManagerDao; 
        private SqlDAO? _sqlDao;
        private ConfigService? _configService;
        private ILogTarget _logTarget;
        private ILogger _logger;
     

        [TestInitialize]
        public void Setup()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);
            _logger = new Logger(_logTarget);
            _spaceManagerDao = new SpaceManagerDao(_sqlDao);
            _spaceCreation = new SpaceCreation(_spaceManagerDao, _logger);
        }
        private async Task CleanupTestData()
        {
            // var SAUser = Credential.CreateSAUser();
            // var connectionString = string.Format(@"Data Source=localhost\SpaceSurfer;Initial Catalog=SS_Server;User Id={0};Password={1};", SAUser.user, SAUser.pass);
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

            ConfigService configFile = new ConfigService(configFilePath);
            var connectionString = configFile.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string sql = $"DELETE FROM dbo.Logs WHERE [Username] = 'test@email'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test cleanup: {ex}");
            }
        }

        [TestMethod]
        public async Task CreateSpace_Success()
        {
            Stopwatch timer = new Stopwatch();
            var validFloorInfo = new CompanyFloor
            {
                hashedUsername = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ",
                FloorPlanName = "NEW Demo Code",
                FloorPlanImage = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                FloorSpaces = new Dictionary<string, int> { { "newcode1", 2 }, { "newdemo1", 3 }, {"newreview1", 3} },
            };
            timer.Start();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(validFloorInfo);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            timer.Stop();

            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CreateSpace_InvalidHashedUsername()
        {
            Stopwatch timer = new Stopwatch();
            var companyFloor = new CompanyFloor
            {
                hashedUsername = "invalidHashedUsername",
                FloorPlanName = "Test Floor Plan",
                FloorPlanImage = new byte[] { 0x01, 0x02 },
                FloorSpaces = new Dictionary<string, int> { { "S1", 120 } }
            };

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
            Assert.IsTrue(response.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CreateSpace_NullCompanyFloor()
        {
            var companyFloor = new CompanyFloor
            {
                hashedUsername = null,
                FloorPlanName = null,
                FloorPlanImage = null,
                FloorSpaces = null
            };
            
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public async Task CreateSpace_MissingFloorPlanName()
        {
            var companyFloor = new CompanyFloor
            {
                hashedUsername = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ",
                FloorPlanName = null, 
                FloorPlanImage = new byte[] { 0x01, 0x02 },
                FloorSpaces = new Dictionary<string, int> { { "S1", 120 } }
            };
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public async Task CreateSpace_MissingFloorPlanImage()
        {
            var companyFloor = new CompanyFloor
            {
                hashedUsername = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ",
                FloorPlanName = "Test Floor Plan",
                FloorPlanImage = null, 
                FloorSpaces = new Dictionary<string, int> { { "S1", 120 } }
            };

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
            // Adjust the Assert based on your method's expected behavior when the floor plan image is missing
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public async Task CreateSpace_MissingFloorSpaces()
        {
            var companyFloor = new CompanyFloor
            {
                hashedUsername = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ",
                FloorPlanName = "Test Floor Plan",
                FloorPlanImage = new byte[] { 0x01, 0x02 },
                FloorSpaces = null // Intentionally setting this to an empty dictionary
            };

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
            // Adjust the Assert based on your method's expected behavior when floor spaces are missing
            Assert.IsTrue(response.HasError);
        }


    }
}
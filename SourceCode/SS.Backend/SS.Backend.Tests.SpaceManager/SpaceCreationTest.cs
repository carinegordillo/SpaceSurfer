using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
// using System.Data.SqlClient;
// using SS.Backend.Services.AccountCreationService;
using SS.Backend.SpaceManager;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace SS.Backend.Tests.SpaceCreationTest
{
    [TestClass]
    public class SpaceCreationTest
    {
        private SpaceCreation? _spaceCreation;
        private ISpaceManagerDao? _spaceManagerDao; 
        private SqlDAO? _sqlDao;
        private ConfigService? _configService;
        // private SqlCommand? _command;

        [TestInitialize]
        public void Setup()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);
            _spaceManagerDao = new SpaceManagerDao(_sqlDao);
            _spaceCreation = new SpaceCreation(_spaceManagerDao);
            
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


            string validCompanyHash = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";
            var validFloorInfo = new CompanyFloor
            {
                FloorPlanName = "New Floor Plan",
                FloorPlanImage = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                FloorSpaces = new Dictionary<string, int> { { "Okay", 2 }, { "Oh", 3 }, {"No", 3} },
            };
            timer.Start();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(validCompanyHash, validFloorInfo);
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
            string invalidHashedUsername = "invalidHashValue";
            var companyFloor = new CompanyFloor
            {
                FloorPlanName = "Test Floor Plan",
                FloorPlanImage = new byte[] { 0x01, 0x02 },
                FloorSpaces = new Dictionary<string, int> { { "S1", 120 } }
            };

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(invalidHashedUsername, companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
            Assert.IsTrue(response.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
     
        }

        [TestMethod]
        public async Task CreateSpace_NullCompanyFloor()
        {
            string validHashedUsername = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(validHashedUsername, null);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public async Task CreateSpace_MissingFloorPlanName()
        {
            string validHashedUsername = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";
            var companyFloor = new CompanyFloor
            {
                FloorPlanName = null, // Intentionally setting this to null
                FloorPlanImage = new byte[] { 0x01, 0x02 },
                FloorSpaces = new Dictionary<string, int> { { "S1", 120 } }
            };

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(validHashedUsername, companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
            Assert.IsTrue(response.HasError);

        }

        [TestMethod]
        public async Task CreateSpace_MissingFloorPlanImage()
        {
            string validHashedUsername = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";
            var companyFloor = new CompanyFloor
            {
                FloorPlanName = "Test Floor Plan",
                FloorPlanImage = null, // Intentionally setting this to null
                FloorSpaces = new Dictionary<string, int> { { "S1", 120 } }
            };

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(validHashedUsername, companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
            // Adjust the Assert based on your method's expected behavior when the floor plan image is missing
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public async Task CreateSpace_MissingFloorSpaces()
        {
            string validHashedUsername = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";
            var companyFloor = new CompanyFloor
            {
                FloorPlanName = "Test Floor Plan",
                FloorPlanImage = new byte[] { 0x01, 0x02 },
                FloorSpaces = null // Intentionally setting this to an empty dictionary
            };

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceCreation.CreateSpace(validHashedUsername, companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
            // Adjust the Assert based on your method's expected behavior when floor spaces are missing
            Assert.IsTrue(response.HasError);
        }


    }
}
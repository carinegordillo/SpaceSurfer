using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
// using System.Data.SqlClient;
// using SS.Backend.Services.AccountCreationService;
using SS.Backend.SpaceManager;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using SS.Backend.Services.LoggingService;

namespace SS.Backend.Tests.SpacemodificationTest
{
    [TestClass]
    public class SpaceModificationTest
    {
        private SpaceModification? _spaceModification;
        private SpaceCreation? _spaceCreation;
        private ISpaceManagerDao? _spaceManagerDao; 
        private SqlDAO? _sqlDao;
        private ConfigService? _configService;
        private ILogTarget _logTarget;
        private ILogger _logger;
        // private SqlCommand? _command;

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
            _spaceModification = new SpaceModification(_spaceManagerDao, _logger);
            _spaceCreation = new SpaceCreation(_spaceManagerDao, _logger);
            
        }
        [TestCleanup]
        public async Task TestCleanup()
        {
            await CleanupTestData();
        }
        private async Task CleanupTestData()
        {
            try
            {
                CompanyFloor companyFloor = new CompanyFloor{
                    hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ", 
                    FloorPlanName = "test floor",
                };
                var response = await _spaceModification.DeleteFloor(companyFloor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test cleanup: {ex}");
            }
        }


        [TestMethod]
        public async Task ModifyFloorPlan_Success()
        {
            var validFloorInfo = new CompanyFloor
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                FloorPlanName = "test floor",
                FloorPlanImage = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                FloorSpaces = new Dictionary<string, int> { { "test1", 2 }, { "tets2", 3 }, {"tets3", 3} },
            };
            var createresponse = await _spaceCreation.CreateSpace(validFloorInfo);


            Stopwatch timer = new Stopwatch();
            CompanyFloor companyFloor = new CompanyFloor{
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ", 
                FloorPlanName = "test floor",
                FloorPlanImage =  new byte[] { 0x08, 0x02, 0x03, 0x09 }
            };
            timer.Start();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceModification.ModifyFloorImage(companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            timer.Stop();
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }
     
        [TestMethod]
        public async Task ModifySpace_Success()
        {
            var validFloorInfo = new CompanyFloor
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                FloorPlanName = "test floor",
                FloorPlanImage = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                FloorSpaces = new Dictionary<string, int> { { "test1", 2 }, { "tets2", 3 }, {"tets3", 3} },
            };
            var createresponse = await _spaceCreation.CreateSpace(validFloorInfo);

            Stopwatch timer = new Stopwatch();
            SpaceModifier spaceModifier = new SpaceModifier{
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ", 
                spaceID = "test1",
                newTimeLimit =  10
            };
            timer.Start();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceModification.ModifyTimeLimit(spaceModifier);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            timer.Stop();
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteSpace_Success()
        {
            var validFloorInfo = new CompanyFloor
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                FloorPlanName = "test floor",
                FloorPlanImage = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                FloorSpaces = new Dictionary<string, int> { { "test1", 2 }, { "test2", 3 }, {"tets3", 3} },
            };
            var createresponse = await _spaceCreation.CreateSpace(validFloorInfo);

            Stopwatch timer = new Stopwatch();
            SpaceModifier spaceModifier = new SpaceModifier{
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ", 
                spaceID = "test2",
            };
            timer.Start();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceModification.DeleteSpace(spaceModifier);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            timer.Stop();
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }
        [TestMethod]
        public async Task getCompanyFloor_Success()
        {
            var validFloorInfo = new CompanyFloor
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                FloorPlanName = "test floor",
                FloorPlanImage = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                FloorSpaces = new Dictionary<string, int> { { "test1", 2 }, { "tets2", 3 }, {"tets3", 3} },
            };
            var createresponse = await _spaceCreation.CreateSpace(validFloorInfo);

            Stopwatch timer = new Stopwatch();
            timer.Start();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceModification.getCompanyFloor("kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            timer.Stop();
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteFloor_Success()
        {
            var validFloorInfo = new CompanyFloor
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                FloorPlanName = "test floor",
                FloorPlanImage = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                FloorSpaces = new Dictionary<string, int> { { "test1", 2 }, { "tets2", 3 }, {"tets3", 3} },
            };
            var createresponse = await _spaceCreation.CreateSpace(validFloorInfo);

            Stopwatch timer = new Stopwatch();
            CompanyFloor companyFloor = new CompanyFloor{
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ", 
                FloorPlanName = "test floor"
            };
            timer.Start();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _spaceModification.DeleteFloor(companyFloor);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            timer.Stop();
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            // await CleanupTestData().ConfigureAwait(false);
        }
    }
}
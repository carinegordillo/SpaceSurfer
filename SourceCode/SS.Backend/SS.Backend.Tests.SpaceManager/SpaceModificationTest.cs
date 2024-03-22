using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
// using System.Data.SqlClient;
// using SS.Backend.Services.AccountCreationService;
using SS.Backend.SpaceManager;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace SS.Backend.Tests.SpacemodificationTest
{
    [TestClass]
    public class SpaceModificationTest
    {
        private SpaceModification _spaceModification;
        private ISpaceManagerDao _spaceManagerDao; 
        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private SqlCommand _command;

        [TestInitialize]
        public void Setup()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);
            _spaceManagerDao = new SpaceManagerDao(_sqlDao);
            _spaceModification = new SpaceModification(_spaceManagerDao);
            
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
        public async Task ModifyFloorPlan_Success()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var newFloorPlanImage = new byte[] { 0x01, 0x02, 0x03, 0x09 };
            var response = await _spaceModification.ModifyFloorImage(4, "new flooroa", newFloorPlanImage);
            timer.Stop();
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }
     
        [TestMethod]
        public async Task ModifySpace_Success()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var response = await _spaceModification.ModifyTimeLimit(4, "A4a", 3);
            timer.Stop();
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }

        [TestMethod]
        public async Task DeleteSpace_Success()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var response = await _spaceModification.DeleteSpace(4, "A4a");
            timer.Stop();
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteFloor_Success()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var response = await _spaceModification.DeleteFloor(6, "Testing Floor");
            timer.Stop();
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task getCompanyFloor_Success()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var response = await _spaceModification.getCompanyFloor(4);
            timer.Stop();
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }


    }
}
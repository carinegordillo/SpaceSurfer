using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
// using System.Data.SqlClient;
// using SS.Backend.Services.AccountCreationService;
using SS.Backend.SpaceManager;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace SS.Backend.Tests.SpaceManagerTest
{
    [TestClass]
    public class SpaceManagerTests
    {
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
        public async Task CreateUserAccount_Success()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            CompanyInfo companyInfo = new CompanyInfo();
            CompanyFloor companyFloor = new CompanyFloor();
            SpaceCreation spaceCreation = new SpaceCreation();
            Stopwatch timer = new Stopwatch();


            var validCompanyInfo = new CompanyInfo
            {
                companyName = "NewService", 
                address = "Irvine", 
                openingHours = "2:00:00",
                closingHours = "2:00:00" ,
                daysOpen = "Monday,Tuesday"
            };
           
            var validFloorInfo = new CompanyFloor
            {
                FloorPlanName = "New Company",
                FloorPlanImage = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                FloorSpaces = new Dictionary<string, int> { { "F5", 2 }, { "F6", 3 } },
            };

            timer.Start();
            var response = await spaceCreation.CreateSpace(validCompanyInfo, validFloorInfo);
            timer.Stop();

            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }

    }
}
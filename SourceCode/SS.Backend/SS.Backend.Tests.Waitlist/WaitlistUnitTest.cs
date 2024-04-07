//using SS.Backend.DataAccess;
//using SS.Backend.Services.EmailService;
//using SS.Backend.SharedNamespace;
//using Microsoft.Data.SqlClient;
//using System.Threading.Tasks;
//using System.Text;
//using System.Data;
//using System;

//namespace SS.Backend.Tests.Waitlist;

//[TestClass]
//public class WaitlistUnitTest
//{
//    [TestInitialize]
//    public void TestInitialize()
//    {
//        Response result = new Response();
//        var builder = new CustomSqlCommandBuilder();
//        var baseDirectory = AppContext.BaseDirectory;
//        var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//        var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
//        ConfigService configService = new ConfigService(configFilePath);
//        SqlDAO dao = new SqlDAO(configService);
//    }

//    private async Task CleanupTestData()
//    {

//        try
//        {
//            var cleanupCmd = builder.WaitlistCleanup().Build();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Exception during test cleanup: {ex}");
//        }
//    }

//    [TestMethod]
//    public async Task DataAccess_SuccessfulConnection_Pass()
//    {
//        // Assert that dao is not null before proceeding
//        Assert.IsNotNull(dao);

//        // Arrange
//        string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Info', 'test@email', 'View', 'test desc');";
//        var command = new SqlCommand(sql);

//        // Act
//        var result = await dao!.SqlRowsAffected(command); // Using ! to assert non-null

//        // Assert
//        Assert.IsFalse(result.HasError);
//        Assert.IsTrue(result.RowsAffected > 0);

//        // Cleanup
//        await CleanupTestData();
//    }
//}
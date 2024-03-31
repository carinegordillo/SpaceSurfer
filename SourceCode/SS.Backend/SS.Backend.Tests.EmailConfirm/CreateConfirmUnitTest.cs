using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
using SS.Backend.EmailConfirm;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using System.Data;
using System.Threading.Tasks;
using System;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace SS.Backend.Tests.EmailConfirm;

[TestClass]
public class CreateConfirmUnitTest
{
    private EmailConfirmService _emailConfirm;
    private IEmailConfirmDAO _emailDAO;
    private SqlDAO _sqlDao;
    private ConfigService _configService;

    [TestInitialize]
    public void Setup()
    {
        
        var baseDirectory = AppContext.BaseDirectory;
        var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
        _configService = new ConfigService(configFilePath);
        _sqlDao = new SqlDAO(_configService);
        _emailDAO = new EmailConfirmDAO(_sqlDao);
        _emailConfirm = new EmailConfirmService(_emailDAO);
    }

    private async Task CleanupTestData()
    {
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

                string sql1 = $"DELETE FROM dbo.ConfirmReservations WHERE [reservationID] = '8'";
                string sql2 = $"DELETE FROM dbo.Reservations WHERE [spaceID] = 'SPACE103'";

                using (SqlCommand command1 = new SqlCommand(sql1, connection))
                {
                    await command1.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                using (SqlCommand command2 = new SqlCommand(sql2, connection))
                {
                    await command2.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during test cleanup: {ex}");
        }
    }

    [TestMethod]
    public async Task CreateConfirm_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        int reservationID = 8;
        var response = new Response();
        var builder = new CustomSqlCommandBuilder();
        var parameters = new Dictionary<string, object>
        {
            { "companyID", 1 },
            { "floorPlanID", 1 },
            { "spaceID", "SPACE103" },
            { "reservationDate", "2024-04-01" },
            { "reservationStartTime", "13:00:00" },
            { "reservationEndTime", "17:00:00" },
            { "status", "Available" }
        };
        var insertCommand = builder
            .BeginInsert("Reservations")
            .Columns(parameters.Keys)
            .Values(parameters.Keys)
            .AddParameters(parameters)
            .Build();
        await _sqlDao!.SqlRowsAffected(insertCommand);

        //Act
        timer.Start();
        (string ics, string otp, result) = await _emailConfirm.CreateConfirmation(reservationID);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsNotNull(ics);
        Assert.IsNotNull(otp);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);

    }

    // [TestMethod]
    // public void CreateConfirm_NullInputs_Fail()
    // {

    // }

    // [TestMethod]
    // public void CreateConfirm_MissingInfo_Fail()
    // {

    // }

    // [TestMethod]
    // public void CreateConfirm_Timeout_Fail()
    // {

    // }

    // [TestMethod]
    // public void CreateConfirm_DBRetrieval_Fail()
    // {

    // }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.Backend.EmailConfirm;
using SS.Backend.SharedNamespace;
using SS.Backend.Services.CalendarService;
using SS.Backend.DataAccess;
using System.Data;
using System.Threading.Tasks;
using System;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace SS.Backend.Tests.EmailConfirm;

[TestClass]
public class ConfirmListUnitTest
{
    private IEmailConfirmDAO _emailDAO;
    private SqlDAO _sqlDao;
    private ConfigService _configService;
    private EmailConfirmList _confirmList;

    [TestInitialize]
    public void Setup()
    {
        
        var baseDirectory = AppContext.BaseDirectory;
        var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
        _configService = new ConfigService(configFilePath);
        _sqlDao = new SqlDAO(_configService);
        _emailDAO = new EmailConfirmDAO(_sqlDao);
        _confirmList = new EmailConfirmList(_emailDAO);
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

                string sql1 = $"DELETE FROM dbo.ConfirmReservations WHERE [reservationID] = '40'";

                using (SqlCommand command1 = new SqlCommand(sql1, connection))
                {
                    await command1.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during test cleanup: {ex}");
        }
    }

    [TestMethod]
    public async Task GetAllTableInfo_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var tableName = "Reservations";
        var response = new Response();

        //Act
        timer.Start();
        result = await _emailDAO.GetAllTableInfo(tableName);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);

    }

    [TestMethod]
    public async Task ConfirmList_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        var hashedUsername = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=";

        //Act
        timer.Start();
        var results = await _confirmList.ListConfirmations(hashedUsername);
        timer.Stop();

        //Assert
        Assert.IsNotNull(results);
        //Assert.AreEqual(0, results.Count(), "Expected list of confirmations for valid hashed username.");
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ConfirmList_InvalidInputs_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        var hashedUsername = "bobsworld";

        //Act
        timer.Start();
        var results = await _confirmList.ListConfirmations(hashedUsername);
        timer.Stop();

        //Assert
        Assert.IsNotNull(results);
        Assert.AreEqual(0, results.Count(), "Expected no confirmations for an invalid username.");  // Use LINQ to count elements
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ListConfirmations_Timeout_Fail()
    {
        //Arrange
        var hashedUsername = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=";
        var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(3000));

        //Act
        var operationTask = _confirmList.ListConfirmations(hashedUsername);
        var completedTask = await Task.WhenAny(operationTask, timeoutTask);

        // Assert
        if (completedTask == operationTask)
        {
            // Operation completed before timeout, now it's safe to await it and check results
            var results = await operationTask;

            // Assert the operation's success
            Assert.IsNotNull(results);
            //Assert.AreEqual(0, results.Count(), "Expected list of confirmations for valid hashed username.");
        }
        else
        {
            // Fail the test if we hit the timeout
            Assert.Fail("The ListConfirmations operation timed out.");
        }

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);
    }
}
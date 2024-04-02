using SS.Backend.EmailConfirm;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace SS.Backend.Tests.EmailConfirm;

[TestClass]
public class ResendUnitTest
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

                string sql1 = $"DELETE FROM dbo.ConfirmReservations WHERE [reservationID] = '7'";
                //string sql2 = $"DELETE FROM dbo.Reservations WHERE [spaceID] = 'SPACE103'";

                using (SqlCommand command1 = new SqlCommand(sql1, connection))
                {
                    await command1.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            //     using (SqlCommand command2 = new SqlCommand(sql2, connection))
            //     {
            //         await command2.ExecuteNonQueryAsync().ConfigureAwait(false);
            //     }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during test cleanup: {ex}");
        }
    }

    [TestMethod]
    public async Task UpdateOtp_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        int reservationID = 7;
        var getOtp = new GenOTP();
        var newOtp = getOtp.generateOTP();

        //Act
        timer.Start();
        (string icsFile, string otp, result) = await _emailConfirm.CreateConfirmation(reservationID);
        var otpResult = await _emailDAO.UpdateOtp(reservationID, newOtp);
        timer.Stop();

        //Assert
        Assert.IsFalse(otpResult.HasError, otpResult.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task GetStatus_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        int reservationID = 7;

        //Act
        timer.Start();
        (string icsFile, string otp, result) = await _emailConfirm.CreateConfirmation(reservationID);
        var statusResult = await _emailDAO.GetConfirmInfo(reservationID);
        timer.Stop();

        //Assert
        Assert.IsFalse(statusResult.HasError, statusResult.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);
    }


    [TestMethod]
    public async Task ResendConfirm_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        int reservationID = 7;
        (string icsFile, string otp, result) = await _emailConfirm.CreateConfirmation(reservationID);

        //Act
        timer.Start();
        (icsFile, otp, result) = await _emailConfirm.ResendConfirmation(reservationID);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsNotNull(icsFile);
        Assert.IsNotNull(otp);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ResendConfirm_InvalidInputs_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        int reservationID = -1;
        (string icsFile, string otp, result) = await _emailConfirm.CreateConfirmation(reservationID);

        //Act
        timer.Start();
        (icsFile, otp, result) = await _emailConfirm.ResendConfirmation(reservationID);
        timer.Stop();

        //Assert
        Assert.IsTrue(result.HasError, "Expected ResendConfirmation to fail with invalid input.");
        Assert.IsFalse(string.IsNullOrEmpty(result.ErrorMessage), "Expected an error message for invalid input.");
        Assert.IsNotNull(icsFile);
        Assert.IsNotNull(otp);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task CreateConfirm_Timeout_Fail()
    {
        //Arrange
        int reservationID = 7;
        var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(3000));
        (string icsFile, string otp, Response response) = await _emailConfirm.CreateConfirmation(reservationID);

        //Act
        var operationTask =  _emailConfirm.ResendConfirmation(reservationID);
        var completedTask = await Task.WhenAny(operationTask, timeoutTask);

        // Assert
        if (completedTask == operationTask)
        {
            // Operation completed before timeout, now it's safe to await it and check results
            (icsFile, otp, response) = await operationTask;

            // Assert the operation's success
            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsNotNull(otp);
        }
        else
        {
            // Fail the test if we hit the timeout
            Assert.Fail("The CreateConfirmation operation timed out.");
        }

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);
    }

    // // [TestMethod]
    // // public async Tas CreateConfirm_DBRetrieval_Fail()
    // // {

    // // }
}
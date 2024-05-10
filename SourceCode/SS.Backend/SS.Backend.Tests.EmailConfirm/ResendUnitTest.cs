using SS.Backend.EmailConfirm;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using SS.Backend.Services.LoggingService;

namespace SS.Backend.Tests.EmailConfirm;

[TestClass]
public class ResendUnitTest
{
     private EmailConfirmService _emailConfirm;
    private IEmailConfirmDAO _emailDAO;
    private SqlDAO _sqlDao;
    private ConfigService _configService;
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
        _emailDAO = new EmailConfirmDAO(_sqlDao);
        _emailConfirm = new EmailConfirmService(_emailDAO, _logger);
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

                string sql1 = $"DELETE FROM dbo.ConfirmReservations WHERE [reservationID] = '4'";

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
    public async Task UpdateOtp_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        int reservationID = 4;
        var getOtp = new GenOTP();
        var newOtp = getOtp.generateOTP();

        //Act
        timer.Start();
        var (icsFile, otp, html, result) = await _emailConfirm.CreateConfirmation(reservationID);
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
        int reservationID = 4;

        //Act
        timer.Start();
        var (icsFile, otp, html, result)= await _emailConfirm.CreateConfirmation(reservationID);
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
        int reservationID = 4;
        var (icsFile, otp, html, result)= await _emailConfirm.CreateConfirmation(reservationID);

        //Act
        timer.Start();
        (icsFile, otp, html, result) = await _emailConfirm.ResendConfirmation(reservationID);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsNotNull(icsFile);
        Assert.IsNotNull(otp);
        Assert.IsNotNull(html);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ResendConfirm_Confirmed_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        int reservationID = 14;
        //var (icsFile, otp, html, result) = await _emailConfirm.CreateConfirmation(reservationID);

        //Act
        timer.Start();
        var (icsFile, otp, html, result)= await _emailConfirm.ResendConfirmation(reservationID);
        timer.Stop();

        //Assert
        Assert.IsTrue(result.HasError, "Expected ResendConfirmation to fail with confirmed reservation.");
        Assert.IsFalse(string.IsNullOrEmpty(result.ErrorMessage), "Expected an error message for confirmed reservation.");
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ResendConfirm_InvalidInputs_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        int reservationID = -1;
        var (icsFile, otp, html, result) = await _emailConfirm.CreateConfirmation(reservationID);

        //Act
        timer.Start();
        (icsFile, otp, html, result)= await _emailConfirm.ResendConfirmation(reservationID);
        timer.Stop();

        //Assert
        Assert.IsTrue(result.HasError, "Expected ResendConfirmation to fail with invalid input.");
        Assert.IsFalse(string.IsNullOrEmpty(result.ErrorMessage), "Expected an error message for invalid input.");
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        //await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task CreateConfirm_Timeout_Fail()
    {
        //Arrange
        int reservationID = 4;
        var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(3000));
        var (icsFile, otp, html, response) = await _emailConfirm.CreateConfirmation(reservationID);

        //Act
        var operationTask =  _emailConfirm.ResendConfirmation(reservationID);
        var completedTask = await Task.WhenAny(operationTask, timeoutTask);

        // Assert
        if (completedTask == operationTask)
        {
            // Operation completed before timeout, now it's safe to await it and check results
            (icsFile, otp, html, response) = await operationTask;

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

}
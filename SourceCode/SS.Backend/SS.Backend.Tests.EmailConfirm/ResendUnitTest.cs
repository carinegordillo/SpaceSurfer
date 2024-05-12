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


    private async Task CleanupTestData(int reservationID)
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

                // Start a database transaction for the cleanup
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete from dependent tables first if foreign key constraints exist
                        string deleteConfirmationsSql = $@"
                            DELETE FROM [dbo].[ConfirmReservations] 
                            WHERE reservationID = {reservationID};";
                        using (SqlCommand command = new SqlCommand(deleteConfirmationsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ReservationID", reservationID);
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        // Then delete from the main Reservations table
                        string deleteReservationsSql = $@"DELETE FROM [dbo].[Reservations] WHERE reservationID = {reservationID};";
                        using (SqlCommand command = new SqlCommand(deleteReservationsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ReservationID", reservationID);
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction in case of an error
                        transaction.Rollback();
                        Console.WriteLine($"An error occurred during cleanup: {ex.Message}");
                        throw;  // Optional: rethrow the exception if you want to handle it outside
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during cleanup: {ex.Message}");
        }
    }

    private async Task<int> InsertReservationTestData()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

        ConfigService configFile = new ConfigService(configFilePath);
        var connectionString = configFile.GetConnectionString();
        int reservationID = 0; 

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                // Start a database transaction.
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // insert data into Reservations table
                        string sql = @"
                            INSERT INTO [dbo].[Reservations] 
                            (companyID, floorPlanID, spaceID, reservationDate, reservationStartTime, reservationEndTime, status, userHash) 
                            OUTPUT INSERTED.reservationID 
                            VALUES (9, 8, 'SPACE022', '2024-06-22', '2024-05-08T09:00:00Z', '2024-05-05T11:00:00Z', 'Active', '7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=');
                        ";
                        using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                        {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                            reservationID = (int)await command.ExecuteScalarAsync().ConfigureAwait(false);
#pragma warning restore CS8605 // Unboxing a possibly null value.
                        }

                        // insert data in confirmReservations for reservationID
                        string sqlConfirm = $@"
                            INSERT INTO [dbo].[ConfirmReservations] (reservationID, reservationOTP, confirmStatus, icsFile) 
                            VALUES ({reservationID}, '123456', 'yes', null);
                        ";
                        using (SqlCommand commandConfirm = new SqlCommand(sqlConfirm, connection, transaction))
                        {
                            commandConfirm.Parameters.AddWithValue("@ReservationID", reservationID);
                            await commandConfirm.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Something went wrong within the transaction, roll it back
                        transaction.Rollback();
                        Console.WriteLine($"Transaction rolled back due to an exception: {ex.Message}");
                        throw; // Re-throw the exception to handle it outside or log it
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during data insertion: {ex.Message}");
        }
        return reservationID; // Return the generated ID, or 0 if an error occurred
    
    }

    private async Task<int> InsertResOnlyTestData()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

        ConfigService configFile = new ConfigService(configFilePath);
        var connectionString = configFile.GetConnectionString();
        int reservationID = 0; 

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                // Start a database transaction.
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // insert data into Reservations table
                        string sql = @"
                            INSERT INTO [dbo].[Reservations] 
                            (companyID, floorPlanID, spaceID, reservationDate, reservationStartTime, reservationEndTime, status, userHash) 
                            OUTPUT INSERTED.reservationID 
                            VALUES (9, 8, 'SPACE022', '2024-06-22', '2024-05-08T09:00:00Z', '2024-05-05T11:00:00Z', 'Active', '7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=');
                        ";
                        using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                        {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                            reservationID = (int)await command.ExecuteScalarAsync().ConfigureAwait(false);
#pragma warning restore CS8605 // Unboxing a possibly null value.
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Something went wrong within the transaction, roll it back
                        transaction.Rollback();
                        Console.WriteLine($"Transaction rolled back due to an exception: {ex.Message}");
                        throw; // Re-throw the exception to handle it outside or log it
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during data insertion: {ex.Message}");
        }
        return reservationID; // Return the generated ID, or 0 if an error occurred
    
    }

    [TestMethod]
    public async Task UpdateOtp_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        var reservationIDTask = await InsertReservationTestData();
        var reservationID = Convert.ToInt32(reservationIDTask);

        var getOtp = new GenOTP();
        var newOtp = getOtp.generateOTP();

        //Act
        timer.Start();
        //var (icsFile, otp, html, result) = await _emailConfirm.CreateConfirmation(reservationID);
        var otpResult = await _emailDAO.UpdateOtp(reservationID, newOtp);
        timer.Stop();

        //Assert
        Assert.IsFalse(otpResult.HasError, otpResult.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task GetStatus_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        var reservationID = await InsertReservationTestData();

        //Act
        timer.Start();
        //var (icsFile, otp, html, result)= await _emailConfirm.CreateConfirmation(reservationID);
        var statusResult = await _emailDAO.GetConfirmInfo(reservationID);
        timer.Stop();

        //Assert
        Assert.IsFalse(statusResult.HasError, statusResult.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID).ConfigureAwait(false);
    }


    [TestMethod]
    public async Task ResendConfirm_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        var reservationID = await InsertResOnlyTestData();
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
        await CleanupTestData(reservationID).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ResendConfirm_Confirmed_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        var reservationID = await InsertReservationTestData();
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
        await CleanupTestData(reservationID).ConfigureAwait(false);
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
    public async Task ResendConfirm_Timeout_Fail()
    {
        //Arrange
        var reservationID = await InsertResOnlyTestData();
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
        await CleanupTestData(reservationID).ConfigureAwait(false);
    }

}
using SS.Backend.EmailConfirm;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using SS.Backend.Services.LoggingService;

namespace SS.Backend.Tests.EmailConfirm;

[TestClass]
public class ConfirmationUnitTest
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
                        string deleteConfirmationsSql = "DELETE FROM [dbo].[ConfirmReservations] WHERE reservationID = @ReservationID;";
                        using (SqlCommand command = new SqlCommand(deleteConfirmationsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ReservationID", reservationID);
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        // Then delete from the main Reservations table
                        string deleteReservationsSql = "DELETE FROM [dbo].[Reservations] WHERE reservationID = @ReservationID;";
                        using (SqlCommand command = new SqlCommand(deleteReservationsSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ReservationID", reservationID);
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        // Then delete from userHash table
                        string userHash = "testUser";
                        string deleteUserSql = "DELETE FROM [dbo].[userHash] WHERE hashedUsername = @UserHash;";
                        using (SqlCommand command = new SqlCommand(deleteUserSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@UserHash", userHash);
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                        // Then delete from userAccount table
                        string username = "Sarah.Santos@student.csulb.edu";
                        string deleteAcctSql = "DELETE FROM [dbo].[userAccount] WHERE username = @UserName;";
                        using (SqlCommand command = new SqlCommand(deleteAcctSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@UserName", username);
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
                        // insert into userAccount
                        string sqlAcct = $@"
                            INSERT INTO [dbo].[userAccount] (username, birthDate, companyID) 
                            VALUES ('Sarah.Santos@student.csulb.edu', '2002-01-11', null);
                        ";
                        using (SqlCommand commandUser = new SqlCommand(sqlAcct, connection, transaction))
                        {
                            await commandUser.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                        //insert into userHash
                        string sqlUser = $@"
                            INSERT INTO [dbo].[userHash] (hashedUsername, username) 
                            VALUES ('testUser', 'Sarah.Santos@student.csulb.edu');
                        ";
                        using (SqlCommand commandUser = new SqlCommand(sqlUser, connection, transaction))
                        {
                            await commandUser.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        // insert into Reservations table
                        string sql = @"
                            INSERT INTO [dbo].[Reservations] 
                            (companyID, floorPlanID, spaceID, reservationDate, reservationStartTime, reservationEndTime, status, userHash) 
                            OUTPUT INSERTED.reservationID 
                            VALUES (9, 8, 'SPACE022', '2024-04-22', '2024-04-08T09:00:00Z', '2024-04-05T11:00:00Z', 'Active', 'testUser');
                        ";
                        using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                        {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                            reservationID = (int)await command.ExecuteScalarAsync().ConfigureAwait(false);
#pragma warning restore CS8605 // Unboxing a possibly null value.
                        }

                        // insert data in confirmReservations for reservationID
                        string sqlConfirm = @"
                            INSERT INTO [dbo].[ConfirmReservations] (reservationID, reservationOTP, confirmStatus, icsFile) 
                            VALUES (@ReservationID, '123456', 'yes', null);
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
                        // insert into userAccount
                        string sqlAcct = $@"
                            INSERT INTO [dbo].[userAccount] (username, birthDate, companyID) 
                            VALUES ('Sarah.Santos@student.csulb.edu', '2002-01-11', null);
                        ";
                        using (SqlCommand commandUser = new SqlCommand(sqlAcct, connection, transaction))
                        {
                            await commandUser.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                        //insert into userHash
                        string sqlUser = $@"
                            INSERT INTO [dbo].[userHash] (hashedUsername, username) 
                            VALUES ('testUser', 'Sarah.Santos@student.csulb.edu');
                        ";
                        using (SqlCommand commandUser = new SqlCommand(sqlUser, connection, transaction))
                        {
                            await commandUser.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
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
    public async Task UpdateStatus_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var reservationID = await InsertReservationTestData();
        //(string icsFile, string otp, string html, result) = await _emailConfirm.CreateConfirmation(reservationID);

        //Act
        timer.Start();
        result = await _emailDAO.UpdateConfirmStatus(reservationID);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ConfirmEmail_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var reservationID = await InsertResOnlyTestData();
        Console.WriteLine(reservationID);
        (string icsFile, string otp, string html, result) = await _emailConfirm.CreateConfirmation(reservationID);

        //Act
        timer.Start();
        result = await _emailConfirm.ConfirmReservation(reservationID, otp);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID);
    }


    [TestMethod]
    public async Task ResendConfirm_InvalidID_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        int reservationID = -1;
        var getOtp = new GenOTP();
        var newOtp = getOtp.generateOTP();
        (string icsFile, string otp, string html, result) = await _emailConfirm.CreateConfirmation(reservationID);

         //Act
        timer.Start();
        result = await _emailConfirm.ConfirmReservation(reservationID, newOtp);
        timer.Stop();

        //Assert
        Assert.IsTrue(result.HasError, "Expected ConfirmReservation to fail with invalid input.");
        Assert.IsFalse(string.IsNullOrEmpty(result.ErrorMessage), "Expected an error message for invalid input.");
        // Assert.IsNotNull(icsFile);
        // Assert.IsNotNull(otp);
        // Assert.IsNotNull(html);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        //await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ConfirmReservation_Timeout_Fail()
    {
        //Arrange
        var reservationID = await InsertResOnlyTestData();
        var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(3000));
        (string icsFile, string otp, string html, Response result)= await _emailConfirm.CreateConfirmation(reservationID);

        //Act
        var operationTask =  _emailConfirm.ConfirmReservation(reservationID, otp);
        var completedTask = await Task.WhenAny(operationTask, timeoutTask);

        // Assert
        if (completedTask == operationTask)
        {
            // Operation completed before timeout, now it's safe to await it and check results
            result = await operationTask;

            // Assert the operation's success
            Assert.IsFalse(result.HasError, result.ErrorMessage);
            Assert.IsNotNull(otp);
        }
        else
        {
            // Fail the test if we hit the timeout
            Assert.Fail("The ConfirmReservation operation timed out.");
        }

        //Cleanup
        await CleanupTestData(reservationID);
    }

}
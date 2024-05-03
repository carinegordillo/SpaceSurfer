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
public class ConfirmationDeletionUnitTest
{
    private EmailConfirmService _emailConfirm;
    private IEmailConfirmDAO _emailDAO;
    private SqlDAO _sqlDao;
    private ConfigService _configService;
    private EmailConfirmList _confirmList;
    private ConfirmationDeletion _confirmDelete;

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
        _confirmList = new EmailConfirmList(_emailDAO);
        _confirmDelete = new ConfirmationDeletion(_emailDAO, _confirmList);
    }

    private async Task CleanupTestData(int reservationID)
    {
        // var baseDirectory = AppContext.BaseDirectory;
        // var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        // var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

        // ConfigService configFile = new ConfigService(configFilePath);
        // var connectionString = configFile.GetConnectionString();
        // try
        // {
        //     using (SqlConnection connection = new SqlConnection(connectionString))
        //     {
        //         await connection.OpenAsync().ConfigureAwait(false);

        //         string sql1 = $"DELETE FROM dbo.ConfirmReservations WHERE [reservationID] = '3'";

        //         using (SqlCommand command1 = new SqlCommand(sql1, connection))
        //         {
        //             await command1.ExecuteNonQueryAsync().ConfigureAwait(false);
        //         }
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"Exception during test cleanup: {ex}");
        // }
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
        int reservationID = 0; // This will store the generated reservation ID

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
                        string sql = @"
                            INSERT INTO [dbo].[Reservations] 
                            (companyID, floorPlanID, spaceID, reservationDate, reservationStartTime, reservationEndTime, status, userHash) 
                            OUTPUT INSERTED.reservationID 
                            VALUES (9, 8, 'SPACE022', '2024-04-22', '2024-04-08T09:00:00Z', '2024-04-05T11:00:00Z', 'Active', '7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=');
                        ";
                        using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                        {
                            // Execute the command and get the inserted reservation ID
#pragma warning disable CS8605 // Unboxing a possibly null value.
                            reservationID = (int)await command.ExecuteScalarAsync().ConfigureAwait(false);
#pragma warning restore CS8605 // Unboxing a possibly null value.
                        }

                        // Optionally, insert into another table if needed using reservationID
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
    
        // var baseDirectory = AppContext.BaseDirectory;
        // var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        // var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

        // ConfigService configFile = new ConfigService(configFilePath);
        // var connectionString = configFile.GetConnectionString();
        // int reservationID = 0;
        // try
        // {
        //     using (SqlConnection connection = new SqlConnection(connectionString))
        //     {
        //         await connection.OpenAsync().ConfigureAwait(false);

        //         string sql1 = $"INSERT INTO [dbo].[Reservations] (companyID, floorPlanID, spaceID, reservationDate, reservationStartTime, reservationEndTime, status, userHash) VALUES ( 9, 8, 'SPACE022', '2024-04-22', '2024-04-08T09:00:00Z', '2024-04-05T11:00:00Z', 'Available', '7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=')";
        //         using (SqlCommand command1 = new SqlCommand(sql1, connection))
        //         {
        //             //reservationID = (int)await command1.ExecuteScalarAsync().ConfigureAwait(false); 
        //             reservationID = Convert.ToInt32(await command1.ExecuteScalarAsync().ConfigureAwait(false));
        //         }
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"Exception during data insertion: {ex}");
        // }
        // return reservationID;
    }

    private async Task<int> InsertConfirmationTestData()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

        ConfigService configFile = new ConfigService(configFilePath);
        var connectionString = configFile.GetConnectionString();
        var reservationID = await InsertReservationTestData();
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                string sql1 = $"INSERT INTO [dbo].[ConfirmReservations](reservationID, reservationOTP, confirmStatus, icsFile) VALUES ({reservationID}, '123456', 'yes', null);";
                using (SqlCommand command1 = new SqlCommand(sql1, connection))
                {
                    //reservationID = (int)await command1.ExecuteScalarAsync().ConfigureAwait(false); 
                    reservationID = Convert.ToInt32(await command1.ExecuteScalarAsync().ConfigureAwait(false));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during data insertion: {ex}");
        }
        return reservationID;
    }

    [TestMethod]
    public async Task CancelConfirmation_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var reservationID = await InsertReservationTestData();

        //Act
        timer.Start();
        result = await _emailDAO.CancelConfirmation(reservationID);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID);
    }

    [TestMethod]
    public async Task CancelConfirmedReservation_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var reservationID = await InsertReservationTestData();
        var hashedUsername = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=";

        //Act
        timer.Start();
        result = await _confirmDelete.CancelConfirmedReservation(hashedUsername, reservationID);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID);

    }

    [TestMethod]
    public async Task DeleteConfirmedReservation_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var reservationID = await InsertConfirmationTestData();
        //var hashedUsername = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=";

        //Act
        timer.Start();
        result = await _confirmDelete.DeleteConfirmedReservation(reservationID);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID);
    }

    [TestMethod]
    public async Task CancelConfirmedReservation_InvalidID_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var reservationID = -1;
        var hashedUsername = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=";

        //Act
        timer.Start();
        result = await _confirmDelete.CancelConfirmedReservation(hashedUsername, reservationID);
        timer.Stop();

        //Assert
        Assert.IsTrue(result.HasError, "Expected CancelConfirmedReservation to fail with invalid input.");
        Assert.IsFalse(string.IsNullOrEmpty(result.ErrorMessage), "Expected an error message for invalid input.");
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        //await CleanupTestData(reservationID);

    }

    [TestMethod]
    public async Task CancelConfirmedReservation_InvalidUser_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var reservationID = await InsertReservationTestData();
        var hashedUsername = "bobsworld";

        //Act
        timer.Start();
        result = await _confirmDelete.CancelConfirmedReservation(hashedUsername, reservationID);
        timer.Stop();

        //Assert
        Assert.IsTrue(result.HasError, "Expected CancelConfirmedReservation to fail with invalid input.");
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID);

    }

    [TestMethod]
    public async Task DeleteConfirmedReservation_InvalidInput_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var reservationID = -1;

        //Act
        timer.Start();
        result = await _confirmDelete.DeleteConfirmedReservation(reservationID);
        timer.Stop();

        //Assert
        Assert.IsTrue(result.HasError, "Expected DeleteConfirmedReservation to fail with invalid input.");
        Assert.IsFalse(string.IsNullOrEmpty(result.ErrorMessage), "Expected an error message for invalid input.");
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        //await CleanupTestData(reservationID);

    }

   [TestMethod]
    public async Task CancelConfirmedReservation_Timeout_Fail()
    {
        // Arrange
        var timeoutDuration = TimeSpan.FromMilliseconds(3000);
        var timeoutTask = Task.Delay(timeoutDuration);
        var reservationID = await InsertReservationTestData();  // Assumes this method is correctly setting up test data
        var hashedUsername = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=";
        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var operationTask = _confirmDelete.CancelConfirmedReservation(hashedUsername, reservationID);
        var completedTask = await Task.WhenAny(operationTask, timeoutTask);
        timer.Stop();

        // Assert
        if (completedTask == timeoutTask)
        {
            Assert.Fail("The CancelConfirmedReservation operation timed out.");
        }
        else
        {
            // It's safe to await operationTask here because we know it has completed
            var result = await operationTask;
            Assert.IsFalse(result.HasError, "Expected no errors: " + result.ErrorMessage);
            Assert.IsTrue(timer.Elapsed < timeoutDuration, "Expected operation to complete within " + timeoutDuration.TotalMilliseconds + " milliseconds");
        }

        // Cleanup
        await CleanupTestData(reservationID);
    }

    [TestMethod]
    public async Task DeleteConfirmedReservation_Timeout_Fail()
    {
        // Arrange
        var timeoutDuration = TimeSpan.FromMilliseconds(3000);
        var timeoutTask = Task.Delay(timeoutDuration);
        var reservationID = await InsertReservationTestData();  
        Stopwatch timer = new Stopwatch();

        // Act
        timer.Start();
        var operationTask = _confirmDelete.DeleteConfirmedReservation(reservationID);
        var completedTask = await Task.WhenAny(operationTask, timeoutTask);
        timer.Stop();

        // Assert
        if (completedTask == timeoutTask)
        {
            Assert.Fail("The DeleteConfirmedReservation operation timed out.");
        }
        else
        {
            // It's safe to await operationTask here because we know it has completed
            var result = await operationTask;
            Assert.IsFalse(result.HasError, "Expected no errors: " + result.ErrorMessage);
            Assert.IsTrue(timer.Elapsed < timeoutDuration, "Expected operation to complete within " + timeoutDuration.TotalMilliseconds + " milliseconds");
        }

        // Cleanup
        await CleanupTestData(reservationID);
    }

}

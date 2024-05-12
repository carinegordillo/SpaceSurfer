using SS.Backend.EmailConfirm;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using SS.Backend.ReservationManagement;
using SS.Backend.Services.LoggingService;

namespace SS.Backend.Tests.EmailConfirm;

[TestClass]
public class SendConfirmationUnitTest
{
    private EmailConfirmSender _emailSender;
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
        _logger = new Logger(_logTarget);
        _sqlDao = new SqlDAO(_configService);
        _emailDAO = new EmailConfirmDAO(_sqlDao);
        _emailConfirm = new EmailConfirmService(_emailDAO, _logger);
        _emailSender = new EmailConfirmSender(_emailConfirm, _emailDAO, _logger);
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
                            VALUES ({reservationID}, '123456', 'yes', 
                            0x424547494E3A5643414C454E4441520A56455253494F4E3A322E300A50524F4449443A2D2F2F53706163655375726665722F2F5265736572766174696F6E20436F6E6669726D6174696F6E2F2F454E0A424547494E3A564556454E540A5549443A38366435393235612D333735632D343563382D393961612D35333131363062353034313340676D61696C2E636F6D0A44545354414D503A3230323430343038543132303030300A445453544152543A3230323430343038543132303030300A4454454E443A3230323430343035543133303030300A53554D4D4152593A5370616365537572666572205265736572766174696F6E0A4445534352495054494F4E3A5265736572766174696F6E2061743A205374656C6C617220436F6666656520526F617374657273205265736572766174696F6E49443A203720537061636549443A20535041434530323220200A4C4F434154494F4E3A34303820537461726C6967687420457870726573737761792C2053656174746C652C205741202020202020202020202020200A454E443A564556454E540A454E443A5643414C454E4441520A);
                        ";
                        using (SqlCommand commandConfirm = new SqlCommand(sqlConfirm, connection, transaction))
                        {
                            commandConfirm.Parameters.AddWithValue("@ReservationID", reservationID);
                            await commandConfirm.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        string sqlUser = $@"
                            INSERT INTO [dbo].[userHash] (hashedUsername, username, user_id) 
                            VALUES ('7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=', '4sarahsantos@gmail.com', 0110);
                        ";
                        using (SqlCommand commandUser = new SqlCommand(sqlUser, connection, transaction))
                        {
                            await commandUser.ExecuteNonQueryAsync().ConfigureAwait(false);
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
    public async Task GetUsername_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        //int reservationID = 3;
        var reservationID = await InsertReservationTestData();
        string userHash = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=";
        result = await _emailDAO.GetUsername(userHash);
        string email = "4sarahsantos@gmail.com";

        //Act
        timer.Start();
        result = await _emailDAO.GetUsername(userHash);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        string targetEmail = result.ValuesRead.Rows[0]["username"].ToString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (targetEmail != email)
        {
            result.HasError = true;
            result.ErrorMessage = $"{targetEmail} is not equal to {email}";
        }
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task GetUserReservationById_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        //Response result = new Response();
        var reservationID = await InsertResOnlyTestData();

        //Act
        timer.Start();
        var(reservation, result) = await _emailDAO.GetUserReservationByID(reservationID);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsNotNull(reservation);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SendConfirm_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var reservationID = await InsertResOnlyTestData();
        UserReservationsModel reservation = new UserReservationsModel
        {
            ReservationID = reservationID, 
            CompanyID = 9, 
            FloorPlanID = 8, 
            SpaceID = "SPACE022", 
            ReservationStartTime = new DateTime(2024, 6, 22, 9, 0, 0), 
            ReservationEndTime = new DateTime(2024, 6, 22, 11, 0, 0), 
            Status = ReservationStatus.Active,
            UserHash = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU="
        };

        //Act
        timer.Start();
        result = await _emailSender.SendConfirmation(reservation);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID).ConfigureAwait(false);
    }


    [TestMethod]
    public async Task SendConfirm_InvalidInputs_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        int reservationID = -1;
        UserReservationsModel reservation = new UserReservationsModel
        {
            ReservationID = reservationID, 
            CompanyID = 9, 
            FloorPlanID = 8, 
            SpaceID = "SPACE022", 
            ReservationStartTime = new DateTime(2024, 6, 22, 9, 0, 0), 
            ReservationEndTime = new DateTime(2024, 6, 22, 11, 0, 0), 
            Status = ReservationStatus.Active,
            UserHash = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU="
        };

         //Act
        timer.Start();
        result = await _emailSender.SendConfirmation(reservation);
        timer.Stop();

        //Assert
        Assert.IsTrue(result.HasError, "Expected SendConfirmation to fail with invalid input.");
        Assert.IsFalse(string.IsNullOrEmpty(result.ErrorMessage), "Expected an error message for invalid input.");
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        int resID = Convert.ToInt32(reservation.ReservationID);
        await CleanupTestData(resID).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SendConfirm_Timeout_Fail()
    {
        //Arrange
        
        var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(3000));
        var reservationID = await InsertResOnlyTestData();
        UserReservationsModel reservation = new UserReservationsModel
        {
            ReservationID = reservationID, 
            CompanyID = 9, 
            FloorPlanID = 8, 
            SpaceID = "SPACE022", 
            ReservationStartTime = new DateTime(2024, 6, 22, 9, 0, 0), 
            ReservationEndTime = new DateTime(2024, 6, 22, 11, 0, 0), 
            Status = ReservationStatus.Active,
            UserHash = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU="
        };

        //Act
        var response =  await _emailSender.SendConfirmation(reservation);
        Task<Response> operationTask = Task.FromResult(response);
        var completedTask = await Task.WhenAny(operationTask, timeoutTask);

        // Assert
        if (completedTask == operationTask)
        {
            // Operation completed before timeout, now it's safe to await it and check results
            response = await operationTask;

            // Assert the operation's success
            Assert.IsFalse(response.HasError, response.ErrorMessage);
        }
        else
        {
            // Fail the test if we hit the timeout
            Assert.Fail("The SendConfirmation operation timed out.");
        }

        //Cleanup
        await CleanupTestData(reservationID).ConfigureAwait(false);
    }

}
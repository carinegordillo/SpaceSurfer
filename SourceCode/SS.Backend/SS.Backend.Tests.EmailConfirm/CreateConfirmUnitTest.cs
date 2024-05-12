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
using SS.Backend.Services.LoggingService;

namespace SS.Backend.Tests.EmailConfirm;

[TestClass]
public class CreateConfirmUnitTest
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
    public async Task InsertConfirmInfo_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        var reservationID = await InsertResOnlyTestData();
        var response = new Response();
        var baseDirectory = AppContext.BaseDirectory;
        var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        var calendarFilePath = Path.Combine(projectRootDirectory, "CalendarFiles", "SSReservation.ics");
        string otp = string.Empty;
        string icsFile = string.Empty;
        byte[]? fileBytes = null;

        var infoResponse = await _emailDAO.GetReservationInfo(reservationID);
        if (!infoResponse.HasError && infoResponse.ValuesRead != null && infoResponse.ValuesRead.Rows.Count > 0)
        {
            DataRow row = infoResponse.ValuesRead.Rows[0];

            var getOtp = new GenOTP();
            otp = getOtp.generateOTP();
            if (otp == null)
            {
                response.ErrorMessage = "The otp is null";
            }

            var address = infoResponse.ValuesRead.Columns.Contains("CompanyAddress") ? row["CompanyAddress"].ToString() : null;
            var spaceID = infoResponse.ValuesRead.Columns.Contains("spaceID") ? row["spaceID"].ToString() : null;
            var companyName = infoResponse.ValuesRead.Columns.Contains("CompanyName") ? row["CompanyName"].ToString() : null;
            //extract and handle reservation date and time 
            var startTime = row.Table.Columns.Contains("reservationStartTime")? (DateTime?)DateTime.Parse(row["reservationStartTime"].ToString()) : null;
            var endTime = row.Table.Columns.Contains("reservationEndTime") ?(DateTime?)DateTime.Parse(row["reservationEndTime"].ToString()) : null;
            var date = startTime.Value.Date;
            
            if (address == null) response.ErrorMessage = "The 'address' data was not found.";
            if (spaceID == null) response.ErrorMessage = "The 'spaceID' data was not found.";
            if (companyName == null) response.ErrorMessage = "The 'CompanyName' data was not found.";
#pragma warning disable CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
            if (date == null) response.ErrorMessage = "The 'reservationDate' data was not found.";
#pragma warning restore CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
            if (startTime == null) response.ErrorMessage = "The 'reservationStartTime' data was not found.";
            if (endTime == null) response.ErrorMessage = "The 'reservationEndTime' data was not found.";


            //calendar ics creator
#pragma warning disable CS8601 // Possible null reference assignment.
            var reservationInfo = new ReservationInfo
            {
                filePath = calendarFilePath,
                eventName = "SpaceSurfer Reservation",
                dateTime = date,
                start = startTime,
                end = endTime,
                description = $"Reservation at: {companyName} \nReservationID: {reservationID} \nSpaceID: {spaceID}",
                location = address
            };
#pragma warning restore CS8601 // Possible null reference assignment.
            var calendarCreator = new CalendarCreator();
            icsFile = await calendarCreator.CreateCalendar(reservationInfo);
            if (icsFile == null)
            {
                response.ErrorMessage = "The ics file is null";
            }
            fileBytes = await File.ReadAllBytesAsync(icsFile);
        }

        //Act
        timer.Start();
        result = await _emailDAO.InsertConfirmationInfo(reservationID, otp, fileBytes);
        timer.Stop();

        //Assert
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsNotNull(fileBytes);
        Assert.IsNotNull(otp);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID).ConfigureAwait(false);

    }

    [TestMethod]
    public async Task CreateConfirm_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        var reservationID = await InsertResOnlyTestData();

        //Act
        timer.Start();
        var (icsFile, otp, html, result) = await _emailConfirm.CreateConfirmation(reservationID);
        //result = await _emailDAO.InsertConfirmationInfo(reservationID, otp, fileBytes);
        timer.Stop();

        //Assert
        Assert.IsNotNull(icsFile);
        Assert.IsNotNull(otp);
        Assert.IsNotNull(html);
        Assert.IsFalse(result.HasError, result.ErrorMessage);
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        await CleanupTestData(reservationID).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task CreateConfirm_InvalidInputs_Fail()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        int reservationID = -1;

        //Act
        timer.Start();
        var (icsFile, otp, html, result) = await _emailConfirm.CreateConfirmation(reservationID);
        timer.Stop();

        //Assert
        Assert.IsTrue(result.HasError, "Expected CreateConfirmation to fail with invalid input.");
        Assert.IsFalse(string.IsNullOrEmpty(result.ErrorMessage), "Expected an error message for invalid input.");
        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //Cleanup
        //await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task CreateConfirm_Timeout_Fail()
    {
        //Arrange
        var reservationID = await InsertResOnlyTestData();
        var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(3000));

        //Act
        var operationTask = _emailConfirm.CreateConfirmation(reservationID);
        var completedTask = await Task.WhenAny(operationTask, timeoutTask);

        // Assert
        if (completedTask == operationTask)
        {
            // Operation completed before timeout, now it's safe to await it and check results
            var (icsFile, otp, html, response) = await operationTask;

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
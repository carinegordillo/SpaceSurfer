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

                string sql1 = $"DELETE FROM dbo.ConfirmReservations WHERE [reservationID] = '3'";

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
    public async Task InsertConfirmInfo_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        Response result = new Response();
        int reservationID = 3;
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
        await CleanupTestData().ConfigureAwait(false);

    }

    [TestMethod]
    public async Task CreateConfirm_Success()
    {
        //Arrange
        Stopwatch timer = new Stopwatch();
        int reservationID = 3;

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
        await CleanupTestData().ConfigureAwait(false);
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
        await CleanupTestData().ConfigureAwait(false);
    }

    [TestMethod]
    public async Task CreateConfirm_Timeout_Fail()
    {
        //Arrange
        int reservationID = 3;
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
        await CleanupTestData().ConfigureAwait(false);
    }
}
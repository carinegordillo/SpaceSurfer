using SS.Backend.DataAccess;
using SS.Backend.Waitlist;
using SS.Backend.Services.EmailService;
using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Text;
using System.Data;
using System;

namespace SS.Backend.Tests.Waitlist;

[TestClass]
public class WaitlistUnitTest
{
    private Response result;
    private CustomSqlCommandBuilder builder;
    private ConfigService configService;
    private SqlDAO dao;
    private WaitlistService waitlistService;
    private ReservationCreatorService _ReservationCreatorService;
    private ReservationManagementRepository _reservationManagementRepository;
    private ReservationValidationService _reservationValidationService;
    private ReservationCreationManager _reservationCreationManager;
    private ReservationCancellationService _reservationCancellationService;

    [TestInitialize]
    public void TestInitialize()
    {
        result = new Response();
        builder = new CustomSqlCommandBuilder();

        var baseDirectory = AppContext.BaseDirectory;
        var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

        configService = new ConfigService(configFilePath);
        dao = new SqlDAO(configService);
        waitlistService = new WaitlistService(dao);

        _reservationManagementRepository = new ReservationManagementRepository(dao);
        _reservationValidationService = new ReservationValidationService(_reservationManagementRepository);
        _ReservationCreatorService = new ReservationCreatorService(_reservationManagementRepository, waitlistService);
        _reservationCreationManager = new ReservationCreationManager(_ReservationCreatorService, _reservationValidationService, waitlistService);
        _reservationCancellationService = new ReservationCancellationService(_reservationManagementRepository, waitlistService);
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        await CleanupTestData();
    }

    private async Task CleanupTestData()
    {
        try
        {
            string sql = "DELETE FROM Waitlist WHERE Username = 'testHash'";
            var cmd = new SqlCommand(sql);
            var response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM Waitlist WHERE Username = 'testHash2'";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM Waitlist WHERE Username = 'testHash3'";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM TestReservations";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during test cleanup: {ex}");
        }
    }

    [TestMethod]
    public async Task Waitlist_AddedAfterUserCreatesReservation_Pass()
    {
        // Arrange
        string userHash = "testHash";
        string tableName = "TestReservations";

        UserReservationsModel userReservationsModel = new UserReservationsModel
        {
            CompanyID = 1,
            FloorPlanID = 1,
            SpaceID = "OS-02",
            ReservationStartTime = new DateTime(2024, 02, 15, 14, 00, 00),
            ReservationEndTime = new DateTime(2024, 02, 15, 15, 00, 00),
            Status = ReservationStatus.Active,
            UserHash = userHash
        };

        // Act
        var response = await _ReservationCreatorService.CreateReservationWithAutoIDAsync(tableName, userReservationsModel);

        string sql = $"SELECT * FROM Waitlist WHERE Username = '{userHash}'";
        var onWaitlistCmd = new SqlCommand(sql);
        response = await dao.ReadSqlResult(onWaitlistCmd);

        int resId = Convert.ToInt32(response.ValuesRead?.Rows[0]?["ReservationID"]);
        bool onWaitlist = response.ValuesRead.Rows.Count > 0;

        int userPosition = await waitlistService.GetWaitlistPosition(userHash, resId);
        bool isPositionCorrect = false;
        if (userPosition == 0)
        {
            isPositionCorrect = true;
        }

        // Assert
        Assert.IsTrue(onWaitlist);
        Assert.IsTrue(isPositionCorrect);
        Assert.IsFalse(response.HasError);
    }

    [TestMethod]
    public async Task Waitlist_UserJoinsWaitlist_Pass()
    {
        // Arrange
        string userHash = "testHash";
        string userHash2 = "testHash2";
        string tableName = "TestReservations";

        UserReservationsModel userReservationsModel = new UserReservationsModel
        {
            CompanyID = 1,
            FloorPlanID = 1,
            SpaceID = "OS-02",
            ReservationStartTime = new DateTime(2024, 02, 15, 14, 00, 00),
            ReservationEndTime = new DateTime(2024, 02, 15, 15, 00, 00),
            Status = ReservationStatus.Active,
            UserHash = userHash2
        };

        string sql = $"INSERT INTO TestReservations VALUES ({userReservationsModel.CompanyID},{userReservationsModel.FloorPlanID},'{userReservationsModel.SpaceID}','{userReservationsModel.ReservationStartTime}','{userReservationsModel.ReservationEndTime}','{userReservationsModel.Status}','{userHash}')";
        var cmd = new SqlCommand(sql);
        var response = await dao.SqlRowsAffected(cmd);
        Console.WriteLine("Command: " + cmd.CommandText);

        // Act
        response = await _reservationCreationManager.AddToWaitlist(tableName, userReservationsModel);

        sql = $"SELECT * FROM Waitlist WHERE Username = '{userHash2}'";
        var onWaitlistCmd = new SqlCommand(sql);
        response = await dao.ReadSqlResult(onWaitlistCmd);
        bool onWaitlist = response.ValuesRead.Rows.Count > 0;

        int resId = Convert.ToInt32(response.ValuesRead?.Rows[0]?["ReservationID"]);

        int userPosition = await waitlistService.GetWaitlistPosition(userHash2, resId);
        bool isPositionCorrect = false;
        if (userPosition == 0)
        {
            isPositionCorrect = true;
        }

        // Assert
        Assert.IsTrue(onWaitlist);
        Assert.IsTrue(isPositionCorrect);
        Assert.IsFalse(response.HasError);
    }

    [TestMethod]
    public async Task Waitlist_LeaveWaitlist_Pass()
    {
        // Arrange
        string userHash = "testHash";
        string tableName = "TestReservations";

        string sql = $"INSERT INTO Waitlist VALUES ('{userHash}', 777, 1)";
        var cmd = new SqlCommand(sql);
        var response = await dao.SqlRowsAffected(cmd);

        // Act
        await waitlistService.UpdateWaitlist_WaitlistedUserLeft(777, 1);

        sql = $"SELECT * FROM Waitlist WHERE Username = '{userHash}'";
        cmd = new SqlCommand(sql);
        response = await dao.ReadSqlResult(cmd);
        bool empty = false;
        if (response.ValuesRead == null)
        {
            empty = true;
        }

        // Assert
        Assert.IsTrue(empty);
        Assert.IsTrue(response.HasError);
    }

    [TestMethod]
    public async Task Waitlist_PositionsUpdate_UserCancelsReservation_Pass()
    {
        // Arrange
        string userHash = "testHash";
        string userHash2 = "testHash2";
        string userHash3 = "testHash3";
        string tableName = "TestReservations";

        UserReservationsModel userReservationsModel = new UserReservationsModel
        {
            CompanyID = 1,
            FloorPlanID = 1,
            SpaceID = "OS-02",
            ReservationStartTime = new DateTime(2024, 02, 15, 14, 00, 00),
            ReservationEndTime = new DateTime(2024, 02, 15, 15, 00, 00),
            Status = ReservationStatus.Active,
            UserHash = userHash2
        };

        string sql = $"INSERT INTO TestReservations VALUES ({userReservationsModel.CompanyID},{userReservationsModel.FloorPlanID},'{userReservationsModel.SpaceID}','{userReservationsModel.ReservationStartTime}','{userReservationsModel.ReservationEndTime}','{userReservationsModel.Status}','{userHash}')";
        var cmd = new SqlCommand(sql);
        var response = await dao.SqlRowsAffected(cmd);

        int resId = await waitlistService.GetReservationID(tableName, 1, 1, "OS-02", userReservationsModel.ReservationStartTime, userReservationsModel.ReservationEndTime);
        
        sql = $"INSERT INTO Waitlist VALUES ('{userHash}', {resId}, 0)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = $"INSERT INTO Waitlist VALUES ('{userHash2}', {resId}, 1)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = $"INSERT INTO Waitlist VALUES ('{userHash3}', {resId}, 2)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);

        //// Act
        response = await _reservationCancellationService.CancelReservationAsync(tableName, resId);

        sql = $"SELECT * FROM Waitlist WHERE Username = '{userHash}'";
        cmd = new SqlCommand(sql);
        response = await dao.ReadSqlResult(cmd);
        bool empty = false;
        if (response.ValuesRead == null)
        {
            empty = true;
        }

        int userPosition = await waitlistService.GetWaitlistPosition(userHash2, resId);
        int userPosition2 = await waitlistService.GetWaitlistPosition(userHash3, resId);

        bool isPositionCorrect = false;
        if (userPosition == 1 && userPosition2 == 2)
        {
            isPositionCorrect = true;
        }

        // Assert
        Assert.IsTrue(empty);
        Assert.IsTrue(isPositionCorrect);
        Assert.IsTrue(response.HasError);
    }

    [TestMethod]
    public async Task Waitlist_PositionsUpdate_WaitlistedUserLeaves_Pass()
    {
        // Arrange
        string userHash = "testHash";
        string userHash2 = "testHash2";
        string userHash3 = "testHash3";
        string tableName = "TestReservations";

        string sql = $"INSERT INTO Waitlist VALUES ('{userHash}', 777, 0)";
        var cmd = new SqlCommand(sql);
        var response = await dao.SqlRowsAffected(cmd);
        sql = $"INSERT INTO Waitlist VALUES ('{userHash2}', 777, 1)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = $"INSERT INTO Waitlist VALUES ('{userHash3}', 777, 2)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);

        // Act
        await waitlistService.UpdateWaitlist_WaitlistedUserLeft(777, 1);

        sql = $"SELECT * FROM Waitlist WHERE Username = '{userHash2}'";
        cmd = new SqlCommand(sql);
        response = await dao.ReadSqlResult(cmd);
        bool empty = false;
        if (response.ValuesRead == null)
        {
            empty = true;
        }

        int userPosition = await waitlistService.GetWaitlistPosition(userHash, 777);
        int userPosition2 = await waitlistService.GetWaitlistPosition(userHash3, 777);

        bool isPositionCorrect = false;
        if (userPosition == 0 && userPosition2 == 1)
        {
            isPositionCorrect = true;
        }

        // Assert
        Assert.IsTrue(empty);
        Assert.IsTrue(isPositionCorrect);
        Assert.IsTrue(response.HasError);
    }

    [TestMethod]
    public async Task Waitlist_AlreadyOnWaitlist_Pass()
    {
        // Arrange
        string userHash = "testHash";
        string userHash2 = "testHash2";
        string tableName = "TestReservations";

        string sql = $"INSERT INTO TestReservations VALUES (1,1,'OS-02','2024-04-15 14:00:00.000','2024-04-15 15:00:00.000','Active','{userHash}')";
        var cmd = new SqlCommand(sql);
        var response = await dao.SqlRowsAffected(cmd);

        DateTime stime = new DateTime(2024, 02, 15, 14, 00, 00);
        DateTime etime = new DateTime(2024, 02, 15, 15, 00, 00);
        int resId = await waitlistService.GetReservationID(tableName, 1, 1, "OS-02", stime, etime);

        sql = $"INSERT INTO Waitlist VALUES ('{userHash}', {resId}, 0)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = $"INSERT INTO Waitlist VALUES ('{userHash2}', {resId}, 1)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);

        UserReservationsModel userReservationsModel = new UserReservationsModel
        {
            CompanyID = 1,
            FloorPlanID = 1,
            SpaceID = "OS-02",
            ReservationStartTime = new DateTime(2024, 02, 15, 14, 00, 00),
            ReservationEndTime = new DateTime(2024, 02, 15, 15, 00, 00),
            Status = ReservationStatus.Active,
            UserHash = userHash2
        };

        // Act
        response = await _reservationCreationManager.AddToWaitlist(tableName, userReservationsModel);

        sql = $"SELECT * FROM Waitlist WHERE Username = '{userHash2}'";
        cmd = new SqlCommand(sql);
        response = await dao.ReadSqlResult(cmd);

        bool noDuplicate = response.ValuesRead.Rows.Count == 1;

        int userPosition = await waitlistService.GetWaitlistPosition(userHash2, resId);
        bool isPositionCorrect = false;
        if (userPosition == 1)
        {
            isPositionCorrect = true;
        }

        // Assert
        Assert.IsTrue(noDuplicate);
        Assert.IsTrue(isPositionCorrect);
        Assert.IsFalse(response.HasError);
    }
}
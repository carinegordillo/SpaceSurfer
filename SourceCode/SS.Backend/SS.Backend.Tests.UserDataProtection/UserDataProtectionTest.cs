using SS.Backend.DataAccess;
using SS.Backend.Services.EmailService;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;
using System.Data;
using System;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using SS.Backend.Services.DeletingService;
using SS.Backend.UserDataProtection;

namespace SS.Backend.Tests.UserDataProtection;

[TestClass]
public class UserDataProtectionTest
{
    private Response result;
    private CustomSqlCommandBuilder builder;
    private ConfigService configService;
    private GenOTP genotp;
    private Hashing hasher;
    private SqlDAO dao;
    private UserProtectionService service;
    private AccountDeletion accountDeletion;

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
        genotp = new GenOTP();
        hasher = new Hashing();
        service = new UserProtectionService(dao, genotp, hasher);
        accountDeletion = new AccountDeletion(new DatabaseHelper(dao));
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
            string sql = "DELETE FROM dbo.userHash WHERE hashedUsername = 'asdfasdfasdf'";
            var cmd = new SqlCommand(sql);
            var response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM dbo.userAccount WHERE username = 'test@gmail.com'";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM dbo.userProfile WHERE hashedUsername = 'asdfasdfasdf'";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM dbo.activeAccount WHERE hashedUsername = 'asdfasdfasdf'";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM dbo.reservations WHERE userHash = 'asdfasdfasdf'";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM dbo.companyFloor WHERE companyID = 777";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM dbo.companyFloorSpaces WHERE companyID = 777";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM dbo.companyProfile WHERE hashedUsername = 'asdfasdfasdf'";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM dbo.OTP WHERE Username = 'asdfasdfasdf'";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM dbo.Waitlist WHERE Username = 'asdfasdfasdf'";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
            sql = "DELETE FROM dbo.Logs WHERE Username = 'asdfasdfasdf'";
            cmd = new SqlCommand(sql);
            response = await dao.SqlRowsAffected(cmd);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during test cleanup: {ex}");
        }
    }

    public async Task InsertQueries_Manager()
    {
        string sql = "INSERT INTO dbo.userHash (hashedUsername, username, user_id) VALUES ('asdfasdfasdf', 'test@gmail.com', 999)";
        var cmd = new SqlCommand(sql);
        var response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.userAccount (user_id, username, birthDate, companyID) VALUES (999, 'test@gmail.com', '2002-04-05', 777)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.userProfile (hashedUsername, firstName, lastName, backupEmail, appRole) VALUES ('asdfasdfasdf', 'test', 'user', 'backupTest@gmail.com', 2)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.activeAccount (hashedUsername, isActive) VALUES ('asdfasdfasdf', 'Yes')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.reservations (reservationID, companyID, floorPlanID, spaceID, reservationStartTime, reservationEndTime, status, userHash) VALUES (9999, 777, 1, 'TS-01', '2024-04-15 14:21:00.000', '2024-04-15 15:21:00.000', 'Passed', 'asdfasdfasdf')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.companyFloor (floorPlanID, companyID, floorPlanName, floorPlanImage) VALUES (1, 777, 'Test Floor', '0xFFD8FFE000104A46')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.companyFloorSpaces (spaceID, floorPlanID, companyID, timeLimit) VALUES ('TS-01', 1, 777, 2)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.companyProfile (companyID, hashedUsername, companyName, address, openingHours, closingHours, daysOpen) VALUES (777, 'asdfasdfasdf', 'Test Company', '123 Test Rd', '08:00:00.0000000', '22:00:00.0000000', 'Monday, Tuesday, Wednesday')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.OTP (Username, OTP, Salt, Timestamp) VALUES ('asdfasdfasdf', 'lfjdgjgkfdjksfjdkgfd', 'kdkfjdfkdk', '2024-05-11 09:53:34.670')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.Waitlist (Username, ReservationID, Position) VALUES ('asdfasdfasdf', 9999, 0)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.Logs (Log_ID, Timestamp, Log Level, Username, Category, Description) VALUES (9999, '2024-04-05 02:33:20.480', 'Info', 'asdfasdfasdf', 'Data Store', 'Testing')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
    }

    public async Task InsertQueries_General()
    {
        string sql = "INSERT INTO dbo.userHash (hashedUsername, username, user_id) VALUES ('asdfasdfasdf', 'test@gmail.com', 999)";
        var cmd = new SqlCommand(sql);
        var response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.userAccount (user_id, username, birthDate, companyID) VALUES (999, 'test@gmail.com', '2002-04-05', 'NULL')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.userProfile (hashedUsername, firstName, lastName, backupEmail, appRole) VALUES ('asdfasdfasdf', 'test', 'user', 'backupTest@gmail.com', 5)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.activeAccount (hashedUsername, isActive) VALUES ('asdfasdfasdf', 'Yes')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.reservations (reservationID, companyID, floorPlanID, spaceID, reservationStartTime, reservationEndTime, status, userHash) VALUES (9999, 777, 1, 'TS-01', '2024-04-15 14:21:00.000', '2024-04-15 15:21:00.000', 'Passed', 'asdfasdfasdf')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.OTP (Username, OTP, Salt, Timestamp) VALUES ('asdfasdfasdf', 'lfjdgjgkfdjksfjdkgfd', 'kdkfjdfkdk', '2024-05-11 09:53:34.670')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.Waitlist (Username, ReservationID, Position) VALUES ('asdfasdfasdf', 9999, 0)";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
        sql = "INSERT INTO dbo.Logs (Log_ID, Timestamp, Log Level, Username, Category, Description) VALUES (9999, '2024-04-05 02:33:20.480', 'Info', 'asdfasdfasdf', 'Data Store', 'Testing')";
        cmd = new SqlCommand(sql);
        response = await dao.SqlRowsAffected(cmd);
    }

    public async Task<bool> CheckUserDataMatches_Manager(UserDataModel userData)
    {
        Dictionary<string, object> correctInfo = new Dictionary<string, object>
        {
            {"Username", "test@gmail.com"},
            {"FirstName", "test"},
            {"LastName", "user"},
            {"BackupEmail", "backupTest@gmail.com"},
            {"AppRole", 2},
            {"IsActive", "Yes"},
            {"OTP", "lfjdgjgkfdjksfjdkgfd"}
        };

        foreach (var kvp in correctInfo)
        {
            var propertyName = kvp.Key;
            var correctValue = kvp.Value;
            var userValue = userData.GetType().GetProperty(propertyName)?.GetValue(userData);

            if (!correctValue.Equals(userValue))
            {
                Console.WriteLine($"{propertyName} doesn't match");
                return false;
            }
        }

        return true;
    }

    public async Task<bool> CheckUserDataMatches_General(UserDataModel userData)
    {
        Dictionary<string, object> correctInfo = new Dictionary<string, object>
    {
        {"Username", "test@gmail.com"},
        {"FirstName", "test"},
        {"LastName", "user"},
        {"BackupEmail", "backupTest@gmail.com"},
        {"AppRole", 5},
        {"IsActive", "Yes"},
        {"OTP", "lfjdgjgkfdjksfjdkgfd"}
    };

        foreach (var kvp in correctInfo)
        {
            var propertyName = kvp.Key;
            var correctValue = kvp.Value;
            var userValue = userData.GetType().GetProperty(propertyName)?.GetValue(userData);

            if (!correctValue.Equals(userValue))
            {
                Console.WriteLine($"{propertyName} doesn't match");
                return false;
            }
        }

        return true;
    }

    public async Task<bool> CheckTables()
    {
        var tablesToCheck = new Dictionary<string, string>
    {
        {"Logs", "SELECT * FROM dbo.Logs WHERE Username = 'asdfasdfasdf'"},
        {"userHash", "SELECT * FROM dbo.userHash WHERE hashedUsername = 'asdfasdfasdf'"},
        {"userAccount", "SELECT * FROM dbo.userAccount WHERE username = 'test@gmail.com'"},
        {"userProfile", "SELECT * FROM dbo.userProfile WHERE hashedUsername = 'asdfasdfasdf'"},
        {"activeAccount", "SELECT * FROM dbo.activeAccount WHERE hashedUsername = 'asdfasdfasdf'"},
        {"reservations", "SELECT * FROM dbo.reservations WHERE userHash = 'asdfasdfasdf'"},
        {"companyFloor", "SELECT * FROM dbo.companyFloor WHERE companyID = 777"},
        {"companyFloorSpaces", "SELECT * FROM dbo.companyFloorSpaces WHERE companyID = 777"},
        {"companyProfile", "SELECT * FROM dbo.companyProfile WHERE hashedUsername = 'asdfasdfasdf'"},
        {"OTP", "SELECT * FROM dbo.OTP WHERE Username = 'asdfasdfasdf'"},
        {"Waitlist", "SELECT * FROM dbo.Waitlist WHERE Username = 'asdfasdfasdf'"}
    };

        foreach (var table in tablesToCheck)
        {
            var cmd = new SqlCommand(table.Value);
            var response = await dao.SqlRowsAffected(cmd);
            if (result.ValuesRead?.Rows.Count > 0)
            {
                Console.WriteLine($"Failed to retrieve data from {table.Key} table");
                return false;
            }
        }

        return true;
    }

    [TestMethod]
    public async Task AccessData_GeneralUser_Should_Pass()
    {
        // Arrange
        UserDataModel userData = new UserDataModel();
        string userHash = "asdfasdfasdf";
        await InsertQueries_General();

        // Act
        userData = await service.accessData_GeneralUser(userHash);
        bool infoMatches = await CheckUserDataMatches_General(userData);

        // Assert
        Assert.IsTrue(infoMatches);
    }

    [TestMethod]
    public async Task AccessData_Manager_Should_Pass()
    {
        // Arrange
        UserDataModel userData = new UserDataModel();
        string userHash = "asdfasdfasdf";
        await InsertQueries_Manager();

        // Act
        userData = await service.accessData_GeneralUser(userHash);
        bool infoMatches = await CheckUserDataMatches_Manager(userData);

        // Assert
        Assert.IsTrue(infoMatches);
    }

    [TestMethod]
    public async Task DeleteData_GeneralUser_Should_Pass()
    {
        // Arrange
        UserDataModel userData = new UserDataModel();
        string userHash = "asdfasdfasdf";
        await InsertQueries_General();

        // Act
        userData = await service.accessData_GeneralUser(userHash);
        bool infoMatches = await CheckUserDataMatches_General(userData);
        result = await accountDeletion.DeleteAccount(userHash);
        await service.deleteData(userHash);
        bool userDeleted = await CheckTables();

        // Assert
        Assert.IsTrue(infoMatches);
        Assert.IsTrue(userDeleted);
        Assert.IsFalse(result.HasError);
    }

    [TestMethod]
    public async Task DeleteData_Manager_Should_Pass()
    {
        // Arrange
        UserDataModel userData = new UserDataModel();
        string userHash = "asdfasdfasdf";
        await InsertQueries_Manager();

        // Act
        userData = await service.accessData_Manager(userHash);
        bool infoMatches = await CheckUserDataMatches_Manager(userData);
        result = await accountDeletion.DeleteAccount(userHash);
        await service.deleteData(userHash);
        bool userDeleted = await CheckTables();

        // Assert
        Assert.IsTrue(infoMatches);
        Assert.IsTrue(userDeleted);
        Assert.IsFalse(result.HasError);
    }
}
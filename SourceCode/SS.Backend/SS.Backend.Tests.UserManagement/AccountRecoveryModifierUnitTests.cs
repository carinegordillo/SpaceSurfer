
using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.UserManagement;
using Microsoft.Data.SqlClient;
using System.Data;


[TestClass]
public class AccountRecoveryModifierUnitTests
{
    private AccountRecoveryModifier _accountRecoveryModifier; 
    private IUserManagementDao _userManagementDao;

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
        _userManagementDao = new UserManagementDao(_sqlDao);

        _accountRecoveryModifier = new AccountRecoveryModifier(_userManagementDao);

        
    }

    // [TestMethod]
    // public async Task NonExistentResolveRequest_Fails()
    // {
    //     // Arrange
    //     var userHash = "testUserHash1";
    //     var resolveStatus = "accepted";
        
    //     // Act
    //     var result = await _accountRecoveryModifier.ResolveRequest(userHash, resolveStatus);

    //     // Assert
    //     Assert.IsTrue(result.HasError);

    //     // Verify the update in the database
    //     var response = await _userManagementDao.readTableWhere("userHash", userHash, "dbo.userRequests");
    //     Assert.IsTrue(response.HasError);
    // }


    [TestMethod]
    public async Task PendingRequest_UpdatesStatusToPending()
    {
        // Arrange
        var userHash = "testUserHash5";
        
        // Act
        var result = await _accountRecoveryModifier.PendingRequest(userHash);

        // Assert
        Assert.IsFalse(result.HasError);

        // Verify the update in the database
        var response = await _userManagementDao.readTableWhere("hashedUsername", userHash, "dbo.activeAccount");
        Assert.IsFalse(response.HasError);
        foreach (DataRow row in response.ValuesRead.Rows)
        {
            Assert.AreEqual("pending", row["isActive"].ToString().Trim());
        }
    }

    [TestMethod]
    public async Task ResolveRequest_UpdatesStatusCorrectly()
    {
        // Arrange
        var userHash = "testUserHash4";
        var resolveStatus = "accepted";
        
        // Act
        var result = await _accountRecoveryModifier.ResolveRequest(userHash, resolveStatus);

        // Assert
        Assert.IsFalse(result.HasError);

        // Verify the update in the database
        var response = await _userManagementDao.readTableWhere("userHash", userHash, "dbo.userRequests");
        Assert.IsFalse(response.HasError);
        foreach (DataRow row in response.ValuesRead.Rows)
        {
            Assert.AreEqual("accepted", row["status"].ToString().Trim());
        }
    }

    



    [TestMethod]
    public async Task EnabledAccount_UpdatesStatusToEnabled()
    {
        // Arrange
        var userHash = "testUserHash1";
      
        // Act
        var result = await _accountRecoveryModifier.EnableAccount(userHash);



        var response_activeAccount = await _userManagementDao.readTableWhere("hashedUsername", userHash, "dbo.activeAccount");

        Assert.IsFalse(response_activeAccount.HasError);
        foreach (DataRow row in response_activeAccount.ValuesRead.Rows)
        {
            Assert.AreEqual(row["isActive"], "yes");
        }
        
    }

    [TestMethod]
    public async Task DiabledAccount_UpdatesStatusToEnabled()
    {
        // Arrange
        var userHash = "testUserHash2";
      
        // Act
        var result = await _accountRecoveryModifier.EnableAccount(userHash);

        // Assert

        Assert.IsFalse(result.HasError);


        var response_activeAccount = await _userManagementDao.readTableWhere("hashedUsername", userHash, "dbo.activeAccount");

        Assert.IsFalse(response_activeAccount.HasError);
        foreach (DataRow row in response_activeAccount.ValuesRead.Rows)
        {
            Assert.AreEqual(row["isActive"], "yes");
        }

        var response_userRequests = await _userManagementDao.readTableWhere("userHash", userHash, "dbo.userRequests");

        Console.WriteLine(response_userRequests.ErrorMessage);

        Assert.IsFalse(response_userRequests.HasError);
        foreach (DataRow row in response_userRequests.ValuesRead.Rows)
        {
            Assert.AreEqual(row["status"].ToString().Trim(), "accepted");
        }
    }


    [TestMethod]
    public async Task ReadUserPendingRequests_RetrievesPendingRequestsSuccessfully()
    {
        // Act
        var response = await _accountRecoveryModifier.ReadUserPendingRequests();

        // Assert
        Assert.IsFalse(response.HasError);
        Assert.IsTrue(response.ValuesRead.Rows.Count > 0, "No pending requests found.");
        foreach (DataRow row in response.ValuesRead.Rows)
        {
            Assert.AreEqual("Pending", row["status"].ToString().Trim());
        }
    }









}





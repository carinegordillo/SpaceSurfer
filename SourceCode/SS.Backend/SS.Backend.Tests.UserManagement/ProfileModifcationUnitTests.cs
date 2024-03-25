
using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.UserManagement;
using Microsoft.Data.SqlClient;
using System.Data;


[TestClass]
public class ProfileModifierUnitTests
{
    private ProfileModifier _profileModifier;
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
        _profileModifier = new ProfileModifier(_userManagementDao);
        
    }

    [TestMethod]
    public async Task ModifyFirstName_ReturnsSuccess_WithValidInput()
    {
        // Arrange
        var hashedUsername = "testUserHash1";
        var newFirstName = "testUserHash1NewFirstName";

        // Act
        var response = await _profileModifier.ModifyFirstName(hashedUsername, newFirstName);

        // Assert
        Assert.IsFalse(response.HasError);

        response = await _userManagementDao.readTableWhere("hashedUsername", hashedUsername, "dbo.userProfile");
        
        Assert.IsFalse(response.HasError);
        foreach (DataRow row in response.ValuesRead.Rows)
        {
            Assert.AreEqual(row["firstName"], newFirstName);
        }
    
    }

    [TestMethod]
    public async Task ModifyLastName_ReturnsSuccess_WithValidInput()
    {
        // Arrange
        var hashedUsername = "testUserHash1";
        var newLastName = "testUserHash2NewLastName";

        // Act
        var response = await _profileModifier.ModifyLastName(hashedUsername, newLastName);

        // Assert
        Assert.IsFalse(response.HasError);


        response = await _userManagementDao.readTableWhere("hashedUsername", hashedUsername, "dbo.userProfile");
        
        Assert.IsFalse(response.HasError);
        foreach (DataRow row in response.ValuesRead.Rows)
        {
            Console.WriteLine(row["lastName"]);
            Assert.AreEqual(row["lastName"], newLastName);
        }
        
    }

    [TestMethod]
    public async Task ModifyBackupEmail_ReturnsSuccess_WithValidInput()
    {
        // Arrange
        var hashedUsername = "testUserHash4";
        var newbackupEmail = "testUserHash1NewBackupEmail";

        // Act
        var response = await _profileModifier.ModifyBackupEmail(hashedUsername, newbackupEmail);
        Console.WriteLine(response.ErrorMessage);
        // Assert
        Assert.IsFalse(response.HasError);

        var response2 = await  _userManagementDao.readTableWhere("hashedUsername", hashedUsername, "dbo.userProfile");
        
        foreach (DataRow row in response2.ValuesRead.Rows)
        {
            Console.WriteLine(row["backupEmail"]);
            Assert.AreEqual(row["backupEmail"], newbackupEmail);
        }
        Assert.IsFalse(response2.HasError);
    }

    [TestMethod]
    public async Task GetUserProfile_ReturnsUserProfile_WithValidUsername()
    {
        // Arrange
        var hashed  = "testUserHash1";

        // Act
        var response = await _profileModifier.getUserProfile(hashed);

        // Assert
        Assert.IsFalse(response.HasError);
        foreach (DataRow row in response.ValuesRead.Rows)
        {
            Assert.AreEqual(row["hashedUsername"], hashed);
        }
    }

    [TestMethod]
    public async Task ModifyFirstName_InvalidUserHash_ShouldFail()
    {
        // Arrange
        var invalidUserHash = "invalidUserHash";
        var newFirstName = "NewFirstName";

        // Act
        var response = await _profileModifier.ModifyFirstName(invalidUserHash, newFirstName);

        // Assert
        Assert.IsTrue(response.HasError);
        Assert.IsTrue(response.RowsAffected == 0);
    }



}



using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.UserManagement;
using Microsoft.Data.SqlClient;
using SS.Backend.Services.LoggingService;
using System.Data;
using SS.Backend.SharedNamespace;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ProfileModifierUnitTests
{
    private ProfileModifier _profileModifier;
    private IUserManagementDao _userManagementDao; 
    private SqlDAO _sqlDao;
    private ConfigService _configService;
    private ILogTarget _logTarget;
    private ILogger _logger;

    string hashedUsername = "Yu86Ho6KDmtOeP687I/AHNE4rhxoCzZDs9v/Mpe+SZw=";

    [TestInitialize]
    public void Setup()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
        _configService = new ConfigService(configFilePath);
        _sqlDao = new SqlDAO(_configService);
        _userManagementDao = new UserManagementDao(_sqlDao);
        _logger = new Logger(_logTarget);
        _profileModifier = new ProfileModifier(_userManagementDao, _logger);
    }

    [TestMethod]
    public async Task ModifyProfile_ReturnsSuccess_WithValidFirstName()
    {
        // Arrange
        var userProfile = new EditableUserProfile
        {
            username = hashedUsername,
            firstname = "NewFirstName"
        };

        // Act
        var response = await _profileModifier.ModifyProfile(userProfile);

        // Assert
        Assert.IsFalse(response.HasError);

        response = await _userManagementDao.readTableWhere("hashedUsername", hashedUsername, "dbo.userProfile");

        Assert.IsFalse(response.HasError);
        foreach (DataRow row in response.ValuesRead.Rows)
        {
            Assert.AreEqual(row["firstName"], userProfile.firstname);
        }
    }

    [TestMethod]
    public async Task ModifyProfile_ReturnsSuccess_WithValidLastName()
    {
        // Arrange
        var userProfile = new EditableUserProfile
        {
            username = hashedUsername,
            lastname = "NewLastName"
        };

        // Act
        var response = await _profileModifier.ModifyProfile(userProfile);

        // Assert
        Assert.IsFalse(response.HasError);

        response = await _userManagementDao.readTableWhere("hashedUsername", hashedUsername, "dbo.userProfile");

        Assert.IsFalse(response.HasError);
        foreach (DataRow row in response.ValuesRead.Rows)
        {
            Assert.AreEqual(row["lastName"], userProfile.lastname);
        }
    }

    [TestMethod]
    public async Task GetUserProfile_ReturnsUserProfile_WithValidUsername()
    {
        // Arrange

        // Act
        var response = await _profileModifier.getUserProfile(hashedUsername);

        // Assert
        Assert.IsFalse(response.HasError);
        foreach (DataRow row in response.ValuesRead.Rows)
        {
            Assert.AreEqual(row["hashedUsername"], hashedUsername);
        }
    }

    [TestMethod]
    public async Task ModifyProfile_InvalidUserHash_ShouldFail()
    {
        // Arrange
        var userProfile = new EditableUserProfile
        {
            username = "invalidUserHash",
            firstname = "NewFirstName"
        };

        // Act
        var response = await _profileModifier.ModifyProfile(userProfile);

        // Assert
        Assert.IsTrue(response.HasError);
        Assert.AreEqual(response.RowsAffected, 0);
    }
}

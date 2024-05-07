
using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.UserManagement;
using Microsoft.Data.SqlClient;

namespace SS.Backend.Tests.UserManagement;

    [TestClass]
    public class UserManagementDaoUnitTests
    {
        private UserManagementDao _userManagementDao;
        private SqlDAO _sqlDao;
        private ConfigService _configService;

        string userHash1 = "Yu86Ho6KDmtOeP687I/AHNE4rhxoCzZDs9v/Mpe+SZw=";
        string userHash2 = "MaKM/H0KYHLGJPn4alLS1BpbvakoB3RjXLmbbJI4PE4=";
        
        
        [TestInitialize]
        public void Setup()
        {

            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);

            _userManagementDao = new UserManagementDao(_sqlDao);
        }

        

        [TestMethod]
        public async Task GeneralModifier_Pass()
        {
            
            
            // Arrange
            var column = "hashedUsername";  
            var value = userHash1;
            var columnToModify = "isActive";
            var newValue = "no";
            var tableName = "dbo.activeAccount";

            // Act
            var response = await _userManagementDao.GeneralModifier(column, value, columnToModify, newValue, tableName);
            
            // Assert
            Assert.IsFalse(response.HasError);
        }

        public async Task GeneralModifier_Fail()
        {
            // Arrange
            var column = "hashedUsername";  
            var value = "nonExistentUsername";
            var columnToModify = "isActive";
            var newValue = "no";
            var tableName = "dbo.activeAccount";

            // Act
            var response = await _userManagementDao.GeneralModifier(column, value, columnToModify, newValue, tableName);
            
            // Assert
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public async Task GeneralModifier_InvalidTable()
        {
            // Arrange
            // Arrange
            var column = "hashedUsername";  
            var value = userHash1;
            var columnToModify = "isActive";
            var newValue = "no";
            var tableName = "nonExistentTable";

            // Act
            var response = await _userManagementDao.GeneralModifier(column, value, columnToModify, newValue, tableName);
            
            // Assert
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public async Task GeneralModifier_InvalidColumn()
        {
            // Arrange
            // Arrange
            var column = "nonExistentColumn";  
            var value = userHash1;
            var columnToModify = "isActive";
            var newValue = "no";
            var tableName = "dbo.activeAccount";

            // Act
            var response = await _userManagementDao.GeneralModifier(column, value, columnToModify, newValue, tableName);
            
            // Assert
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public async Task ReadUserTable_ReturnsSuccess_WithValidTableName()
        {
            // Arrange
            var tableName = "dbo.userProfile";

            // Act
            var response = await _userManagementDao.ReadUserTable(tableName);

            // Assert
            Assert.IsFalse(response.HasError);
            Assert.IsTrue(response.ErrorMessage.Contains("command successful"));
        }


        [TestMethod]
        public async Task CreateAccountRecoveryRequest_AddsRequestSuccessfully()
        {
            // Arrange
            var userRequest = new UserRequestModel
            {
                UserHash = userHash2,
                RequestDate = DateTime.UtcNow,
                Status = "Pending",
                RequestType = "Recovery",
                AdditionalInformation = "this is a test from USerManagementDaoUnitTest.cs"
            };

            var tableName = "dbo.userRequests";

            // Act
            var response = await _userManagementDao.createAccountRecoveryRequest(userRequest, tableName);

            // Assert
            Assert.IsFalse(response.HasError);
            Assert.IsTrue(response.ErrorMessage.Contains("command successful"));
        }


        [TestMethod]
        public async Task ReadTableWhere_ReturnsCorrectData_WithValidCondition()
        {
             // Arrange
            var whereClause = "hashedUsername";
            var whereClauseVal = userHash2;
            var tableName = "dbo.userProfile";

            // Act
            var response = await _userManagementDao.readTableWhere(whereClause, whereClauseVal, tableName);

            // Assert
            Assert.IsFalse(response.HasError);
            Assert.IsTrue(response.ErrorMessage.Contains("command successful"));


        }



        [TestCleanup]
        public void Cleanup()
        {
            // Define the SQL command to delete test data
            var commandText = "DELETE FROM dbo.userRequests WHERE userHash = 'testUserHash'";
            
            // Create a SQL command object
            SqlCommand sqlCommand = new SqlCommand(commandText);
             _sqlDao.SqlRowsAffected(sqlCommand);
        }
}


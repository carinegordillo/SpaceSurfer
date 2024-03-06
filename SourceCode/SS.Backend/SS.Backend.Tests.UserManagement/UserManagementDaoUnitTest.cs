
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
        private SqlCommand _command;

        
        
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
            var value = "user8hash";
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
            var column = "hashedUsername";  
            var value = "testUsername";
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
            var column = "nonExistentColumn";  
            var value = "testUsername";
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
                UserHash = "testUserHash",
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
            var whereClauseVal = "user7hash";
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
            using (var connection = new SqlConnection(_configService.GetConnectionString()))
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }




}


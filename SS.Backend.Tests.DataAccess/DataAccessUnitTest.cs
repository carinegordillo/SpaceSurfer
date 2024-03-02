using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Tests.DataAccess
{
    [TestClass]
    public class DataAccessUnitTest
    {
        private SqlDAO? dao;

        [TestInitialize]
        public void TestInitialize()
        {
            Response result = new Response();
            var builder = new CustomSqlCommandBuilder();
            string configFilePath = "/Users/sarahsantos/SpaceSurfer/SourceCode/SS.Backend/Configs/config.local.txt";
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO dao = new SqlDAO(configService);


        }

        private async Task CleanupTestData()
        {
            //var SAUser = Credential.CreateSAUser();
            //var connectionString = string.Format(@"Data Source=localhost\SpaceSurfer;Initial Catalog=SSDatabaser;User Id={0};Password={1}; TrustServerCertificate=True;", SAUser.user, SAUser.pass);
            string configFilePath = "/Users/sarahsantos/SpaceSurfer/SourceCode/SS.Backend/Configs/config.local.txt";
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO dao = new SqlDAO(configService);           
            try
            {
                using (SqlConnection connection = new SqlConnection(dao))
                {
                    await connection.OpenAsync();

                    string sql = $"DELETE FROM dbo.Logs WHERE [Username] = 'test@email'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test cleanup: {ex}");
            }
        }

        [TestMethod]
        public async Task DataAccess_SuccessfulConnection_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Info', 'test@email', 'View', 'test desc');";
            var command = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(command);

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(result.RowsAffected > 0);


            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Create_CreateValidRecord_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Info', 'test@email', 'View', 'test desc');";
            var command = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(command);

            // Assert
            Assert.IsTrue(result.RowsAffected > 0);
            Assert.IsFalse(result.HasError);


            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Create_HasNullInput_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), NULL, 'test@email', 'View', 'test desc');";
            var command = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(command);

            // Assert
            Assert.IsTrue(result.HasError);


            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Create_EmptySql_Pass()
        {

            // Arrange
            string sql = "";
            var command = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(command);

            // Assert
            Assert.IsTrue(result.HasError);


            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Create_InvalidSql_Pass()
        {

            // Arrange
            string sql = "ISRENT INTO dbo.Logs VALUES (SYSUTCDATETIME(), '', 'test@email', 'View', 'test desc');";
            var command = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(command);

            // Assert
            Assert.IsTrue(result.HasError);


            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Read_Singular_CorrectLogRead_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc 1');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "SELECT * FROM dbo.Logs WHERE username = 'test@email'";
            command = new SqlCommand(sql);

            // Act
            var result = await dao.ReadSqlResult(command);

            // Assert
            Assert.IsFalse(result.HasError);
            //Assert.IsNotNull(result.ValuesRead); ------------------------> FIX
            //Assert.AreEqual(result.ValuesRead[0][2], "Debug");
            //Assert.AreEqual(result.ValuesRead[0][3], "test@email");
            //Assert.AreEqual(result.ValuesRead[0][4], "Business");
            //Assert.AreEqual(result.ValuesRead[0][5], "test desc 1");

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Read_Multiple_CorrectLogsRead_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc 1');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            await Task.Delay(1000);
            sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Warning', 'test@email', 'Server', 'test desc 2');";
            command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "SELECT * FROM dbo.Logs WHERE username = 'test@email'";
            command = new SqlCommand(sql);

            // Act
            var result = await dao.ReadSqlResult(command);

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(result.ValuesRead);
            //Assert.AreEqual(result.ValuesRead[0][2], "Debug");
            //Assert.AreEqual(result.ValuesRead[0][3], "test@email");
            //Assert.AreEqual(result.ValuesRead[0][4], "Business");
            //Assert.AreEqual(result.ValuesRead[0][5], "test desc 1");
            //Assert.AreEqual(result.ValuesRead[1][2], "Warning");
            //Assert.AreEqual(result.ValuesRead[1][3], "test@email");
            //Assert.AreEqual(result.ValuesRead[1][4], "Server");
            //Assert.AreEqual(result.ValuesRead[1][5], "test desc 2");  ------------------------> FIX


            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Read_NoExistingRow_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc 1');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "SELECT * FROM dbo.Logs WHERE username = 'gibberish@email'";
            command = new SqlCommand(sql);

            // Act
            var result = await dao.ReadSqlResult(command);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsNull(result.ValuesRead);

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Read_EmptySql_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc 1');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "";
            command = new SqlCommand(sql);

            // Act
            var result = await dao.ReadSqlResult(command);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsNull(result.ValuesRead);

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Read_InvalidSql_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc 1');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "SLECET * FROM dbo.Logs WHERE username = 'test@email'";
            command = new SqlCommand(sql);

            // Act
            var result = await dao.ReadSqlResult(command);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsNull(result.ValuesRead);

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Update_UpdatesCorrectly_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "UPDATE dbo.Logs SET description = 'UPDATED DESCRIPTION' WHERE username = 'test@email';";
            var updateCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(updateCommand);

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(result.RowsAffected > 0);

            //Check to make sure description was updated correctly
            sql = "SELECT * FROM dbo.Logs WHERE username = 'test@email';";
            var selectCommand = new SqlCommand(sql);
            var read = await dao.ReadSqlResult(selectCommand);
            Assert.IsFalse(read.HasError);
            Assert.IsNotNull(read.ValuesRead);
            //Assert.AreEqual(read.ValuesRead[0][5], "UPDATED DESCRIPTION");  ------------------------> FIX

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Update_NoExistingRow_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "UPDATE dbo.Logs SET description = 'UPDATED DESCRIPTION' WHERE username = 'gibberish@email';";
            var updateCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(updateCommand);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Update_EmptySql_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "";
            var updateCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(updateCommand);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Update_InvalidSql_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "UADPTE dbo.Logs SET description = 'UPDATED DESCRIPTION' WHERE username = 'test@email';";
            var updateCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(updateCommand);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Delete_DeletesCorrectly_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "DELETE FROM dbo.Logs WHERE username = 'test@email';";
            var deleteCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(deleteCommand);

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(result.RowsAffected > 0);

            //Check to make sure delete succeeded
            var delete = await dao.SqlRowsAffected(deleteCommand);
            Assert.IsTrue(delete.RowsAffected == 0);

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Delete_NoExistingRow_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "DELETE FROM dbo.Logs WHERE username = 'gibberish@email';";
            var deleteCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(deleteCommand);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Delete_EmptySql_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "";
            var deleteCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(deleteCommand);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData();
        }

        [TestMethod]
        public async Task DataAccess_Delete_InvalidSql_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command);
            sql = "DEELTE FROM dbo.Logs WHERE username = 'test@email';";
            var deleteCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(deleteCommand);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData();
        }

    }
}
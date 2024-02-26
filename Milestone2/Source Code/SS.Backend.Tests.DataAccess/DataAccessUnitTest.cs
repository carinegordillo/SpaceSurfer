using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.Tests.DataAccess
{
    [TestClass]
    public class DataAccessUnitTest
    {
        private SqlDAO? dao;

        [TestInitialize]
        public void TestInitialize()
        {
            var SAUser = Credential.CreateSAUser();
            dao = new SqlDAO(SAUser);

        }

        private async Task CleanupTestData()
        {
            var SAUser = Credential.CreateSAUser();
            var connectionString = string.Format(@"Data Source=localhost\SpaceSurfer;Initial Catalog=SS_Server;User Id={0};Password={1};", SAUser.user, SAUser.pass);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string sql = $"DELETE FROM dbo.Logs WHERE [Username] = 'test@email'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
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
            var result = await dao.SqlRowsAffected(command).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(result.RowsAffected > 0);


            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Create_CreateValidRecord_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Info', 'test@email', 'View', 'test desc');";
            var command = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(command).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.RowsAffected > 0);
            Assert.IsFalse(result.HasError);


            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Create_HasNullInput_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), NULL, 'test@email', 'View', 'test desc');";
            var command = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(command).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);


            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Create_EmptySql_Pass()
        {

            // Arrange
            string sql = "";
            var command = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(command).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);


            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Create_InvalidSql_Pass()
        {

            // Arrange
            string sql = "ISRENT INTO dbo.Logs VALUES (SYSUTCDATETIME(), '', 'test@email', 'View', 'test desc');";
            var command = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(command).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);


            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Read_Singular_CorrectLogRead_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc 1');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "SELECT * FROM dbo.Logs WHERE username = 'test@email'";
            command = new SqlCommand(sql);

            // Act
            var result = await dao.ReadSqlResult(command).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(result.ValuesRead);
            Assert.AreEqual(result.ValuesRead[0][2], "Debug");
            Assert.AreEqual(result.ValuesRead[0][3], "test@email");
            Assert.AreEqual(result.ValuesRead[0][4], "Business");
            Assert.AreEqual(result.ValuesRead[0][5], "test desc 1");

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Read_Multiple_CorrectLogsRead_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc 1');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            await Task.Delay(1000);
            sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Warning', 'test@email', 'Server', 'test desc 2');";
            command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "SELECT * FROM dbo.Logs WHERE username = 'test@email'";
            command = new SqlCommand(sql);

            // Act
            var result = await dao.ReadSqlResult(command).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(result.ValuesRead);
            Assert.AreEqual(result.ValuesRead[0][2], "Debug");
            Assert.AreEqual(result.ValuesRead[0][3], "test@email");
            Assert.AreEqual(result.ValuesRead[0][4], "Business");
            Assert.AreEqual(result.ValuesRead[0][5], "test desc 1");
            Assert.AreEqual(result.ValuesRead[1][2], "Warning");
            Assert.AreEqual(result.ValuesRead[1][3], "test@email");
            Assert.AreEqual(result.ValuesRead[1][4], "Server");
            Assert.AreEqual(result.ValuesRead[1][5], "test desc 2");


            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Read_NoExistingRow_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc 1');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "SELECT * FROM dbo.Logs WHERE username = 'gibberish@email'";
            command = new SqlCommand(sql);

            // Act
            var result = await dao.ReadSqlResult(command).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsNull(result.ValuesRead);

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Read_EmptySql_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc 1');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "";
            command = new SqlCommand(sql);

            // Act
            var result = await dao.ReadSqlResult(command).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsNull(result.ValuesRead);

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Read_InvalidSql_Pass()
        {

            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc 1');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "SLECET * FROM dbo.Logs WHERE username = 'test@email'";
            command = new SqlCommand(sql);

            // Act
            var result = await dao.ReadSqlResult(command).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsNull(result.ValuesRead);

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Update_UpdatesCorrectly_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "UPDATE dbo.Logs SET description = 'UPDATED DESCRIPTION' WHERE username = 'test@email';";
            var updateCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(updateCommand).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(result.RowsAffected > 0);

            //Check to make sure description was updated correctly
            sql = "SELECT * FROM dbo.Logs WHERE username = 'test@email';";
            var selectCommand = new SqlCommand(sql);
            var read = await dao.ReadSqlResult(selectCommand).ConfigureAwait(false);
            Assert.IsFalse(read.HasError);
            Assert.IsNotNull(read.ValuesRead);
            Assert.AreEqual(read.ValuesRead[0][5], "UPDATED DESCRIPTION");

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Update_NoExistingRow_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "UPDATE dbo.Logs SET description = 'UPDATED DESCRIPTION' WHERE username = 'gibberish@email';";
            var updateCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(updateCommand).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Update_EmptySql_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "";
            var updateCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(updateCommand).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Update_InvalidSql_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "UADPTE dbo.Logs SET description = 'UPDATED DESCRIPTION' WHERE username = 'test@email';";
            var updateCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(updateCommand).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Delete_DeletesCorrectly_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "DELETE FROM dbo.Logs WHERE username = 'test@email';";
            var deleteCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(deleteCommand).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(result.RowsAffected > 0);

            //Check to make sure delete succeeded
            var delete = await dao.SqlRowsAffected(deleteCommand).ConfigureAwait(false);
            Assert.IsTrue(delete.RowsAffected == 0);

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Delete_NoExistingRow_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "DELETE FROM dbo.Logs WHERE username = 'gibberish@email';";
            var deleteCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(deleteCommand).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Delete_EmptySql_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "";
            var deleteCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(deleteCommand).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DataAccess_Delete_InvalidSql_Pass()
        {
            // Arrange
            string sql = "INSERT INTO dbo.Logs VALUES (SYSUTCDATETIME(), 'Debug', 'test@email', 'Business', 'test desc');";
            var command = new SqlCommand(sql);
            await dao.SqlRowsAffected(command).ConfigureAwait(false);
            sql = "DEELTE FROM dbo.Logs WHERE username = 'test@email';";
            var deleteCommand = new SqlCommand(sql);

            // Act
            var result = await dao.SqlRowsAffected(deleteCommand).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected == 0);

            // Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }


    }
}
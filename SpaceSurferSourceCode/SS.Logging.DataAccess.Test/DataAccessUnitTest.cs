using SS.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Logging.DataAccess.Test
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
        public async Task DataAccess_SuccessfulDataAccess_Pass()
        {

            // Arrange
            var log = new LogEntry
            {
                level = "Info",
                username = "test@email",
                category = "View",
                description = "Testing successful data access..."
            };

            // Act
            var result = await dao.WriteData(log).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.HasError);


            // Cleanup
            if (!result.HasError)
            {
                await CleanupTestData().ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ReadData_Singular_ReadsCorrectLog_Pass()
        {
            // Arrange
            var log = new LogEntry
            {
                level = "Warning",
                username = "test@email",
                category = "Server",
                description = "Testing ReadData_Singular..."
            };

            var write = await dao.WriteData(log).ConfigureAwait(false);

            // Act
            var read = await dao.ReadData_Singular(write.Log_ID).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(read.HasError);
            Assert.AreEqual(log.level, read.LogEntry.level);
            Assert.AreEqual(log.username, read.LogEntry.username);
            Assert.AreEqual(log.category, read.LogEntry.category);
            Assert.AreEqual(log.description, read.LogEntry.description);

            // Cleanup
            if (!read.HasError)
            {
                await CleanupTestData().ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ReadData_Multiple_ReadsCorrectLogs_Pass()
        {
            // Arrange
            var logs = new List<LogEntry>
            {
                new LogEntry
                {
                    level = "Warning",
                    username = "test@email",
                    category = "Server",
                    description = "Testing ReadData_Multiple 1..."
                },
                new LogEntry
                {
                    level = "Info",
                    username = "test@email",
                    category = "View",
                    description = "Testing ReadData_Multiple 2..."
                },
                new LogEntry
                {
                    level = "Debug",
                    username = "test@email",
                    category = "Data",
                    description = "Testing ReadData_Multiple 3..."
                }
            };

            foreach (var log in logs)
            {
                var write = await dao.WriteData(log).ConfigureAwait(false);
                Assert.IsFalse(write.HasError);
            }

            // Act
            var read = await dao.ReadData_Multiple(logs.Count).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(read.HasError);
            Assert.AreEqual(logs.Count, read.LogEntries.Count);

            //// Validate each log entry
            //for (int i = 0; i < logs.Count; i++)
            //{
            //    var expectedLog = logs[i];
            //    var actualLog = read.LogEntries[i];

            //    Assert.AreEqual(expectedLog.level, actualLog.level);
            //    Assert.AreEqual(expectedLog.username, actualLog.username);
            //    Assert.AreEqual(expectedLog.category, actualLog.category);
            //    Assert.AreEqual(expectedLog.description, actualLog.description);
            //}

            // Cleanup
            if (!read.HasError)
            {
                foreach (var log in logs)
                {
                    await CleanupTestData().ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task UpdateData_UpdatesCorrectly_Pass()
        {
            // Arrange
            var log = new LogEntry
            {
                level = "Info",
                username = "test@email",
                category = "Application",
                description = "OG log entry"
            };

            var write = await dao.WriteData(log).ConfigureAwait(false);

            // Act
            var update = await dao.UpdateData(write.Log_ID, "Description", "OG log entry", "Updated log entry").ConfigureAwait(false);

            // Assert
            Assert.IsFalse(update.HasError);
            var read = await dao.ReadData_Singular(write.Log_ID).ConfigureAwait(false);
            Assert.AreEqual("Updated log entry", read.LogEntry.description, "Description should be updated");

            // Cleanup
            if (!read.HasError)
            {
                await CleanupTestData().ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task DeleteData_DeletesCorrectLog_Pass()
        {
            // Arrange
            var log = new LogEntry
            {
                level = "Error",
                username = "test@email",
                category = "View",
                description = "Testing DeleteData..."
            };

            var write = await dao.WriteData(log).ConfigureAwait(false);

            // Act
            var delete = await dao.DeleteData(write.Log_ID).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(delete.HasError);
            var read = await dao.ReadData_Singular(write.Log_ID).ConfigureAwait(false);
            Assert.IsTrue(read.HasError);
        }

    }
}
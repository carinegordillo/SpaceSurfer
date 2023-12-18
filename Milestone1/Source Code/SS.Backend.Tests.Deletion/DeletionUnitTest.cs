using SS.Backend.DataAccess;
using SS.Backend.Services.DeletingService;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.Tests.Services
{

    [TestClass]
    public class DeletionUnitTest
    {
        // Initializing 
        private SqlDAO? dao;
        private CustomSqlCommandBuilder? commandBuilder;
        private Logger? logger;


        [TestInitialize]
        public void InitializeTest()
        {
            var SAUser = Credential.CreateSAUser();
            dao = new SqlDAO(SAUser);
            commandBuilder = new CustomSqlCommandBuilder();
            logger = new Logger(new SqlLogTarget(dao));
        }

        private async Task CleanupTestData(int choice)
        {
            var SAUser = Credential.CreateSAUser();
            var connectionString = string.Format(@"Data Source=localhost\SpaceSurfer;Initial Catalog=SS_Server;User Id={0};Password={1};", SAUser.user, SAUser.pass);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string sql1 = $"DELETE FROM dbo.Logs WHERE [Username] = 'temporary@email'";
                    string sql2 = $"DELETE FROM dbo.Logs WHERE [Username] = 'temporaru@email'";
                    string sql3 = $"DELETE FROM dbo.userHash WHERE [Username] = 'temporary@email'";
                    string sql4 = $"DELETE FROM dbo.userAccount WHERE [Username] = 'temporary@email'";


                    switch (choice)
                    {
                        case 1:
                            using (SqlCommand command = new SqlCommand(sql1, connection))
                            {
                                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                            }
                            break;

                        case 2:
                            using (SqlCommand command = new SqlCommand(sql2, connection))
                            {
                                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                            }
                            using (SqlCommand command = new SqlCommand(sql3, connection))
                            {
                                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                            }
                            using (SqlCommand command = new SqlCommand(sql4, connection))
                            {
                                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                            }
                            break;

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test cleanup: {ex}");
            }
        }


        public async Task insertUser()
        {
            var SAUser = Credential.CreateSAUser();

            var connectionString = string.Format(@"Data Source=localhost\SpaceSurfer;Initial Catalog=SS_Server;User Id={0};Password={1};", SAUser.user, SAUser.pass);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);


                    // Enters values to dbo.userAccount
                    string sql1 = $"SET IDENTITY_INSERT dbo.userAccount ON;" +
                                    $"\r\nINSERT INTO dbo.userAccount(user_id, username,birthDate) VALUES (14, 'temporary@email','2002-12-21');" +
                                    $"\r\nSET IDENTITY_INSERT dbo.userAccount OFF;";
                    // Enters values to dbo.userHash
                    string sql2 = $"INSERT INTO dbo.userHash(hashedUsername, username, user_id) VALUES ('gdulfzfh2duzkp', 'temporary@email', 14)";

                    // Executes sql commands
                    using (SqlCommand command = new SqlCommand(sql1, connection))
                    {
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                    using (SqlCommand command = new SqlCommand(sql2, connection))
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

        [TestCleanup]
        public async Task CleanupTestData()
        {
            await CleanupTestData(1);
        }

        [TestMethod]
        public async Task DeleteAccount_Successful_Deletion()
        {
            await insertUser();

            // initializing account deletion
            var deleter = new AccountDeletion();

            // Temporary user username
            var userToDelete = "temporary@email";

            // Execute deletion command to the database
            var result = await deleter.DeleteAccount(userToDelete);

            // Deletion should be successful
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(result.RowsAffected > 0);

            await CleanupTestData(1);
        }

        [TestMethod]
        public async Task DeleteAccount_Unsuccessful_Deletion()
        {
            await insertUser();
            // initializing account deletion
            var deleter = new AccountDeletion();

            // Temporary user username
            var userToDelete = "temporaru@email";

            // Execute deletion command to the database
            var result = await deleter.DeleteAccount(userToDelete);

            // Deletion should be unsuccessful, therefore true
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(result.RowsAffected <= 0);

            await CleanupTestData(2);

        }

        [TestMethod]
        public async Task TableNames_Success()
        {
            // initializing account deletion
            var test = new DatabaseHelper();
            var result = await test.RetrieveTableNames();


            Assert.IsTrue(result.ValuesRead.Count >= 1);

        }

    }
}
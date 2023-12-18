using SS.Backend.DataAccess;
using SS.Backend.Services.DeletingService;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;

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

        [TestMethod]
        public async Task DeleteAccount_Successful_Deletion()
        {
            // initializing account deletion
            var deleter = new Deleter();

            // Temporary user username
            var userToDelete = "temporaryname";

            // Execute deletion command to the database
            var result = await deleter.DeleteAccount(userToDelete);

            // Deletion should be successful, therefore false
            Assert.IsFalse(result.HasError, "Deletion was successful.");
            Assert.IsTrue(result.RowsAffected > 0);

        }

        [TestMethod]
        public async Task DeleteAccount_Unsuccessful_Deletion()
        {
            // initializing account deletion
            var deleter = new Deleter();

            // Temporary user username
            var userToDelete = "ouser";

            // Execute deletion command to the database
            var result = await deleter.DeleteAccount(userToDelete);

            // Deletion should be unsuccessful, therefore true
            Assert.IsTrue(result.HasError, "Deletion was unsuccessful.");
            Assert.IsTrue(result.RowsAffected <= 0);

        }

        public async Task TableNames()
        {
            // initializing account deletion
            var test = new TableNames();

            Console.WriteLine(test);


        }
    }
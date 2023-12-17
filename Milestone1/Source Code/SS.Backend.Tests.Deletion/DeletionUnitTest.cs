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
        private CustomSqlCommandBuilder commandBuilder;
        private Logger logger;

        [TestInitialize]
        public void InitializeTest()
        {
            var SAUser = Credential.CreateSAUser();
            dao = new SqlDAO(SAUser);
            commandBuilder = new CustomSqlCommandBuilder();
        }

        [TestMethod]
        public async Task DeleteAccount_Successful_Deletion()
        {
            var deleter = new Deleter();
        }

    }
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Tests.UsageAnalysisObeservability
{
    [TestClass]
    public class SystemObservabilityTests
    {
        private Response response;
        private CustomSqlCommandBuilder commandBuilder;
        private ISqlDAO sqlDAO;
        private ConfigService configService;
        private ISystemObservabilityDAO _systemObservabilityDAOService;
        private SystemObservabilityService _systemObservabuilityService;

        [TestInitialize]
        public void TestInitialize()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

            response = new Response();
            commandBuilder = new CustomSqlCommandBuilder();
            configService = new ConfigService(configFilePath);
            sqlDAO = new SqlDAO(configService);
            _systemObservabilityDAOService = new SystemObservabilityDAO(sqlDAO);
            _systemObservabuilityService = new SystemObservabilityService(_systemObservabilityDAOService);

        }


        [TestMethod]
        public void RetrievingTop3ViewDurations_Success()
        {

        }

        [TestMethod]
        public void InsertViewDuration_Success()
        {

        }
    }
}
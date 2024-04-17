// global using Microsoft.VisualStudio.TestTools.UnitTesting;
// using System.IO;
// using System.Threading.Tasks;
// using Microsoft.Data.SqlClient;
// using SS.Backend.DataAccess;
// using SS.Backend.TaskManagerHub;
// using SS.Backend.SharedNamespace;

// namespace SS.Backend.Tests.TaskManagerHub{

//    [TestClass]
//    public class TaskManagerHubTests
//    {
//     //    private ReservationCreationManager _reservationCreationManager;
//        private SqlDAO _sqlDao;
//        private ConfigService _configService;
//        private TaskManagerHubService  _taskManagerHubService;

//        private TaskManagerHubRepo _taskManagerHubRepo;


//        [TestInitialize]
//        public void Setup()
//        {
//            var baseDirectory = AppContext.BaseDirectory;
//            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
//            _configService = new ConfigService(configFilePath);
//            _sqlDao = new SqlDAO(_configService);


//            _taskManagerHubRepo = new TaskManagerHubRepo(_sqlDao);

//            _taskManagerHubService = new TaskManagerHubService(_taskManagerHubRepo);

//        }
// }}
global using Microsoft.VisualStudio.TestTools.UnitTesting;

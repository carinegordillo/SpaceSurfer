using SS.Backend.DataAccess;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using SS.Backend.Services.PersonalOverviewService;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Tests.PersonalOverviewService
{
    [TestClass]
    public class ReadingReservations
    {
        private Response response;
        private CustomSqlCommandBuilder commandBuilder;
        private SqlDAO sqlDAO;
        private ConfigService configService;
        private IPersonalOverview _personalOverview;
        private IPersonalOverviewDAO _personalOverviewDAO;
        private ReservationCreatorService _ReservationCreatorService;
        private ReservationManagementRepository _reservationManagementRepository;
        private ReservationValidationService _reservationValidationService;
        private ReservationCreationManager _reservationCreationManager;
        private ReservationCancellationService _reservationCancellationService;


        [TestMethod]
        public void TestInitialize()
        {
            response = new Response();
            commandBuilder = new CustomSqlCommandBuilder();

            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

            configService = new ConfigService(configFilePath);
            sqlDAO = new SqlDAO(configService);
            _personalOverview = new PersonalOverview(sqlDAO);
        }
    }
}
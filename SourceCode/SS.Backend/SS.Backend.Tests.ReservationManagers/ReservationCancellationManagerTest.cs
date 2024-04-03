
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Tests.ReservationManagers{

    [TestClass]
    public class ReservationCancellationManagerTests
    {
        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private ReservationCreatorService  _reservationCreatorService;


        private ReservationManagementRepository _reservationManagementRepository;

        private ReservationValidationService _reservationValidationService;

        private ReservationModificationService _reservationModificationService;

        private ReservationModificationManager _reservationModificationManager;

        private ReservationCancellationManager _reservationCancellationManager;

        private ReservationCancellationService _reservationCancellationService;

        private ReservationStatusUpdater _reservationStatusUpdater;

        //uses newManualIDReservations because it allows manual id insertion

        string MANUAL_ID_TABLE = "dbo.NewManualIDReservations";


        string userHash1 = "Yu86Ho6KDmtOeP687I/AHNE4rhxoCzZDs9v/Mpe+SZw=";

        [TestInitialize]
        public void Setup()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);

            

            _reservationManagementRepository = new ReservationManagementRepository(_sqlDao);

            _reservationValidationService = new ReservationValidationService(_reservationManagementRepository);

            _reservationCreatorService = new ReservationCreatorService(_reservationManagementRepository);
            
            _reservationModificationService = new ReservationModificationService(_reservationManagementRepository);

            _reservationModificationManager = new ReservationModificationManager(_reservationModificationService, _reservationValidationService);

            _reservationCancellationService = new ReservationCancellationService(_reservationManagementRepository);

            _reservationStatusUpdater = new ReservationStatusUpdater(_reservationManagementRepository);

            _reservationCancellationManager = new ReservationCancellationManager(_reservationCancellationService, _reservationValidationService, _reservationStatusUpdater);
        }

        [TestMethod]
        public async Task CancelSpaceSurferSpaceReservation_Success()
        {
            DateTime now = DateTime.Now;
            DateTime reservationStart = new DateTime(now.Year, now.Month, now.Day, 16, 0, 0).AddDays(0);
            DateTime reservationEnd = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0).AddDays(0);

            UserReservationsModel userReservationsModel = new UserReservationsModel
            {
                ReservationID = 2003,
                CompanyID = 2,
                FloorPlanID = 2,
                SpaceID = "S4-FP2",
                ReservationStartTime = reservationStart,
                ReservationEndTime = reservationEnd,
                UserHash = userHash1
            };

            var response = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE,userReservationsModel);

            Console.WriteLine(response.ErrorMessage);

            Assert.IsFalse(response.HasError);

            

            response = await _reservationCancellationManager.CancelSpaceSurferSpaceReservationAsync(userReservationsModel, MANUAL_ID_TABLE);

            Console.WriteLine(response.ErrorMessage);

            Assert.IsFalse(response.HasError);



        }

        [TestCleanup]
        public void Cleanup()
        {

            var testReservtaionIds = new List<int> { 2003};
            var commandBuilder = new CustomSqlCommandBuilder();

            var deleteCommand = commandBuilder.BeginDelete(MANUAL_ID_TABLE)
                                            .Where($"reservationID IN ({string.Join(",", testReservtaionIds)})")
                                            .Build();
                                            
            _sqlDao.SqlRowsAffected(deleteCommand);

        }

    }
}
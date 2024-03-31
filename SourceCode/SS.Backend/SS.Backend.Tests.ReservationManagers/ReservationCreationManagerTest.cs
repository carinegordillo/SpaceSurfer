
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Tests.ReservationManagers{

    [TestClass]
    public class ReservationCreationManagerTests
    {
        private ReservationCreationManager _reservationCreationManager;
        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private ReservationCreatorService  _reservationCreatorService;

        private ReservationManagementRepository _reservationManagementRepository;

        private ReservationValidationService _reservationValidationService;

        string TABLE_NAME = "dbo.testReservations";

        string userHash1 = "hashed_manager3";

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
            
            _reservationCreationManager = new ReservationCreationManager(_reservationCreatorService, _reservationValidationService);
        }

        [TestMethod]
        public async Task CreateSpaceSurferSpaceReservation_Success()
        {
            DateTime now = DateTime.Now;
            DateTime reservationStart = new DateTime(now.Year, now.Month, now.Day, 16, 0, 0).AddDays(0);
            DateTime reservationEnd = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0).AddDays(0);

            UserReservationsModel userReservationsModel = new UserReservationsModel
            {
                CompanyID = 1027,
                FloorPlanID = 60,
                SpaceID = "SPACE002",
                ReservationStartTime = reservationStart,
                ReservationEndTime = reservationEnd,
                UserHash = userHash1
            };

            var response = await _reservationCreationManager.CreateSpaceSurferSpaceReservationAsync(userReservationsModel, TABLE_NAME);

            Console.WriteLine(response.ErrorMessage);

            Assert.IsFalse(response.HasError);

        }

        [TestMethod]
        public async Task CreateSpaceSurferSpaceReservation_InvalidBusinessHours_InvalidMaxDuration()
        {
            DateTime now = DateTime.Now;
            DateTime reservationStart = new DateTime(now.Year, now.Month, now.Day, 13, 0, 0).AddDays(1);
            DateTime reservationEnd = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0).AddDays(1);
            // Arrange
            UserReservationsModel userReservationsModel = new UserReservationsModel
            {
                CompanyID = 1030,
                FloorPlanID = 64,
                SpaceID = "SPACE006",
                ReservationStartTime = reservationStart,
                ReservationEndTime = reservationEnd,
                UserHash = userHash1
            };
            

            var response = await _reservationCreationManager.CreateSpaceSurferSpaceReservationAsync(userReservationsModel, TABLE_NAME);

            Assert.IsTrue(response.HasError, "Expected no error in reservation creation.");
            Assert.IsTrue(response.ErrorMessage.Contains("Reservation did not pass validation checks"));
            Assert.IsTrue(response.ErrorMessage.Contains("CheckBusinessHours, MaxDurationPerSeat"));
        }
    }
}
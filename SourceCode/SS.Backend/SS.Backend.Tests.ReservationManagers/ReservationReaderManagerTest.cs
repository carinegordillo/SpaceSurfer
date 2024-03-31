
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using SS.Backend.SharedNamespace;
using System.Data;

namespace SS.Backend.Tests.ReservationManagers{

    [TestClass]
    public class ReservationReaderManagerTests
    {
        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private ReservationCreatorService  _reservationCreatorService;

        private ReservationManagementRepository _reservationManagementRepository;

        private ReservationValidationService _reservationValidationService;

        private ReservationModificationService _reservationModificationService;

        private ReservationModificationManager _reservationModificationManager;

        private ReservationReadService _reservationReadService;

        private ReservationReaderManager _reservationReaderManager;
        private ReservationCancellationManager _reservationCancellationManager;
        private ReservationCancellationService _reservationCancellationService;

        private ReservationStatusUpdater _reservationStatusUpdater;


        //uses newManualIDReservations because it allows manual id insertion

        string MANUAL_ID_TABLE = "dbo.NewManualIDReservations";


        string userHash1 = "hashed_manager1";

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

            _reservationReadService = new ReservationReadService(_reservationManagementRepository);

            _reservationReaderManager = new ReservationReaderManager(_reservationReadService);
            
            _reservationCancellationService = new ReservationCancellationService(_reservationManagementRepository);

            _reservationStatusUpdater = new ReservationStatusUpdater(_reservationManagementRepository);

            _reservationCancellationManager = new ReservationCancellationManager(_reservationCancellationService, _reservationValidationService, _reservationStatusUpdater);
        }

        [TestMethod]
        public async Task ReadSpaceSurferSpaceReservation_Success()
        {
            Response response = new Response();
            DateTime now = DateTime.Now;
            DateTime reservationStart = new DateTime(now.Year, now.Month, now.Day, 16, 0, 0).AddDays(0);
            DateTime reservationEnd = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0).AddDays(0);

            UserReservationsModel userReservationsModel1 = new UserReservationsModel
            {
                ReservationID = 2004,
                CompanyID = 1027,
                FloorPlanID = 60,
                SpaceID = "SPACE003",
                ReservationStartTime = reservationStart,
                ReservationEndTime = reservationEnd,
                UserHash = userHash1
            };

            response = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE,userReservationsModel1);
            Assert.IsFalse(response.HasError);

            UserReservationsModel userReservationsModel2 = new UserReservationsModel
            {
                ReservationID = 2005,
                CompanyID = 1027,
                FloorPlanID = 60,
                SpaceID = "SPACE002",
                ReservationStartTime = reservationStart,
                ReservationEndTime = reservationEnd,
                UserHash = "hashed_user4"
            };
            response = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE,userReservationsModel2);
            Assert.IsFalse(response.HasError);

            UserReservationsModel userReservationsModel3 = new UserReservationsModel
            {
                ReservationID = 2006,
                CompanyID = 1027,
                FloorPlanID = 60,
                SpaceID = "SPACE001",
                ReservationStartTime = reservationStart,
                ReservationEndTime = reservationEnd,
                UserHash = userHash1
            };
            response = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE,userReservationsModel3);


            Assert.IsFalse(response.HasError);

            var reservationReadResult = await _reservationReaderManager.GetAllUserSpaceSurferSpaceReservationAsync(userHash1, MANUAL_ID_TABLE);


            var expectedIds = new HashSet<int> { 2004, 2006}; 
            var actualIds = new HashSet<int>();

        
            foreach (var info in reservationReadResult)
            {
                actualIds.Add(info.ReservationID.Value);
            }

            Assert.IsNotNull(reservationReadResult);
            Assert.IsTrue(reservationReadResult.Any());


            Assert.IsTrue(expectedIds.SetEquals(actualIds));
        }

        [TestMethod]
        public async Task ReadActiveSpaceSurferSpaceReservation_Success()
        {
            Response response = new Response();
            DateTime now = DateTime.Now;
            DateTime reservationStart = new DateTime(now.Year, now.Month, now.Day, 16, 0, 0).AddDays(0);
            DateTime reservationEnd = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0).AddDays(0);

            //create reservations

            UserReservationsModel userReservationsModel1 = new UserReservationsModel
            {
                ReservationID = 2007,
                CompanyID = 1029,
                FloorPlanID = 63,
                SpaceID = "SPACE005",
                ReservationStartTime = reservationStart,
                ReservationEndTime = reservationEnd,
                UserHash = userHash1
            };

            response = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE,userReservationsModel1);
            Assert.IsFalse(response.HasError);

            UserReservationsModel userReservationsModel2 = new UserReservationsModel
            {
                ReservationID = 2008,
                CompanyID = 1027,
                FloorPlanID = 60,
                SpaceID = "SPACE001",
                ReservationStartTime = reservationStart,
                ReservationEndTime = reservationEnd,
                UserHash = userHash1
            };
            response = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE,userReservationsModel2);
            Assert.IsFalse(response.HasError);

            UserReservationsModel userReservationsModel3 = new UserReservationsModel
            {
                ReservationID = 2009,
                CompanyID = 1029,
                FloorPlanID = 63,
                SpaceID = "SPACE005",
                ReservationStartTime = reservationStart,
                ReservationEndTime = reservationEnd,
                UserHash = userHash1
            };
            response = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE,userReservationsModel3);


            Assert.IsFalse(response.HasError);

            //cancel reservation 2005

            response = await _reservationCancellationManager.CancelSpaceSurferSpaceReservationAsync(userReservationsModel1, MANUAL_ID_TABLE);

            Assert.IsFalse(response.HasError);

            //read reservtaions

            var reservationReadResult = await _reservationReaderManager.GetAllUserActiveSpaceSurferSpaceReservationAsync(userHash1, MANUAL_ID_TABLE);


            var expectedIds = new HashSet<int> { 2008, 2009}; 
            var actualIds = new HashSet<int>();

            if (reservationReadResult != null)
            {
                foreach (var info in reservationReadResult)
                {
                    actualIds.Add(info.ReservationID.Value);
                }
            }
        

            Assert.IsNotNull(reservationReadResult);
            Assert.IsTrue(reservationReadResult.Any());

            Assert.IsTrue(expectedIds.SetEquals(actualIds));

        }

        [TestCleanup]
        public void Cleanup()
        {

            var testReservtaionIds = new List<int> { 2004, 2005, 2006, 2007, 2008, 2009};
            var commandBuilder = new CustomSqlCommandBuilder();

            var deleteCommand = commandBuilder.BeginDelete(MANUAL_ID_TABLE)
                                            .Where($"reservationID IN ({string.Join(",", testReservtaionIds)})")
                                            .Build();
                                            
            _sqlDao.SqlRowsAffected(deleteCommand);

        }

    }
}
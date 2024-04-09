


//using SS.Backend.DataAccess;
//using System.IO;
//using System.Threading.Tasks;
//using SS.Backend.ReservationManagement;
//using SS.Backend.SharedNamespace;
//using Microsoft.Data.SqlClient;
//namespace SS.Backend.Tests.ReservationManagement{

//    [TestClass]
//    public class ReservationCancellationServiceUnitTests
//    {
//        private SqlDAO _sqlDao;
//        private ConfigService _configService;
//        private ReservationCreatorService  _reservationCreatorService;
//        private ReservationCancellationService _reservationCancellationService;

//        private ReservationValidationService _reservationValidationService;

//        private ReservationManagementRepository _reservationManagementRepository;

//        //using dbo.TestReservtaions becaus it allows manual id insertion

//        string MANUAL_ID_TABLE = "dbo.NewManualIDReservations";
//        string USER_HASH2 = "Yu86Ho6KDmtOeP687I/AHNE4rhxoCzZDs9v/Mpe+SZw=";

        
        
//        [TestInitialize]
//        public void Setup()
//        {

//            var baseDirectory = AppContext.BaseDirectory;
//            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
//            _configService = new ConfigService(configFilePath);
//            _sqlDao = new SqlDAO(_configService);

//            _reservationManagementRepository = new ReservationManagementRepository(_sqlDao);

//            _reservationValidationService = new ReservationValidationService(_reservationManagementRepository);

//            _reservationCreatorService = new ReservationCreatorService(_reservationManagementRepository);

//            _reservationCancellationService = new ReservationCancellationService(_reservationManagementRepository);


//        }

//        [TestMethod]
//        public async Task TestCancellation()
//        {
//            // Arrange

//            Response reservtaionCreationResult = new Response();
//            Response reservationCancellationResult = new Response();

//            UserReservationsModel reservationToBeCancelled = new UserReservationsModel
//            {
//                ReservationID = 112,
//                CompanyID = 1,
//                FloorPlanID = 1,
//                SpaceID = "S2-FP1",
//                ReservationStartTime = new DateTime(2025, 01, 01, 13, 00, 00), 
//                ReservationEndTime = new DateTime(2025, 01, 01, 15, 00, 00), 
//                Status = ReservationStatus.Active,
//                UserHash = USER_HASH2
//            };
            
            
//            reservtaionCreationResult = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE,reservationToBeCancelled);
//            Assert.IsFalse(reservtaionCreationResult.HasError);

//            // cancel reservtion
//            reservationCancellationResult = await _reservationCancellationService.CancelReservationAsync(MANUAL_ID_TABLE, reservationToBeCancelled.ReservationID.Value);
//            Console.WriteLine(reservationCancellationResult.ErrorMessage);
//            Assert.IsFalse(reservationCancellationResult.HasError);
//            Assert.IsTrue(reservationCancellationResult.RowsAffected > 0);
            
//        }

//        [TestMethod]
//        public async Task TestCancellationOfNonExistentReservation()
//        {
//            // Arrange

//            Response reservtaionCancellationResult = new Response();

//            UserReservationsModel reservationToBeCancelled = new UserReservationsModel
//            {
//                ReservationID = 578,
//                CompanyID = 1,
//                FloorPlanID = 1,
//                SpaceID = "S2-FP1",
//                ReservationStartTime = new DateTime(2024, 05, 01, 13, 00, 00), // Jan 1, 2022, 1:00 PM
//                ReservationEndTime = new DateTime(2024, 05, 01, 15, 00, 00), // Jan 1, 2022, 3:00 PM
//                Status = ReservationStatus.Active,
//                UserHash = USER_HASH2
//            };

//            // cancel reservtion
//            reservtaionCancellationResult = await _reservationCancellationService.CancelReservationAsync(MANUAL_ID_TABLE, reservationToBeCancelled.ReservationID.Value);
//            Assert.IsTrue(reservtaionCancellationResult.HasError);
//            Assert.IsTrue(reservtaionCancellationResult.RowsAffected == 0);
            
//        }

//        [TestMethod]
//    public async Task ReservationStatusUpdater_UpdatesCorrectly(){

//        Response reservation1Response = new Response();

//        UserReservationsModel reservation1 = new UserReservationsModel
//            {
//                ReservationID = 927,
//                CompanyID = 1,
//                FloorPlanID = 1,
//                SpaceID = "S2-FP1",
//                ReservationStartTime = new DateTime(2024, 02, 23, 11, 00, 00), 
//                ReservationEndTime = new DateTime(2024, 02, 23, 12, 00, 00), 
//                Status = ReservationStatus.Active,
//                UserHash = USER_HASH2
//            };

//        // Act 1: Create the first reservation
//        reservation1Response = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE, reservation1);
//        Console.WriteLine(reservation1Response.ErrorMessage);
//        Assert.IsFalse(reservation1Response.HasError);

//        // Reservation 2 is set to a future date
//        UserReservationsModel reservation2 = new UserReservationsModel {
//            ReservationID = 6765,
//            CompanyID = 1,
//            FloorPlanID = 1,
//            SpaceID = "S2-FP1",
//            ReservationStartTime = DateTime.UtcNow.AddDays(2).Date + new TimeSpan(14, 0, 0),
//            ReservationEndTime = DateTime.UtcNow.AddDays(2).Date + new TimeSpan(17, 0, 0), 
//            Status = ReservationStatus.Active,
//            UserHash = USER_HASH2
//        };

//        var reservation2Response = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE, reservation2);

//        Assert.IsFalse(reservation2Response.HasError);

//        // Update reservation statuses
//        ReservationStatusUpdater reservationStatusUpdater = new ReservationStatusUpdater(_reservationManagementRepository);
//        var response = await reservationStatusUpdater.UpdateReservtionStatuses(MANUAL_ID_TABLE, "UpdateReservationStatusManualID");
//        Console.WriteLine(response.ErrorMessage);
//        Assert.IsFalse(response.HasError); 
//    }


//        [TestCleanup]
//        public void Cleanup()
//        {

//            var testReservtaionIds = new List<int> { 112, 927, 6765};
//            var commandBuilder = new CustomSqlCommandBuilder();

//            var deleteCommand = commandBuilder.BeginDelete(MANUAL_ID_TABLE)
//                                            .Where($"reservationID IN ({string.Join(",", testReservtaionIds)})")
//                                            .Build();
                                            
//            _sqlDao.SqlRowsAffected(deleteCommand);

//        }

//    }
       
//}


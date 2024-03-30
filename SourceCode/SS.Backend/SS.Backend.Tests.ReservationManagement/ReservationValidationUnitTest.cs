


using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.ReservationManagement;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
namespace SS.Backend.Tests.ReservationManagement{

    [TestClass]
    public class ReservationValidationServiceUnitTests
    {
        private SqlDAO _sqlDao;
        private ConfigService _configService;

        private ReservationValidationService _reservationValidationService;
        private ReservationManagementRepository _reservationManagementRepository;
        private ReservationCreatorService _reservationCreatorService;

        string AUTO_ID_TABLE = "dbo.NewAutoIDReservations";

        string PROD_TABLE = "dbo.reservations";
        string USER_HASH1 = "testUserHash1";
        string USER_HASH2 = "testUserHash3";

        
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


        }


        [TestMethod]
        public async Task CheckConflictingReservationsAsyncTestConflict()
        {
            Response response = new Response();


            UserReservationsModel reservation1 = new UserReservationsModel
            {
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationStartTime = new DateTime(2023, 01, 01, 13, 00, 00), 
                ReservationEndTime = new DateTime(2023, 01, 01, 15, 00, 00), 
                Status = ReservationStatus.Active,
                UserHash = USER_HASH1
            };

            // Act 1: Create the first reservation
            response = await _reservationCreatorService.CreateReservationWithAutoIDAsync(PROD_TABLE, reservation1);
            Console.WriteLine(response.ErrorMessage);
            Assert.IsFalse(response.HasError);

            
            UserReservationsModel reservation2 = new UserReservationsModel
            {
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationStartTime = new DateTime(2023, 01, 01, 14, 00, 00), 
                ReservationEndTime = new DateTime(2023, 01, 01, 16, 00, 00), 
                Status = ReservationStatus.Active,
                UserHash = USER_HASH1
            };

            response = await _reservationValidationService.ValidateNoConflictingReservationsAsync(reservation2);
            Console.WriteLine(response.ErrorMessage);
            Assert.IsTrue(response.HasError);

        }


        [TestMethod]
        public async Task CheckConflictingReservationsAsyncTestNoConflict()
        {
            Response response = new Response();

            // First reservation
            UserReservationsModel reservation1 = new UserReservationsModel
            {
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationStartTime = new DateTime(2025, 03, 01, 13, 00, 00), // Jan 1, 2022, 1:00 PM
                ReservationEndTime = new DateTime(2025, 03, 01, 14, 00, 00), // Jan 1, 2022, 3:00 PM
                Status = ReservationStatus.Active,
                 UserHash = USER_HASH1
            };

            // Act 1: Create the first reservation
            response = await _reservationCreatorService.CreateReservationWithAutoIDAsync(AUTO_ID_TABLE, reservation1);
            Console.WriteLine(response.ErrorMessage);
            Assert.IsFalse(response.HasError);

            // Second reservation which overlaps the first one
            UserReservationsModel reservation2 = new UserReservationsModel
            {
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationStartTime = new DateTime(2025, 03, 01, 16, 00, 00), 
                ReservationEndTime = new DateTime(2025, 03, 01, 18, 00, 00), 
                Status = ReservationStatus.Active,
                UserHash = USER_HASH1
            };

            // Act 2 Check for conflicts before creating the second reservation
            response = await _reservationValidationService.ValidateNoConflictingReservationsAsync(reservation2);
            Console.WriteLine(response.ErrorMessage);
            
            // Assert Expect a conflict
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task CheckReservationWithinBusinessHours_Fail(){
        Response response = new Response();

   
            UserReservationsModel reservation1 = new UserReservationsModel
            {
                CompanyID = 1,
                FloorPlanID = 2,
                SpaceID = "SPACE202",
                ReservationStartTime = new DateTime(2023, 01, 01, 07, 00, 00), 
                ReservationEndTime = new DateTime(2023, 01, 01, 08, 00, 00), 
                Status = ReservationStatus.Active,
                 UserHash = USER_HASH1
            };

            // Act 1: Create the first reservation
            response = await _reservationValidationService.ValidateWithinHoursAsync(reservation1);
            Console.WriteLine(response.ErrorMessage);
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public async Task CheckReservationWithinBusinessHours_Pass(){
        Response response = new Response();

            // First reservation
            UserReservationsModel reservation1 = new UserReservationsModel
            {
                CompanyID = 1,
                FloorPlanID = 2,
                SpaceID = "SPACE202",
                ReservationStartTime = new DateTime(2024, 03, 29, 13, 00, 00), 
                ReservationEndTime = new DateTime(2024, 03, 29, 14, 00, 00), 
                Status = ReservationStatus.Active,
                 UserHash = USER_HASH1
            };

            // Act 1: Create the first reservation
            response = await _reservationValidationService.ValidateWithinHoursAsync(reservation1);
            Console.WriteLine(response.ErrorMessage);
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task ValidateReservationDurationAsync_Test_Pass()
        {
            // Arrange
            UserReservationsModel userReservationsModel = new UserReservationsModel
            {
                CompanyID = 1,
                FloorPlanID = 2,
                SpaceID = "SPACE202",
                ReservationStartTime = new DateTime(2026, 01, 01, 07, 00, 00), 
                ReservationEndTime = new DateTime(2026, 01, 01, 08, 00, 00), 
                Status = ReservationStatus.Active,
                 UserHash = USER_HASH1
            };

            // Act
            var response = await _reservationValidationService.ValidateReservationDurationAsync(userReservationsModel);

            // Assert
            Console.WriteLine(response.ErrorMessage);
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task ValidateReservationDurationAsync_Test_Fail()
        {
            // Arrange
            UserReservationsModel userReservationsModel = new UserReservationsModel
            {
                CompanyID = 1,
                FloorPlanID = 2,
                SpaceID = "SPACE202",
                ReservationStartTime = new DateTime(2026, 01, 01, 13, 00, 00), 
                ReservationEndTime = new DateTime(2026, 01, 01, 18, 00, 00), 
                Status = ReservationStatus.Active,
                 UserHash = USER_HASH1
            };

            // Act
            var response = await _reservationValidationService.ValidateReservationDurationAsync(userReservationsModel);

            // Assert
            Console.WriteLine(response.ErrorMessage);
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public void ValidateReservationLeadTime_Test_Pass()
        {
            // Arrange
            UserReservationsModel userReservationsModel = new UserReservationsModel
            {
                CompanyID = 1,
                FloorPlanID = 2,
                SpaceID = "SPACE202",
                ReservationStartTime = new DateTime(2024, 03, 23, 13, 00, 00), 
                ReservationEndTime = new DateTime(2024, 03, 23, 18, 00, 00),
                UserHash = USER_HASH1 
            };
            TimeSpan maxLeadTime = TimeSpan.FromDays(12);

            // Act
            var response =  _reservationValidationService.ValidateReservationLeadTime(userReservationsModel, maxLeadTime);

            // Assert
            Console.WriteLine(response.ErrorMessage);
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public void ValidateReservationLeadTime_Test_Fail()
        {
            // Arrange
            UserReservationsModel userReservationModel = new UserReservationsModel
            {
                CompanyID = 1,
                FloorPlanID = 2,
                SpaceID = "SPACE202",
                ReservationStartTime = new DateTime(2024, 07, 01, 13, 00, 00), 
                ReservationEndTime = new DateTime(2024, 07, 01, 18, 00, 00), 
                UserHash = USER_HASH1
            };

            TimeSpan maxLeadTime = TimeSpan.FromDays(5);
            // Act
            var response =  _reservationValidationService.ValidateReservationLeadTime(userReservationModel, maxLeadTime);

            // Assert
            Console.WriteLine(response.ErrorMessage);
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public void TestCheckReservtionStatus_Active_Pass()
        {
            Response reservationCheckerResult = new Response();

            UserReservationsModel activeReservation = new UserReservationsModel
            {
                ReservationID = 90,
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationStartTime = new DateTime(2024, 02, 23, 11, 00, 00),
                ReservationEndTime = new DateTime(2024, 02, 23, 12, 00, 00), 
                Status = ReservationStatus.Active,
                UserHash = USER_HASH2
            };

            reservationCheckerResult = _reservationValidationService.checkReservationStatus(activeReservation);
            Assert.IsFalse(reservationCheckerResult.HasError);
            Assert.AreEqual("Reservation is active", reservationCheckerResult.ErrorMessage);
        }

        [TestMethod]
        public void TestCheckReservtionStatus_Cancelled_ReturnsError()
        {
            Response reservationCheckerResult = new Response();

            UserReservationsModel cancelledReservation = new UserReservationsModel
            {
                ReservationID = 90,
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationStartTime = new DateTime(2024, 02, 23, 11, 00, 00), 
                ReservationEndTime = new DateTime(2024, 02, 23, 12, 00, 00), 
                Status = ReservationStatus.Cancelled,
                UserHash = USER_HASH2
            };

            reservationCheckerResult = _reservationValidationService.checkReservationStatus(cancelledReservation);
            Assert.IsTrue(reservationCheckerResult.HasError);
            Assert.AreEqual("Reservation has already been cancelled", reservationCheckerResult.ErrorMessage);
            
        }

        [TestMethod]
        public void TestCheckReservtionStatus_Passed_ReturnsError()
        {
            Response reservationCheckerResult = new Response();

            UserReservationsModel passedReservation = new UserReservationsModel
            {
                ReservationID = 90,
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationStartTime = new DateTime(2024, 02, 23, 11, 00, 00), 
                ReservationEndTime = new DateTime(2024, 02, 23, 12, 00, 00), 
                Status = ReservationStatus.Passed,
                UserHash = USER_HASH2
            };

            reservationCheckerResult = _reservationValidationService.checkReservationStatus(passedReservation);
            Assert.IsFalse(reservationCheckerResult.HasError);
            Assert.AreEqual("Reservation date has passed", reservationCheckerResult.ErrorMessage);
            
        }

    }
       
}


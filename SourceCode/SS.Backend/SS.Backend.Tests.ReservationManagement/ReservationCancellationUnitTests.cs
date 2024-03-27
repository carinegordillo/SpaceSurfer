


using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.ReservationManagement;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
namespace SS.Backend.Tests.ReservationManagement{

    [TestClass]
    public class ReservationCancellationServiceUnitTests
    {
        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private ReservationCreatorService  _ReservationCreatorServiceService;
        private ReservationCancellationService _ReservationCancellationServiceService;

        private ReservationValidationService _reservationValidationService;

        //using dbo.TestReservtaions becaus it allows manual id insertion

        string tableName = "dbo.TestReservations";

        
        
        [TestInitialize]
        public void Setup()
        {

            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);

            _reservationValidationService = new ReservationValidationService();

            _ReservationCreatorServiceService = new ReservationCreatorService(_sqlDao, _reservationValidationService);
            _ReservationCancellationServiceService = new ReservationCancellationService(_sqlDao);


        }

        [TestMethod]
        public async Task TestCancellation()
        {
            // Arrange

            Response reservtaionCreationResult = new Response();
            Response reservtaionCancellationResult = new Response();

            UserReservationsModel reservationToBeCancelled = new UserReservationsModel
            {
                ReservationID = 112,
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationDate = DateTime.Parse("2024-05-01"),
                ReservationStartTime = TimeSpan.Parse("13:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("15:00"), // 2:00 PM as TimeSpan
                Status = ReservationStatus.Active
            };
            
           reservtaionCreationResult = await _ReservationCreatorServiceService.CreateReservationWithManualIDAsync(tableName,reservationToBeCancelled);
           Console.WriteLine(reservtaionCreationResult.ErrorMessage);

            Assert.IsFalse(reservtaionCreationResult.HasError);

            // cancel reservtion
            reservtaionCancellationResult = await _ReservationCancellationServiceService.CancelReservationAsync(tableName, reservationToBeCancelled.ReservationID.Value);
            Assert.IsFalse(reservtaionCancellationResult.HasError);
            Assert.IsTrue(reservtaionCancellationResult.RowsAffected > 0);
            
        }

        [TestMethod]
        public async Task TestCancellationOfNonExistentReservation()
        {
            // Arrange

            Response reservtaionCancellationResult = new Response();

            UserReservationsModel reservationToBeCancelled = new UserReservationsModel
            {
                ReservationID = 578,
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationDate = DateTime.Parse("2024-05-01"),
                ReservationStartTime = TimeSpan.Parse("13:00"), 
                ReservationEndTime = TimeSpan.Parse("15:00"), 
                Status = ReservationStatus.Active
            };

            // cancel reservtion
            reservtaionCancellationResult = await _ReservationCancellationServiceService.CancelReservationAsync(tableName, reservationToBeCancelled.ReservationID.Value);
            Assert.IsTrue(reservtaionCancellationResult.HasError);
            Assert.IsTrue(reservtaionCancellationResult.RowsAffected == 0);
            
        }

        [TestMethod]
    public async Task ReservationStatusUpdater_UpdatesCorrectly(){

        Response reservation1Response = new Response();

        UserReservationsModel reservation1 = new UserReservationsModel
            {
                ReservationID = 927,
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationDate = DateTime.Parse("2024-02-23"),
                ReservationStartTime = TimeSpan.Parse("11:00"), 
                ReservationEndTime = TimeSpan.Parse("12:00"), 
                Status = ReservationStatus.Active
            };

        // Act 1: Create the first reservation
        reservation1Response = await _ReservationCreatorServiceService.CreateReservationWithManualIDAsync(tableName, reservation1);
        Console.WriteLine(reservation1Response.ErrorMessage);
        Assert.IsFalse(reservation1Response.HasError);

        // Reservation 2 is set to a future date
        UserReservationsModel reservation2 = new UserReservationsModel {
            ReservationID = 6765,
            CompanyID = 2,
            FloorPlanID = 3,
            SpaceID = "SPACE302",
            ReservationDate = DateTime.UtcNow, 
            ReservationStartTime = TimeSpan.Parse("14:00"),
            ReservationEndTime = TimeSpan.Parse("18:00"),
            Status = ReservationStatus.Active
        };

        var reservation2Response = await _ReservationCreatorServiceService.CreateReservationWithManualIDAsync(tableName, reservation2);

        Assert.IsFalse(reservation2Response.HasError);

        // Update reservation statuses
        ReservationStatusUpdater reservationStatusUpdater = new ReservationStatusUpdater(_sqlDao);
        var response = await reservationStatusUpdater.UpdateReservtionStatuses(tableName);
        Console.WriteLine(response.ErrorMessage);
        Assert.IsFalse(response.HasError); 
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
                ReservationDate = DateTime.Parse("2024-02-23"),
                ReservationStartTime = TimeSpan.Parse("11:00"), 
                ReservationEndTime = TimeSpan.Parse("12:00"), 
                Status = ReservationStatus.Active
            };

            reservationCheckerResult = _ReservationCancellationServiceService.checkReservationStatus(activeReservation);
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
                ReservationDate = DateTime.Parse("2024-02-23"),
                ReservationStartTime = TimeSpan.Parse("11:00"), 
                ReservationEndTime = TimeSpan.Parse("12:00"), 
                Status = ReservationStatus.Cancelled
            };

            reservationCheckerResult = _ReservationCancellationServiceService.checkReservationStatus(cancelledReservation);
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
                ReservationDate = DateTime.Parse("2024-02-23"),
                ReservationStartTime = TimeSpan.Parse("11:00"), 
                ReservationEndTime = TimeSpan.Parse("12:00"), 
                Status = ReservationStatus.Passed
            };

            reservationCheckerResult = _ReservationCancellationServiceService.checkReservationStatus(passedReservation);
            Assert.IsFalse(reservationCheckerResult.HasError);
            Assert.AreEqual("Reservation date has passed", reservationCheckerResult.ErrorMessage);
            
        }

        [TestCleanup]
        public void Cleanup()
        {

            var testReservtaionIds = new List<int> { 112, 578, 927, 6765, 90};
            var commandBuilder = new CustomSqlCommandBuilder();

            var deleteCommand = commandBuilder.BeginDelete(tableName)
                                            .Where($"reservationID IN ({string.Join(",", testReservtaionIds)})")
                                            .Build();
                                            
            _sqlDao.SqlRowsAffected(deleteCommand);

        }

    }
       
}


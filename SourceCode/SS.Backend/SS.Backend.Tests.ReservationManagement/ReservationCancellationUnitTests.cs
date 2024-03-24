


using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.ReservationManagement;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
namespace SS.Backend.Tests.ReservationManagement{

    [TestClass]
    public class ReservationCancellationUnitTests
    {
        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private SqlCommand _command;
        private ReservationCreation  _reservationcreationService;
        private ReservationCancellation _reservationCancellationService;

        private ReservationValidationService _reservationValidationService;

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

            _reservationcreationService = new ReservationCreation(_sqlDao, _reservationValidationService);
            _reservationCancellationService = new ReservationCancellation(_sqlDao);


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
            
           reservtaionCreationResult = await _reservationcreationService.CreateReservationWithManualID(tableName,reservationToBeCancelled);

            Assert.IsFalse(reservtaionCreationResult.HasError);

            // cancel reservtion
            reservtaionCancellationResult = await _reservationCancellationService.CancelReservation(tableName, reservationToBeCancelled.ReservationID.Value);
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
                ReservationID = 2341,
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationDate = DateTime.Parse("2024-05-01"),
                ReservationStartTime = TimeSpan.Parse("13:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("15:00"), // 2:00 PM as TimeSpan
                Status = ReservationStatus.Active
            };

            // cancel reservtion
            reservtaionCancellationResult = await _reservationCancellationService.CancelReservation(tableName, reservationToBeCancelled.ReservationID.Value);
            Assert.IsTrue(reservtaionCancellationResult.HasError);
            Assert.IsTrue(reservtaionCancellationResult.RowsAffected == 0);
            
        }

        [TestMethod]
    public async Task ReservationStatusUpdater_UpdatesCorrectly(){

        Response reservation1Response = new Response();

        UserReservationsModel reservation1 = new UserReservationsModel
            {
                ReservationID = 1009,
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationDate = DateTime.Parse("2024-03-22"),
                ReservationStartTime = TimeSpan.Parse("13:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("15:00"), // 2:00 PM as TimeSpan
                Status = ReservationStatus.Active
            };

        // Act 1: Create the first reservation
        reservation1Response = await _reservationcreationService.CreateReservationWithManualID(tableName, reservation1);
        Console.WriteLine(reservation1Response.ErrorMessage);
        Assert.IsFalse(reservation1Response.HasError);

        // Reservation 2 is set to a future date
        UserReservationsModel reservation2 = new UserReservationsModel {
            ReservationID = 1010,
            CompanyID = 2,
            FloorPlanID = 3,
            SpaceID = "SPACE302",
            ReservationDate = DateTime.UtcNow, 
            ReservationStartTime = TimeSpan.Parse("13:00"),
            ReservationEndTime = TimeSpan.Parse("15:00"),
            Status = ReservationStatus.Active
        };

        var reservation2Response = await _reservationcreationService.CreateReservationWithManualID(tableName, reservation2);

        Assert.IsFalse(reservation2Response.HasError);

        // Update reservation statuses
        ReservationStatusUpdater reservationStatusUpdater = new ReservationStatusUpdater(_sqlDao);
        var response = await reservationStatusUpdater.updateReservtionStatuses(tableName);
        Console.WriteLine(response.ErrorMessage);
        Assert.IsFalse(response.HasError); 
    }

        
        


        // [TestCleanup]
        // public void Cleanup()
        // {
        //     // clean up the test data
        //     var commandBuilder = new CustomSqlCommandBuilder();
        //     var deleteCommand = commandBuilder.BeginDelete(tableName)
        //         .Where("reservationID = 108")
        //         .Build();
        //     _sqlDao.SqlRowsAffected(deleteCommand);
        // }
        




    }
       
}


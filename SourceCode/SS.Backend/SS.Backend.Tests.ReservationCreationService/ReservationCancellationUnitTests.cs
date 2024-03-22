


using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.ReservationServices;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
namespace SS.Backend.Tests.ReservationCreationService{

    [TestClass]
    public class ReservationUnitTests
    {
        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private SqlCommand _command;
        private ReservationCreation  _reservationcreationService;
        private ReservationCancellation _reservationCancellationService;

        string tableName = "dbo.TestReservations";

        
        
        [TestInitialize]
        public void Setup()
        {

            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);

            _reservationcreationService = new ReservationCreation(_sqlDao);
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
                ReservationID = 104,
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

        




    }
       
}


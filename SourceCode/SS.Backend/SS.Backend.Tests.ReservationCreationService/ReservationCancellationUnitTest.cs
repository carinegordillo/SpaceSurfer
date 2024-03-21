
using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.ReservationServices;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;


namespace SS.Backend.Tests.ReservationCancellationService
{
    [TestClass]
    public class ReservationCancellationUnitTest
    {

        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private SqlCommand _command;

        string tableName = "dbo.Reservations";
        private ReservationCancellation _reservationCancellationService;

        private ReservationCreation _reservationCreationService;

        [TestInitialize]
        public void Setup()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);

            _reservationCancellationService = new ReservationCancellation(_sqlDao);
            _reservationCreationService = new ReservationCreation(_sqlDao);
        }

        [TestMethod]
        public void TestCancellation()
        {
            // Arrange

            UserReservationsModel reservationToBeCancelled = new UserReservationsModel
            {
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationDate = DateTime.Parse("2024-05-01"),
                ReservationStartTime = TimeSpan.Parse("13:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("15:00"), // 2:00 PM as TimeSpan
                Status = ReservationStatus.Active
            };
            
            _reservationCreationService.CreateReservation(tableName, reservationToBeCancelled);
            
            

            // Act
            // TODO: Add code to perform the cancellation

            // Assert
            // TODO: Add assertions to verify the cancellation result
        }
    }
}
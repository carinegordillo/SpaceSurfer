


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
                ReservationID = 108,
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
        


        [TestCleanup]
        public void Cleanup()
        {
            // clean up the test data
            var commandBuilder = new CustomSqlCommandBuilder();
            var deleteCommand = commandBuilder.BeginDelete(tableName)
                .Where("reservationID = 108")
                .Build();
            _sqlDao.SqlRowsAffected(deleteCommand);
        }
        




    }
       
}


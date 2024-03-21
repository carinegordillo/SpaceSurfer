


using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.ReservationCreationService;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
namespace SS.Backend.Tests.ReservationCreationService{

    [TestClass]
    public class ReservationCreationUnitTests
    {
        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private SqlCommand _command;
        private ReservationCreation  _reservationcreationService;

        string tableName = "dbo.Reservations";

        
        
        [TestInitialize]
        public void Setup()
        {

            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);

            _reservationcreationService = new ReservationCreation(_sqlDao);


        }

        

        [TestMethod]
        public async Task AccessReservtaionTable()
        {
            
            
            UserReservationsModel userReservationsModel = new UserReservationsModel
            {
                CompanyID = 1,
                FloorPlanID = 1,
                SpaceID = "Space101",
                ReservationDate = DateTime.Parse("2022-01-01"),
                ReservationStartTime = TimeSpan.Parse("13:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("15:00"), // 2:00 PM as TimeSpan
                Status = "testStatus"
            };


            // Act
            var response = await _reservationcreationService.CreateReservation(tableName,userReservationsModel);
            Console.WriteLine(response.ErrorMessage);
            
            // Assert
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task CheckConflictingReservationsTest()
        {
            Response response = new Response();

            // First reservation
            UserReservationsModel reservation1 = new UserReservationsModel
            {
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationDate = DateTime.Parse("2023-01-01"),
                ReservationStartTime = TimeSpan.Parse("13:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("15:00"), // 2:00 PM as TimeSpan
                Status = "testStatusforFirstReservation"
            };

            // Act 1: Create the first reservation
            response = await _reservationcreationService.CreateReservation(tableName, reservation1);
            Console.WriteLine(response.ErrorMessage);
            Assert.IsFalse(response.HasError);

            // Second reservation which overlaps the first one
            UserReservationsModel reservation2 = new UserReservationsModel
            {
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "Space302",
                ReservationDate = DateTime.Parse("2023-01-01"),
                ReservationStartTime = TimeSpan.Parse("14:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("16:00"), // 2:00 PM as TimeSpan 
                Status = "testStatusforConflictingReservation"
            };

            // Act 2 Check for conflicts before creating the second reservation
            response = await _reservationcreationService.CheckConflictingReservations(reservation2.FloorPlanID, reservation2.SpaceID, reservation2.ReservationStartTime, reservation2.ReservationEndTime);
            Console.WriteLine(response.ErrorMessage);
            
            // Assert Expect a conflict
            Assert.IsTrue(response.HasError);
        }

        [TestMethod]
        public async Task CheckConflictingReservationsTestNoConflict()
        {
            Response response = new Response();

            // First reservation
            UserReservationsModel reservation1 = new UserReservationsModel
            {
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "SPACE302",
                ReservationDate = DateTime.Parse("2025-03-01"),
                ReservationStartTime = TimeSpan.Parse("13:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("14:00"), // 2:00 PM as TimeSpan
                Status = "testStatusforFirstReservationforNONCONFLICTING"
            };

            // Act 1: Create the first reservation
            response = await _reservationcreationService.CreateReservation(tableName, reservation1);
            Console.WriteLine(response.ErrorMessage);
            Assert.IsFalse(response.HasError);

            // Second reservation which overlaps the first one
            UserReservationsModel reservation2 = new UserReservationsModel
            {
                CompanyID = 2,
                FloorPlanID = 3,
                SpaceID = "Space302",
                ReservationDate = DateTime.Parse("2025-03-01"),
                ReservationStartTime = TimeSpan.Parse("16:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("18:00"), // 2:00 PM as TimeSpan
                Status = "testStatusforNONCONFLICTINGReservation"
            };

            // Act 2 Check for conflicts before creating the second reservation
            response = await _reservationcreationService.CheckConflictingReservations(reservation2.FloorPlanID, reservation2.SpaceID, reservation2.ReservationStartTime, reservation2.ReservationEndTime);
            Console.WriteLine(response.ErrorMessage);
            
            // Assert Expect a conflict
            Assert.IsFalse(response.HasError);
        }

        [TestMethod]
        public async Task CheckReservationWithinBusinessHours_Fail(){
        Response response = new Response();

            // First reservation
            UserReservationsModel reservation1 = new UserReservationsModel
            {
                CompanyID = 1,
                FloorPlanID = 2,
                SpaceID = "SPACE202",
                ReservationDate = DateTime.Parse("2023-01-01"),
                ReservationStartTime = TimeSpan.Parse("07:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("08:00"), // 2:00 PM as TimeSpan
                Status = "testStatusforReservationWithinBusinessHours"
            };

            // Act 1: Create the first reservation
            response = await _reservationcreationService.ValidateWithinHours(reservation1.CompanyID, reservation1.ReservationStartTime, reservation1.ReservationEndTime);
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
                ReservationDate = DateTime.Parse("2023-01-01"),
                ReservationStartTime = TimeSpan.Parse("13:00"), // 1:00 PM as TimeSpan
                ReservationEndTime = TimeSpan.Parse("14:00"), // 2:00 PM as TimeSpan
                Status = "testStatusforReservationWithinBusinessHoursFAIL"
            };

            // Act 1: Create the first reservation
            response = await _reservationcreationService.ValidateWithinHours(reservation1.CompanyID, reservation1.ReservationStartTime, reservation1.ReservationEndTime);
            Console.WriteLine(response.ErrorMessage);
            Assert.IsFalse(response.HasError);
        }


    }
       
}


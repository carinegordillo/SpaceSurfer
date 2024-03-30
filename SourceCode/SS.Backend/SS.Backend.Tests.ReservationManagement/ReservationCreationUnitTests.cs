


using SS.Backend.DataAccess;
using System.IO;
using System.Threading.Tasks;
using SS.Backend.ReservationManagement;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
namespace SS.Backend.Tests.ReservationManagement{

    [TestClass]
    public class ReservationCreatorServiceUnitTests
    {
        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private ReservationCreatorService  _ReservationCreatorService;

        private ReservationManagementRepository _reservationManagementRepository;

        private ReservationValidationService _reservationValidationService;

        string AUTO_ID_TABLE = "dbo.NewAutoIDReservations";
        string USER_HASH2 = "hashed_user4";




        
        
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

            _ReservationCreatorService = new ReservationCreatorService(_reservationManagementRepository);


        }

        

        [TestMethod]
        public async Task AccessReservationTable()
        {
            
            
            UserReservationsModel userReservationsModel = new UserReservationsModel
            {
                CompanyID = 1027,
                FloorPlanID = 60,
                SpaceID = "SPACE001",
                ReservationStartTime = new DateTime(2025, 01, 01, 13, 00, 00), 
                ReservationEndTime = new DateTime(2025, 01, 01, 15, 00, 00), 
                Status = ReservationStatus.Active,
                UserHash = USER_HASH2
            };

            var response = await _ReservationCreatorService.CreateReservationWithAutoIDAsync(AUTO_ID_TABLE,userReservationsModel);
            Console.WriteLine(response.ErrorMessage);
            

            Assert.IsFalse(response.HasError);
        }

        ///comment here

    //     [TestMethod]
    //     public async Task CreateReseravtionNoConlficts_Fail()
    //     {
    //         Response response = new Response();


    //         UserReservationsModel reservation1 = new UserReservationsModel
    //         {
    //             CompanyID = 2,
    //             FloorPlanID = 3,
    //             SpaceID = "SPACE302",
    //             ReservationStartTime = new DateTime(2023, 01, 01, 13, 00, 00), 
    //             ReservationEndTime = new DateTime(2023, 01, 01, 15, 00, 00), 
    //             Status = ReservationStatus.Active
    //         };

    //         // Act 1: Create the first reservation
    //         response = await _ReservationCreatorServiceService.CreateReservationWithAutoIDAsync(AUTO_ID_TABLE, reservation1);
    //         Console.WriteLine(response.ErrorMessage);
    //         Assert.IsFalse(response.HasError);

            
    //         UserReservationsModel reservation2 = new UserReservationsModel
    //         {
    //             CompanyID = 2,
    //             FloorPlanID = 3,
    //             SpaceID = "Space302",
    //             ReservationStartTime = new DateTime(2023, 01, 01, 14, 00, 00), 
    //             ReservationEndTime = new DateTime(2023, 01, 01, 16, 00, 00), 
    //             Status = ReservationStatus.Active
    //         };

    //         response = await _ReservationCreatorServiceService.CheckConflictingReservationsAsync(reservation2);
    //         Console.WriteLine(response.ErrorMessage);
    //         Assert.IsTrue(response.HasError);

    //     }


    //     [TestMethod]
    //     public async Task CreateReseravtionNoConlficts_Pass()
    //     {
    //         Response response = new Response();

    //         // First reservation
    //         UserReservationsModel reservation1 = new UserReservationsModel
    //         {
    //             CompanyID = 2,
    //             FloorPlanID = 3,
    //             SpaceID = "SPACE302",
    //             ReservationStartTime = new DateTime(2025, 03, 01, 13, 00, 00), // Jan 1, 2022, 1:00 PM
    //             ReservationEndTime = new DateTime(2025, 03, 01, 14, 00, 00), // Jan 1, 2022, 3:00 PM
    //             Status = ReservationStatus.Active
    //         };

    //         // Act 1: Create the first reservation
    //         response = await _ReservationCreatorServiceService.CreateReservationWithAutoIDAsync(AUTO_ID_TABLE, reservation1);
    //         Console.WriteLine(response.ErrorMessage);
    //         Assert.IsFalse(response.HasError);

    //         // Second reservation which overlaps the first one
    //         UserReservationsModel reservation2 = new UserReservationsModel
    //         {
    //             CompanyID = 2,
    //             FloorPlanID = 3,
    //             SpaceID = "Space302",
    //             ReservationStartTime = new DateTime(2025, 03, 01, 16, 00, 00), 
    //             ReservationEndTime = new DateTime(2025, 03, 01, 18, 00, 00), 
    //             Status = ReservationStatus.Active
    //         };

    //         // Act 2 Check for conflicts before creating the second reservation
    //         response = await _ReservationCreatorServiceService.CheckConflictingReservationsAsync(reservation2);
    //         Console.WriteLine(response.ErrorMessage);
            
    //         // Assert Expect a conflict
    //         Assert.IsFalse(response.HasError);
    //     }

    //     [TestMethod]
    //     public async Task CreateReseravtionBuisnessHoursConflicts_Fail(){
    //     Response response = new Response();

    //         // First reservation
    //         UserReservationsModel reservation1 = new UserReservationsModel
    //         {
    //             CompanyID = 1,
    //             FloorPlanID = 2,
    //             SpaceID = "SPACE202",
    //             ReservationStartTime = new DateTime(2023, 01, 01, 07, 00, 00), // Jan 1, 2022, 1:00 PM
    //             ReservationEndTime = new DateTime(2023, 01, 01, 08, 00, 00), // Jan 1, 2022, 3:00 PM
    //             Status = ReservationStatus.Active
    //         };

    //         // Act 1: Create the first reservation
    //         response = await _ReservationCreatorServiceService.ValidateWithinHoursAsync(reservation1.CompanyID, reservation1);
    //         Console.WriteLine(response.ErrorMessage);
    //         Assert.IsTrue(response.HasError);
    //     }

    //     [TestMethod]
    //     public async Task CreateReservationNoBuisnessHoursConflicts_Pass(){
    //     Response response = new Response();

    //         // First reservation
    //         UserReservationsModel reservation1 = new UserReservationsModel
    //         {
    //             CompanyID = 1,
    //             FloorPlanID = 2,
    //             SpaceID = "SPACE202",
    //             ReservationStartTime = new DateTime(2023, 01, 01, 13, 00, 00), // Jan 1, 2022, 1:00 PM
    //             ReservationEndTime = new DateTime(2023, 01, 01, 14, 00, 00), // Jan 1, 2022, 3:00 PM
    //             Status = ReservationStatus.Active
    //         };

    //         // Act 1: Create the first reservation
    //         response = await _ReservationCreatorServiceService.ValidateWithinHoursAsync(reservation1.CompanyID, reservation1);
    //         Console.WriteLine(response.ErrorMessage);
    //         Assert.IsFalse(response.HasError);
    //     }

    //     [TestMethod]
    //     public async Task CreateReservationValidDuration_Pass()
    //     {
    //         // Arrange
    //         UserReservationsModel userReservationsModel = new UserReservationsModel
    //         {
    //             CompanyID = 1,
    //             FloorPlanID = 2,
    //             SpaceID = "SPACE202",
    //             ReservationStartTime = new DateTime(2026, 01, 01, 07, 00, 00), 
    //             ReservationEndTime = new DateTime(2026, 01, 01, 08, 00, 00), 
    //             Status = ReservationStatus.Active
    //         };

    //         // Act
    //         var response = await _ReservationCreatorServiceService.ValidateReservationDurationAsync(userReservationsModel);

    //         // Assert
    //         Console.WriteLine(response.ErrorMessage);
    //         Assert.IsFalse(response.HasError);
    //     }

    //     [TestMethod]
    //     public async Task CreateReservationInvalidDuration_Fail()
    //     {
    //         // Arrange
    //         UserReservationsModel userReservationsModel = new UserReservationsModel
    //         {
    //             CompanyID = 1,
    //             FloorPlanID = 2,
    //             SpaceID = "SPACE202",
    //             ReservationStartTime = new DateTime(2026, 01, 01, 13, 00, 00), 
    //             ReservationEndTime = new DateTime(2026, 01, 01, 18, 00, 00), 
    //             Status = ReservationStatus.Active
    //         };

    //         // Act
    //         var response = await _ReservationCreatorServiceService.ValidateReservationDurationAsync(userReservationsModel);

    //         // Assert
    //         Console.WriteLine(response.ErrorMessage);
    //         Assert.IsTrue(response.HasError);
    //     }

    //     [TestMethod]
    //     public void CreateReservationValidLeadTime_Pass()
    //     {
    //         // Arrange
    //         UserReservationsModel userReservationsModel = new UserReservationsModel
    //         {
    //             CompanyID = 1,
    //             FloorPlanID = 2,
    //             SpaceID = "SPACE202",
    //             ReservationStartTime = new DateTime(2024, 03, 23, 13, 00, 00), 
    //             ReservationEndTime = new DateTime(2024, 03, 23, 18, 00, 00), 
    //         };
    //         int maxLeadTime = 5;
    //         TimeUnit unitOfTime = TimeUnit.Days;

    //         // Act
    //         var response =  _ReservationCreatorServiceService.ValidateReservationLeadTime(userReservationsModel, maxLeadTime, unitOfTime);

    //         // Assert
    //         Console.WriteLine(response.ErrorMessage);
    //         Assert.IsFalse(response.HasError);
    //     }

    //     [TestMethod]
    //     public void CreateReservationInvalidLeadTime_Fail()
    //     {
    //         // Arrange
    //         UserReservationsModel userReservationModel = new UserReservationsModel
    //         {
    //             CompanyID = 1,
    //             FloorPlanID = 2,
    //             SpaceID = "SPACE202",
    //             ReservationStartTime = new DateTime(2024, 07, 01, 13, 00, 00), 
    //             ReservationEndTime = new DateTime(2024, 07, 01, 18, 00, 00), 
    //         };
    //         int maxLeadTime = 5;
    //         TimeUnit unitOfTime = TimeUnit.Days;

    //         // Act
    //         var response =  _ReservationCreatorServiceService.ValidateReservationLeadTime(userReservationModel, maxLeadTime, unitOfTime);

    //         // Assert
    //         Console.WriteLine(response.ErrorMessage);
    //         Assert.IsTrue(response.HasError);
    //     }

    // }
       
    }
}


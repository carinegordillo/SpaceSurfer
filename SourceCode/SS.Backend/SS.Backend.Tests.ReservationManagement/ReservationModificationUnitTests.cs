


// using SS.Backend.DataAccess;
// using System.IO;
// using System.Threading.Tasks;
// using SS.Backend.ReservationManagement;
// using SS.Backend.SharedNamespace;
// using Microsoft.Data.SqlClient;
// namespace SS.Backend.Tests.ReservationManagement{

//     [TestClass]
//     public class ReservationModificationServiceUnitTests
//     {
//         private SqlDAO _sqlDao;
//         private ConfigService _configService;
//         private ReservationCreatorService  _reservationCreatorService;

//         private ReservationManagementRepository _reservationManagementRepository;

//         private ReservationValidationService _reservationValidationService;

//         string AUTO_ID_TABLE = "dbo.NewAutoIDReservations";
        

//         [TestInitialize]
//         public void Setup()
//         {

//             var baseDirectory = AppContext.BaseDirectory;
//             var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//             var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
//             _configService = new ConfigService(configFilePath);
//             _sqlDao = new SqlDAO(_configService);

            

//             _reservationManagementRepository = new ReservationManagementRepository(_sqlDao);

//             _reservationValidationService = new ReservationValidationService(_reservationManagementRepository);

//             _reservationCreatorService= new ReservationCreatorService(_reservationManagementRepository);



//         }

        
//         [TestMethod]
//         public async Task ModifyReservationTimes_SuccessfullyUpdatesTimes()
//         {
           
//             var reservationId = SetupTestReservation(); 

//             var service = new ReservationValidationService();
//             UserReservationsModel userReservationsModel = new UserReservationsModel
//             {
//                 CompanyID = 1,
//                 FloorPlanID = 1,
//                 SpaceID = "Space101",
//                 ReservationStartTime = new DateTime(2025, 01, 01, 13, 00, 00), 
//                 ReservationEndTime = new DateTime(2025, 01, 01, 15, 00, 00), 
//                 Status = ReservationStatus.Active,
//                 UserHash = "testUserHash4"
//             };

            

//             var model = new UserReservationsModel
//             {
//                 ReservationID = reservationId,
//                 ReservationStartTime = DateTime.UtcNow.AddDays(1),
//                 ReservationEndTime = DateTime.UtcNow.AddDays(1).AddHours(2)
//             };
            
//             // Act
//             var response = await _ReservationCreatorServiceService.CreateReservationWithAutoIDAsync(AUTO_ID_TABLE,userReservationsModel);

//             // Act
//             var response = await service.ModifyReservationTimes("NewAutoIDReservations", model);

//             // Assert
//             Assert.IsFalse(response.HasError);

//             // Cleanup
//             CleanupTestReservation(reservationId); // Implement this to remove the test reservation.
//         }
//     }
// }



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
//         private ReservationCreatorService  _ReservationCreatorServiceService;

//         private ReservationManagementRepository _reservationManagementRepository;

//         private ReservationValidationService _reservationValidationService;

//         string AUTO_ID_TABLE = "dbo.NewAutoIDReservations";
//         string USER_HASH2 = "testUserHash2";




        
        
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

//             _ReservationCreatorServiceService = new ReservationCreatorService(_reservationManagementRepository);



//         }

        
//         [TestMethod]
//         public async Task ModifyReservationTimes_SuccessfullyUpdatesTimes()
//         {
           
//             var reservationId = SetupTestReservation(); 

//             var service = new ReservationValidationService();
//             var model = new UserReservationsModel
//             {
//                 ReservationID = reservationId,
//                 ReservationStartTime = DateTime.UtcNow.AddDays(1),
//                 ReservationEndTime = DateTime.UtcNow.AddDays(1).AddHours(2)
//             };

//             // Act
//             var response = await service.ModifyReservationTimes("NewAutoIDReservations", model);

//             // Assert
//             Assert.IsFalse(response.HasError);

//             // Cleanup
//             CleanupTestReservation(reservationId); // Implement this to remove the test reservation.
//         }
//     }
// }
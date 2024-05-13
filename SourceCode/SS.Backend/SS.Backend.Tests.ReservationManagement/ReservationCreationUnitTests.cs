


//using SS.Backend.DataAccess;
//using System.IO;
//using System.Threading.Tasks;
//using SS.Backend.Waitlist;
//using SS.Backend.ReservationManagement;
//using SS.Backend.SharedNamespace;
//using Microsoft.Data.SqlClient;
//using SS.Backend.Services.LoggingService;
//namespace SS.Backend.Tests.ReservationManagement{

//   [TestClass]
//   public class ReservationCreatorServiceUnitTests
//   {
//       private SqlDAO _sqlDao;
//       private ConfigService _configService;
//       private ReservationCreatorService  _ReservationCreatorService;

//       private ReservationManagementRepository _reservationManagementRepository;

//       private ReservationValidationService _reservationValidationService;
//       private ILogTarget _logTarget;
//       private  ILogger _logger;
//       private WaitlistService waitlistService;


//       string AUTO_ID_TABLE = "dbo.NewAutoIDReservations";
//       string USER_HASH = "Yu86Ho6KDmtOeP687I/AHNE4rhxoCzZDs9v/Mpe+SZw=";




        
        
//       [TestInitialize]
//       public void Setup()
//       {

//           var baseDirectory = AppContext.BaseDirectory;
//           var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//           var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
//           _configService = new ConfigService(configFilePath);
//           _sqlDao = new SqlDAO(_configService);
//           _logger = new Logger(_logTarget);
//           waitlistService = new WaitlistService(_sqlDao);

            

//           _reservationManagementRepository = new ReservationManagementRepository(_sqlDao);

//           _reservationValidationService = new ReservationValidationService(_reservationManagementRepository,_logger);

//           _ReservationCreatorService = new ReservationCreatorService(_reservationManagementRepository,waitlistService);


//       }


//       [TestMethod]
//       public async Task AccessReservationTable()
//       {
            
//           UserReservationsModel userReservationsModel = new UserReservationsModel
//           {
//               CompanyID = 3,
//               FloorPlanID = 3,
//               SpaceID = "S3-FP3",
//               ReservationStartTime = new DateTime(2025, 01, 01, 13, 00, 00), 
//               ReservationEndTime = new DateTime(2025, 01, 01, 15, 00, 00), 
//               Status = ReservationStatus.Active,
//               UserHash = USER_HASH
//           };

//           var response = await _ReservationCreatorService.CreateReservationWithAutoIDAsync(AUTO_ID_TABLE,userReservationsModel);
//           Console.WriteLine(response.ErrorMessage);
            

//           Assert.IsFalse(response.HasError);
//       }


//       [TestMethod]
//       public async Task CreatReservationInReservationTable()
//       {
//           Response response = new Response();


//           UserReservationsModel reservation1 = new UserReservationsModel
//           {
//               CompanyID = 2,
//               FloorPlanID = 2,
//               SpaceID = "S2-FP2",
//               UserHash = USER_HASH,
//               ReservationStartTime = new DateTime(2023, 01, 01, 13, 00, 00), 
//               ReservationEndTime = new DateTime(2023, 01, 01, 15, 00, 00), 
//               Status = ReservationStatus.Active
//           };

//           // Act 1: Create the first reservation
//           response = await _ReservationCreatorService.CreateReservationWithAutoIDAsync(AUTO_ID_TABLE, reservation1);
//           Console.WriteLine(response.ErrorMessage);
//           Assert.IsFalse(response.HasError);


//       }


//       [TestMethod]
//       public async Task CreateReseravtion_Pass()
//       {
//           Response response = new Response();

//           // First reservation
//           UserReservationsModel reservation1 = new UserReservationsModel
//           {
//               CompanyID = 2,
//               FloorPlanID = 2,
//               SpaceID = "S1-FP2",
//               UserHash = USER_HASH,
//               ReservationStartTime = new DateTime(2025, 03, 01, 13, 00, 00), // Jan 1, 2022, 1:00 PM
//               ReservationEndTime = new DateTime(2025, 03, 01, 14, 00, 00), // Jan 1, 2022, 3:00 PM
//               Status = ReservationStatus.Active
//           };

//           // Act 1: Create the first reservation
//           response = await _ReservationCreatorService.CreateReservationWithAutoIDAsync(AUTO_ID_TABLE, reservation1);
//           Console.WriteLine(response.ErrorMessage);
//           Assert.IsFalse(response.HasError);
//       }

    

//   }
       
//}



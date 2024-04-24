


//using SS.Backend.DataAccess;
//using System.IO;
//using System.Threading.Tasks;
//using SS.Backend.ReservationManagement;
//using SS.Backend.SharedNamespace;
//using Microsoft.Data.SqlClient;
//namespace SS.Backend.Tests.ReservationManagement{

//    [TestClass]
//    public class ReservationModificationServiceUnitTests
//    {
//        private SqlDAO _sqlDao;
//        private ConfigService _configService;
//        private IReservationCreatorService  _reservationCreatorService;

//        private IReservationManagementRepository _reservationManagementRepository;

//        private IReservationValidationService _reservationValidationService;

//        private ReservationModificationService _reservationModificationService;

//        string MANUAL_ID_TABLE = "dbo.NewManualIDReservations";

//        string USER_HASH = "Yu86Ho6KDmtOeP687I/AHNE4rhxoCzZDs9v/Mpe+SZw=";
        

//        [TestInitialize]
//        public void Setup()
//        {

//            var baseDirectory = AppContext.BaseDirectory;
//            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
//            _configService = new ConfigService(configFilePath);
//            _sqlDao = new SqlDAO(_configService);

            

//            _reservationManagementRepository = new ReservationManagementRepository(_sqlDao);

//            _reservationValidationService = new ReservationValidationService(_reservationManagementRepository);

//            _reservationCreatorService= new ReservationCreatorService(_reservationManagementRepository);
//            _reservationModificationService = new ReservationModificationService(_reservationManagementRepository);



//        }

        
//        [TestMethod]
//        public async Task ModifyReservationTimes_SuccessfullyUpdatesTimes()
//        {
//            Response creationResponse = new Response();
//            Response modificationResposne = new Response();
           
//            UserReservationsModel userReservationsModel = new UserReservationsModel
//            {
//                ReservationID = 2001,
//                CompanyID = 1,
//                FloorPlanID = 1,
//                SpaceID = "S2-FP1",
//                ReservationStartTime = new DateTime(2025, 01, 01, 13, 00, 00), 
//                ReservationEndTime = new DateTime(2025, 01, 01, 15, 00, 00), 
//                Status = ReservationStatus.Active,
//                UserHash = USER_HASH
//            };

//            creationResponse = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE,userReservationsModel);

//            Assert.IsFalse(creationResponse.HasError);

//            userReservationsModel.ReservationStartTime = new DateTime(2025, 01, 02, 14, 00, 00); 
//            userReservationsModel.ReservationEndTime = new DateTime(2025, 01, 02, 16, 00, 00);

//            modificationResposne = await _reservationModificationService.ModifyReservationTimes(MANUAL_ID_TABLE, userReservationsModel);
//            Console.WriteLine(modificationResposne.ErrorMessage);

//            Assert.IsFalse(modificationResposne.HasError);

//        }

//        [TestCleanup]
//        public void Cleanup()
//        {

//            var testReservtaionIds = new List<int> { 2001};
//            var commandBuilder = new CustomSqlCommandBuilder();

//            var deleteCommand = commandBuilder.BeginDelete(MANUAL_ID_TABLE)
//                                            .Where($"reservationID IN ({string.Join(",", testReservtaionIds)})")
//                                            .Build();
                                            
//            _sqlDao.SqlRowsAffected(deleteCommand);

//        }
//    }
//}

using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using SS.Backend.SharedNamespace;
using SS.Backend.Services.LoggingService;
using SS.Backend.Waitlist;

namespace SS.Backend.Tests.ReservationManagers{

   [TestClass]
   public class ReservationModificationManagerTests
   {
       private SqlDAO _sqlDao;
       private ConfigService _configService;
       private IReservationCreatorService  _reservationCreatorService;

       private IReservationManagementRepository _reservationManagementRepository;

       private IReservationValidationService _reservationValidationService;

       private IReservationModificationService _reservationModificationService;

       private IReservationModificationManager _reservationModificationManager;
       private ILogTarget _logTarget;

       private ILogger _logger;

       private WaitlistService _waitlistService;

       //uses newManualIDReservations because it allows manual id insertion

       string MANUAL_ID_TABLE = "dbo.NewManualIDReservations";

        


       string userHash1 = "Yu86Ho6KDmtOeP687I/AHNE4rhxoCzZDs9v/Mpe+SZw=";

       [TestInitialize]
       public void Setup()
       {
           var baseDirectory = AppContext.BaseDirectory;
           var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
           var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
           _configService = new ConfigService(configFilePath);
           _sqlDao = new SqlDAO(_configService);

           //logging stuff
            _logTarget = new SqlLogTarget(_sqlDao);
            _logger = new Logger(_logTarget);

            //waitlist stuff
            _waitlistService = new WaitlistService(_sqlDao);

            

           _reservationManagementRepository = new ReservationManagementRepository(_sqlDao);

           _reservationValidationService = new ReservationValidationService(_reservationManagementRepository);

           _reservationCreatorService = new ReservationCreatorService(_reservationManagementRepository, _waitlistService);
            
           _reservationModificationService = new ReservationModificationService(_reservationManagementRepository);

           _reservationModificationManager = new ReservationModificationManager(_reservationModificationService, _reservationValidationService,_logger);
       }

       [TestMethod]
       public async Task ModifySpaceSurferSpaceReservation_Success()
       {
           DateTime now = DateTime.Now;
           DateTime reservationStart = new DateTime(now.Year, now.Month, now.Day, 16, 0, 0).AddDays(1);
           DateTime reservationEnd = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0).AddDays(1);

           UserReservationsModel userReservationsModel = new UserReservationsModel
           {
               ReservationID = 2002,
               CompanyID = 1,
               FloorPlanID = 4,
               SpaceID = "S5-FP4",
               ReservationStartTime = reservationStart,
               ReservationEndTime = reservationEnd,
               UserHash = userHash1
           };

           var response = await _reservationCreatorService.CreateReservationWithManualIDAsync(MANUAL_ID_TABLE,userReservationsModel);

           Console.WriteLine(response.ErrorMessage);

           Assert.IsFalse(response.HasError);

           userReservationsModel.ReservationStartTime = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0).AddDays(2);
           userReservationsModel.ReservationEndTime = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0).AddDays(2);

           response = await _reservationModificationManager.ModifySpaceSurferSpaceReservationAsync(userReservationsModel, MANUAL_ID_TABLE);
           //check again if the reservation was modified

           Console.WriteLine(response.ErrorMessage);

           Assert.IsFalse(response.HasError);


       }

       [TestCleanup]
       public void Cleanup()
       {

           var testReservtaionIds = new List<int> { 2002};
           var commandBuilder = new CustomSqlCommandBuilder();

           var deleteCommand = commandBuilder.BeginDelete(MANUAL_ID_TABLE)
                                           .Where($"reservationID IN ({string.Join(",", testReservtaionIds)})")
                                           .Build();
                                            
           _sqlDao.SqlRowsAffected(deleteCommand);

       }

       // [TestMethod]
       // public async Task CreateSpaceSurferSpaceReservation_InvalidBusinessHours_InvalidMaxDuration()
       // {
       //     DateTime now = DateTime.Now;
       //     DateTime reservationStart = new DateTime(now.Year, now.Month, now.Day, 13, 0, 0).AddDays(1);
       //     DateTime reservationEnd = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0).AddDays(1);
       //     // Arrange
       //     UserReservationsModel userReservationsModel = new UserReservationsModel
       //     {
       //         CompanyID = 1,
       //         FloorPlanID = 2,
       //         SpaceID = "SPACE202",
       //         ReservationStartTime = reservationStart,
       //         ReservationEndTime = reservationEnd,
       //         UserHash = userHash1
       //     };

       //     var response = await _reservationCreationManager.CreateSpaceSurferSpaceReservationAsync(TABLE_NAME, userReservationsModel);

       //     Assert.IsTrue(response.HasError, "Expected no error in reservation creation.");
       //     Assert.IsTrue(response.ErrorMessage.Contains("Reservation did not pass validation checks"));
       //     Assert.IsTrue(response.ErrorMessage.Contains("CheckBusinessHours, MaxDurationPerSeat"));
       // }
   }
}
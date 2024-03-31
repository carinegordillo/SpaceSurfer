
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using SS.Backend.SharedNamespace;
using SS.Backend.SpaceManager;
using System.Data;


namespace SS.Backend.Tests.ReservationManagers{

    [TestClass]
    public class AvailibilityDisplayManagerTest
    {
        private SqlDAO _sqlDao;
        private ConfigService _configService;
        private ISpaceManagerDao _spaceManagerDao; 

        private SpaceReader _spaceReader;

        private ReservationManagementRepository _reservationManagementRepository;

        private ReservationValidationService _reservationValidationService;

        private AvailabilityDisplayManager _availabilityDisplayManager;


        //uses newManualIDReservations because it allows manual id insertion


        [TestInitialize]
        public void Setup()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);

            //reader stuff
            _spaceManagerDao = new SpaceManagerDao(_sqlDao);
            _spaceReader = new SpaceReader(_spaceManagerDao);

            //validation stuff

            _reservationManagementRepository = new ReservationManagementRepository(_sqlDao);

            _reservationValidationService = new ReservationValidationService(_reservationManagementRepository);

            _availabilityDisplayManager = new AvailabilityDisplayManager( _reservationValidationService, _spaceReader);

           
        }

        [TestMethod]
        public async Task CheckAvailability_NoConflicts_ReturnsAllSpacesAsAvailable()
        {
            int testCompanyId = 1027;
            DateTime start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 9, 0, 0); // Today at 9:00 AM
            DateTime end = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 12, 0, 0); // Today at 12:00 PM

            var availableSpaces = await _availabilityDisplayManager.CheckAvailabilityAsync(testCompanyId, start, end);

            Assert.IsNotNull(availableSpaces);
            
        }


    }
}
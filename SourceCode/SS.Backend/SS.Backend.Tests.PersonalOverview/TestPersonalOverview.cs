using SS.Backend.DataAccess;
using SS.Backend.Services.PersonalOverviewService;
using SS.Backend.SharedNamespace;


namespace SS.Backend.Tests.PersonalOverviewService
{

    [TestClass]
    public class TestPersonalOverview
    {
        // Declaring necessary class variables
        private Response response;
        private CustomSqlCommandBuilder commandBuilder;
        private ISqlDAO sqlDAO;
        private ConfigService configService;
        private PersonalOverview _personalOverviewService;
        private IPersonalOverviewDAO _personalOverviewDAOService;

        // Method to initialize test environment
        [TestInitialize]
        public void TestInitialize()
        {
            // Initializing variables
            response = new Response();
            commandBuilder = new CustomSqlCommandBuilder();

            // Getting configuration file path
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

            // Creating ConfigService instance
            configService = new ConfigService(configFilePath);
            // Creating SqlDAO instance
            sqlDAO = new SqlDAO(configService);
            // Creating PersonalOverviewDAO instance
            _personalOverviewDAOService = new PersonalOverviewDAO(sqlDAO);
            // Creating PersonalOverview instance
            _personalOverviewService = new PersonalOverview(_personalOverviewDAOService);
        }

        // Method to test retrieving reservation list without filters
        [TestMethod]
        public async Task GetReservationList_Success()
        {
            // Setting up username
            var username = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=";

            // Getting user reservations asynchronously
            var reservations = await _personalOverviewService.GetUserReservationsAsync(username, null, null);

            // Asserting that reservations are not null and contain at least one entry
            Assert.IsNotNull(reservations);
            Assert.IsTrue(reservations.Any());
        }

        // Method to test retrieving reservation list with date filters
        [TestMethod]
        public async Task GetReservationListFiltered_Success()
        {
            // Setting up username and date range
            var username = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=";
            DateOnly fromDate = new DateOnly(2024, 3, 1);
            DateOnly toDate = new DateOnly(2024, 4, 10);

            // Getting user reservations asynchronously with date filters
            var reservations = await _personalOverviewService.GetUserReservationsAsync(username, fromDate, toDate);

            // Asserting that reservations are not null and contain at least one entry
            Assert.IsNotNull(reservations);
            Assert.IsTrue(reservations.Any());
        }

        // Method to test retrieving reservation list from a specific date
        [TestMethod]
        public async Task GetReservationListFromDate_Success()
        {
            // Setting up username and from date
            var username = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=";
            DateOnly fromDate = new DateOnly(2024, 4, 1);

            // Getting user reservations asynchronously from a specific date
            var reservations = await _personalOverviewService.GetUserReservationsAsync(username, fromDate, null);

            // Asserting that reservations are not null and contain at least one entry
            Assert.IsNotNull(reservations);
            Assert.IsTrue(reservations.Any());
        }

        // Method to test retrieving reservation list up to a specific date
        [TestMethod]
        public async Task GetReservationListToDate_Success()
        {
            // Setting up username and to date
            var username = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=";
            DateOnly toDate = new DateOnly(2024, 5, 28);

            // Getting user reservations asynchronously up to a specific date
            var reservations = await _personalOverviewService.GetUserReservationsAsync(username, null, toDate);

            // Asserting that reservations are not null and contain at least one entry
            Assert.IsNotNull(reservations);
            Assert.IsTrue(reservations.Any());
        }

        [TestMethod]
        public async Task GetReservationListInvalidDate_Success()
        {
            var Error = false;
            var username = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=";

            try
            {
                DateOnly toDate = new DateOnly(2024, 31, 01);
                var reservations = await _personalOverviewService.GetUserReservationsAsync(username, null, toDate);
            }
            catch (Exception ex)
            {
                Error = true;
                Console.Write(ex.ToString());
            }

            // Asserting that toDate is False
            Assert.IsTrue(Error);
        }

        // Method to test failure scenario when retrieving reservation list for a user with no reservations
        [TestMethod]
        public async Task GetReservationList_Failure()
        {
            // Setting up username for a user with no reservations
            var username = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hRW=";

            // Getting user reservations asynchronously
            var reservations = await _personalOverviewService.GetUserReservationsAsync(username, null, null);

            // Asserting that reservations are not null and do not contain any entries
            Assert.IsNotNull(reservations);
            Assert.IsFalse(reservations.Any());
        }


        // Method to test failure scenario when retrieving reservation list for a user with no reservations within specified date range
        [TestMethod]
        public async Task GetReservationListFiltered_Failure()
        {
            // Setting up username for a user with no reservations
            var username = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hRW=";
            // Setting up date range
            DateOnly fromDate = new DateOnly(2023, 2, 1);
            DateOnly toDate = new DateOnly(2025, 4, 15);

            // Getting user reservations asynchronously with date filters
            var reservations = await _personalOverviewService.GetUserReservationsAsync(username, fromDate, toDate);

            // Asserting that reservations are not null and do not contain any entries
            Assert.IsNotNull(reservations);
            Assert.IsFalse(reservations.Any());
        }

        // Method to test failure scenario when retrieving reservation list for a user with no reservations from a specific date
        [TestMethod]
        public async Task GetReservationListFromDate_Failure()
        {
            // Setting up username for a user with no reservations
            var username = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hRW=";
            // Setting up from date
            DateOnly fromDate = new DateOnly(2023, 1, 1);

            // Getting user reservations asynchronously from a specific date
            var reservations = await _personalOverviewService.GetUserReservationsAsync(username, fromDate, null);

            // Asserting that reservations are not null and do not contain any entries
            Assert.IsNotNull(reservations);
            Assert.IsFalse(reservations.Any());
        }

        // Method to test failure scenario when retrieving reservation list for a user with no reservations up to a specific date
        [TestMethod]
        public async Task GetReservationListToDate_Failure()
        {
            // Setting up username for a user with no reservations
            var username = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hRW=";
            // Setting up to date
            DateOnly toDate = new DateOnly(2024, 7, 30);

            // Getting user reservations asynchronously up to a specific date
            var reservations = await _personalOverviewService.GetUserReservationsAsync(username, null, toDate);

            // Asserting that reservations are not null and do not contain any entries
            Assert.IsNotNull(reservations);
            Assert.IsFalse(reservations.Any());
        }

        // Method to test successful deletion of user reservations
        [TestMethod]
        public async Task DeleteUserReservationsAsync_Success()
        {
            // Creating a new response instance
            Response result = new Response();

            // Inserting a reservation for a user
            result = await InsertReservation();

            // Setting up username and finding reservation ID
            var username = "R9mXz33hkJew2FYYb28c8cb5jMnTjvEiqaJMizy/uoE=";
            var reservationID = await FindReservationID();

            // Attempting to delete user reservations
            result = await _personalOverviewService.DeleteUserReservationsAsync(username, reservationID);

            // Asserting that result is not null and has no errors
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasError);
        }

        // Method to test failure scenario when trying to delete reservations by a user other than the one who made them
        [TestMethod]
        public async Task NoDeleteUsersReservationByOtherUserAsync_Success()
        {
            // Creating a new response instance
            Response result = new Response();

            // Inserting a reservation for a user
            result = await InsertReservation(); // reservationID inserted by from R9mXz33hkJew2FYYb28c8cb5jMnTjvEiqaJMizy/uoE= 

            // Setting up username and finding reservation ID (user attempts to delete reservation made by another user)
            var username = "HCGl3rHu5KQyzNKfiLlm7ZMg0eCDSjxs1ZMWWp8E7Uw=";
            var reservationID = await FindReservationID(); // 'HCGl3rHu5KQyzNKfiLlm7ZMg0eCDSjxs1ZMWWp8E7Uw=' tries to delete the reservationID inserted by 'R9mXz33hkJew2FYYb28c8cb5jMnTjvEiqaJMizy/uoE='

            // Attempting to delete user reservations
            result = await _personalOverviewService.DeleteUserReservationsAsync(username, reservationID);

            // Asserting that result is not null and has errors
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasError);

            // Cleaning up by deleting the inserted reservation
            username = "R9mXz33hkJew2FYYb28c8cb5jMnTjvEiqaJMizy/uoE=";
            await _personalOverviewService.DeleteUserReservationsAsync(username, reservationID);
        }

        // Method to test failure scenario when trying to delete non-existing reservations
        [TestMethod]
        public async Task DeleteUserReservationsAsync_Failure()
        {
            // Creating a new response instance
            Response result = new Response();

            // Setting up username and finding non-existing reservation ID
            var username = "R9mXz33hkJew2FYYb28c8cb5jMnTjvEiqaJMizy/uoH=";
            var reservationID = await FindReservationID(); // reservationID does not exist

            // Attempting to delete user reservations
            result = await _personalOverviewService.DeleteUserReservationsAsync(username, reservationID);

            // Asserting that result is not null and has errors
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasError);
        }

        // Method to insert a reservation into the database
        public async Task<Response> InsertReservation()
        {
            // Creating a new response instance
            Response response = new Response();

            try
            {
                commandBuilder = new CustomSqlCommandBuilder();

                // Constructing SQL query to insert a reservation
                var query = commandBuilder.BeginInsert("dbo.reservations ([companyID], [floorPlanID], [spaceID], [reservationStartTime], [reservationEndTime], [status], [userHash]) " +
                    "VALUES (3, 1, 'CE5', '2024-08-24 12:00:00.000', '2024-08-24 15:00:00.000', 'Active', 'R9mXz33hkJew2FYYb28c8cb5jMnTjvEiqaJMizy/uoE=')").Build();

                // Executing SQL query to insert reservation and getting response
                response = await sqlDAO.SqlRowsAffected(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test: {ex}");
            }

            return response;
        }

        // Method to find a reservation ID in the database
        public async Task<int> FindReservationID()
        {
            // Initializing reservation ID variable
            int reservationID = 0;
            // Creating a new response instance
            Response result = new Response();

            try
            {
                commandBuilder = new CustomSqlCommandBuilder();

                // Constructing SQL query to find reservation ID
                var query = commandBuilder.BeginSelectString("reservationID")
                                          .From("dbo.reservations")
                                          .Where("companyID = 3 AND floorPlanID = 1 AND spaceID = 'CE5' " +
                                                 "AND reservationStartTime = '2024-08-24 12:00:00.000' " +
                                                 "AND reservationEndTime = '2024-08-24 15:00:00.000' " +
                                                 "AND status = 'Active' " +
                                                 "AND userHash = 'R9mXz33hkJew2FYYb28c8cb5jMnTjvEiqaJMizy/uoE=';")
                                          .Build();

                // Executing SQL query to find reservation ID and getting response
                result = await sqlDAO.ReadSqlResult(query);

                // Checking if the result is not null before retrieving reservation ID
                reservationID = Convert.ToInt32(result.ValuesRead?.Rows[0]?["reservationID"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test: {ex}");
            }

            return reservationID;
        }
    }
}

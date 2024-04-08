using SS.Backend.DataAccess;
using SS.Backend.Services.PersonalOverviewService;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Tests.PersonalOverviewService
{
    [TestClass]
    public class TestPersonalOverview
    {
        private Response response;
        private CustomSqlCommandBuilder commandBuilder;
        private ISqlDAO sqlDAO;
        private ConfigService configService;
        private PersonalOverview _personalOverviewService;
        private IPersonalOverviewDAO _personalOverviewDAOService;


        [TestInitialize]
        public void TestInitialize()
        {
            response = new Response();
            commandBuilder = new CustomSqlCommandBuilder();

            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

            configService = new ConfigService(configFilePath);
            sqlDAO = new SqlDAO(configService);
            _personalOverviewDAOService = new PersonalOverviewDAO(sqlDAO);
            _personalOverviewService = new PersonalOverview(_personalOverviewDAOService);
        }

        [TestMethod]
        public async Task GetReservationList_Success()
        {
            var username = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=";
            DateOnly fromDate = new DateOnly(2024, 3, 1);
            DateOnly toDate = new DateOnly(2024, 4, 10);

            var reservations = await _personalOverviewService.GetUserReservationsAsync(username, fromDate, toDate);

            Assert.IsNotNull(reservations);
            Assert.IsTrue(reservations.Any());

        }

        [TestMethod]
        public async Task DeleteUserReservationsAsync_Success()
        {

            await InsertReservation();

            var username = "HCGl3rHu5KQyzNKfiLlm7ZMg0eCDSjxs1ZMWWp8E7Uw=";
            var reservationID = await FindReservationID();

            // Act
            var response = await _personalOverviewService.DeleteUserReservationsAsync(username, reservationID);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(response.HasError);
        }


        public async Task InsertReservation()
        {
            Response response = new Response();

            try
            {

                commandBuilder = new CustomSqlCommandBuilder();

                // Enters values to dbo.reservations
                var query = commandBuilder.BeginInsert("dbo.reservations ([companyID], [floorPlanID], [spaceID], [reservationStartTime], [reservationEndTime], [status], [userHash]) " +
                    "VALUES (3, 1, 'CE5', '2024-08-24 12:00:00.000', '2024-08-24 15:00:00.000', 'Active', 'R9mXz33hkJew2FYYb28c8cb5jMnTjvEiqaJMizy/uoE=')").Build();

                response = await sqlDAO.ReadSqlResult(query);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test: {ex}");
            }

        }

        public async Task<int> FindReservationID()
        {
            int reservationID = 0;

            try
            {

                commandBuilder = new CustomSqlCommandBuilder();

                // Construct the SELECT query to find reservationID
                var query = commandBuilder.BeginSelectString("reservationID")
                                          .From("dbo.reservations")
                                          .Where("companyID = 3 AND floorPlanID = 1 AND spaceID = 'CE5' " +
                                                 "AND reservationStartTime = '2024-08-24 12:00:00.000' " +
                                                 "AND reservationEndTime = '2024-08-24 15:00:00.000' " +
                                                 "AND status = 'Active' " +
                                                 "AND userHash = 'R9mXz33hkJew2FYYb28c8cb5jMnTjvEiqaJMizy/uoE=';")
                                          .Build();

                // Execute the query and retrieve the reservationID
                var result = await sqlDAO.ReadSqlResult(query);

                // Check if the result is not null before unboxing

                reservationID = Convert.ToInt32(result.ValuesRead?.Rows[0]?["reservationID"]);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test: {ex}");
            }

            return reservationID;
        }
    }

};
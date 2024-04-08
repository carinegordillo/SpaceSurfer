using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.PersonalOverviewService
{
    public class PersonalOverviewDAO : IPersonalOverviewDAO
    {
        private ISqlDAO _sqlDAO;
        private ConfigService? configService;
        public PersonalOverviewDAO(ISqlDAO sqlDAO)
        {
            _sqlDAO = sqlDAO;
        }

        public async Task<Response> GetReservationList(string username, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            Response result = new Response();
            try
            {
                var baseDirectory = AppContext.BaseDirectory;
                var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
                var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
                configService = new ConfigService(configFilePath);

                // initializes a new instance of the logger
                Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

                var commandBuilder = new CustomSqlCommandBuilder();

                if (fromDate == null)
                {
                    fromDate = DateOnly.MinValue;
                }

                if (toDate == null)
                {
                    toDate = DateOnly.MaxValue;
                }

                var getCommand = commandBuilder.BeginSelect()
                                            .SelectColumns("r.reservationID, c.companyName", "r.companyID", "c.address", "r.floorPlanID", "r.spaceID",
                                                "CAST(r.reservationStartTime AS DATE) AS reservationDate",
                                                "CAST(r.reservationStartTime AS TIME) AS startTime",
                                                "CAST(r.reservationEndTime AS TIME) AS endTime",
                                                "r.status")
                                            .From("reservations r")
                                            .Join("userHash uh", "r.userHash", "uh.hashedUsername")
                                            .Join("companyProfile c", "r.companyID", "c.companyID")
                                            .Where($"userHash = '{username}' AND CAST(r.reservationStartTime AS DATE) >= '{fromDate}' AND CAST(r.reservationStartTime AS DATE) <= '{toDate}'")
                                            .OrderBy("reservationDate").Build();

                result = await _sqlDAO.ReadSqlResult(getCommand);

                if (result.HasError == false)
                {

                    // Successful Deletion
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful DAO Reservation Retrieval"
                    };

                    await logger.SaveData(entry);
                }
                else
                {
                    //Unsuccessful Deletion
                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful DAO Reservation Retrieval"
                    };

                    await logger.SaveData(errorEntry);
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }

            return result;

        }

        public async Task<Response> DeleteReservation(string username, int reservationID)
        {

            Response response = new Response();

            try
            {
                var baseDirectory = AppContext.BaseDirectory;
                var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
                var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
                configService = new ConfigService(configFilePath);

                // initializes a new instance of the logger
                Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

                var commandBuilder = new CustomSqlCommandBuilder();

                var getCommand = commandBuilder.BeginDelete("dbo.reservations")
                                            .Where($"reservationID = {reservationID} AND userHash = '{username}';")
                                            .Build();

                response = await _sqlDAO.SqlRowsAffected(getCommand);

                if (response.HasError == false)
                {

                    // Successful Deletion
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful DAO Reservation Deletion"
                    };

                    await logger.SaveData(entry);
                }
                else
                {
                    //Unsuccessful Deletion
                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful DAO Reservation Deletion"
                    };

                    await logger.SaveData(errorEntry);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}

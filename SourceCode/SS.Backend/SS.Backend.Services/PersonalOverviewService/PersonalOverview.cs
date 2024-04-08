using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Data;

namespace SS.Backend.Services.PersonalOverviewService
{
    public class PersonalOverview : IPersonalOverview
    {
        private ConfigService? configService;
        private readonly IPersonalOverviewDAO _personalOverviewDAO;

        public PersonalOverview(IPersonalOverviewDAO personalOverviewDAO)
        {
            _personalOverviewDAO = personalOverviewDAO;
        }

        public async Task<IEnumerable<ReservationInformation>> GetUserReservationsAsync(string username, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            List<ReservationInformation> reservationList = new List<ReservationInformation>();
            Response result = new Response();

            try
            {
                var baseDirectory = AppContext.BaseDirectory;
                var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
                var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

                // initializes a new instance of the logger
                Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

                result = await _personalOverviewDAO.GetReservationList(username, fromDate, toDate);

                if (!result.HasError && result.ValuesRead != null)
                {
                    foreach (DataRow row in result.ValuesRead.Rows)
                    {
                        var reservationDate = ((DateTime)row["reservationDate"]).Date;

                        var reservation = new ReservationInformation
                        {
                            ReservationID = Convert.ToInt32(row["reservationID"]),
                            CompanyName = row["companyName"].ToString(),
                            CompanyID = Convert.ToInt32(row["companyID"]),
                            Address = row["address"].ToString(),
                            FloorPlanID = Convert.ToInt32(row["floorPlanID"]),
                            SpaceID = row["spaceID"].ToString(),
                            ReservationDate = DateOnly.FromDateTime(reservationDate),
                            ReservationStartTime = ((TimeSpan)row["startTime"]),
                            ReservationEndTime = ((TimeSpan)row["endTime"]),
                            Status = row["status"].ToString()
                        };

                        reservationList.Add(reservation);
                    }

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Reservation Retrieval"
                    };

                    await logger.SaveData(entry);
                }
                else
                {
                    result.HasError = true;
                    result.ErrorMessage += $"No data found.";

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Reservation Retrieval or No Existing Reservations"
                    };
                }
            }
            catch (Exception ex)
            {
                // If an error occurs error
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }


            return reservationList;
        }

        public async Task<Response> DeleteUserReservationsAsync(string username, int reservationID)
        {

            Response response = new Response();

            try
            {
                var baseDirectory = AppContext.BaseDirectory;
                var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
                var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

                // initializes a new instance of the logger
                Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

                response = await _personalOverviewDAO.DeleteReservation(username, reservationID);

                if (response.HasError == false)
                {

                    // Successful Deletion
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Reservation Deletion"
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
                        description = "Unsuccessful Reservation Deletion"
                    };

                    await logger.SaveData(errorEntry);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage += $"Encountered an error deleting the reservation:{ex.Message}";
                return response;
            }

            return response;
        }
    }
}

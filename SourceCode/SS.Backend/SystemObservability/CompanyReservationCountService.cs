using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.Backend.SystemObservability
{
    public class CompanyReservationCountService : ICompanyReservationCountService
    {
        private ConfigService configService;
        private readonly ISystemObservabilityDAO _systemObservabilityDAO;

        public CompanyReservationCountService(ISystemObservabilityDAO systemObservabilityDAO)
        {
            _systemObservabilityDAO = systemObservabilityDAO;
        }

        public async Task<IEnumerable<CompanyReservationCount>> GetTop3CompaniesWithMostReservations(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            List<CompanyReservationCount> companyReservationCountsList = new List<CompanyReservationCount>();

            try
            {

                response = await _systemObservabilityDAO.RetrieveCompanyReservationsCount(username, timeSpan);

                if (!response.HasError && response.ValuesRead != null)
                {
                    foreach (DataRow row in response.ValuesRead.Rows)
                    {

                        var companyReservationCount = new CompanyReservationCount
                        {
                            CompanyName = (string)row["CompanyName"],
                            ReservationCount = (int)row["ReservationCount"]
                        };

                        companyReservationCountsList.Add(companyReservationCount);
                    }

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data",
                        description = "Successful Retrieval of Top 3 Companies with the Most Reservations"
                    };

                    await logger.SaveData(entry);
                }
                else
                {
                    response.HasError = true;

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data",
                        description = "Unsuccessful Retrieval of Top 3 Companies with the Most Reservations"
                    };
                    await logger.SaveData(errorEntry);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data",
                    description = "Service Error In Retrieval of Top 3 Companies with the Most Reservations"
                };
                await logger.SaveData(errorEntry);
            }

            return companyReservationCountsList;
        }
    }
}

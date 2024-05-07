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
    public class RegistrationCountService : IRegistrationCountService
    {
        private ConfigService configService;
        private readonly ISystemObservabilityDAO _systemObservabilityDAO;

        public RegistrationCountService(ISystemObservabilityDAO systemObservabilityDAO)
        { 
            _systemObservabilityDAO = systemObservabilityDAO;
        }

        public async Task<IEnumerable<RegistrationCount>> GetRegistrationCount(string username, string timeSpan)
        {

            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            List<RegistrationCount> registrationCountList = new List<RegistrationCount>();

            Response response = new Response();

            try
            {
                response = await _systemObservabilityDAO.RetrieveRegistrationsCount(username, timeSpan);

                if (!response.HasError && response.ValuesRead != null)
                {
                    foreach (DataRow row in response.ValuesRead.Rows)
                    {

                        var registrationCounts = new RegistrationCount
                        {
                            Month = (int)row["Month"],
                            Year = (int)row["Year"],
                            FailedRegistrations = (int)row["Failed Registrations"],
                            SuccessfulRegistrations = (int)row["Successful Registrations"]
                        };

                        registrationCountList.Add(registrationCounts);
                    }

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data",
                        description = "Successful Retrieval of Registration Counts In Service"
                    };

                    await logger.SaveData(entry);
                }
                else
                {
                    response.HasError = true;
                    response.ErrorMessage += $"No data found.";

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data",
                        description = "Unsuccessful Retrieval of Registration Counts In Service"
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
                    description = "Error in the Rerrieval of Registration Counts in Service"
                };
                await logger.SaveData(errorEntry);
            }

            return registrationCountList;

        }

    }
}

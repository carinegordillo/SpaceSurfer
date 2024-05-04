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
    public class LoginCountService : ILoginCountService
    {
        private ConfigService configService;
        private readonly ISystemObservabilityDAO _systemObservabilityDAO;

        public LoginCountService(ISystemObservabilityDAO systemObservabilityDAO)
        { 
            _systemObservabilityDAO = systemObservabilityDAO;
        }


        public async Task<IEnumerable<LogCount>> GetLoginCount(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            
            List<LogCount> loginCountsList = new List<LogCount>();

            Response response = new Response();

            try 
            {
                response = await _systemObservabilityDAO.RetrieveLoginsCount(username, timeSpan);

                if (!response.HasError && response.ValuesRead != null)
                {
                    foreach (DataRow row in response.ValuesRead.Rows)
                    {

                        var loginCounts = new LogCount
                        {
                            Month = (int)row["Month"],
                            Year = (int)row["Year"],
                            FailedLogins = (int)row["Failed Logins"],
                            SuccessfulLogins = (int)row["Successful Logins"]
                        };

                        loginCountsList.Add(loginCounts);
                    }

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data",
                        description = "Successful Retrieval of Logins Counts"
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
                        description = "Unsuccessful Retrieval of Logins Counts"
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
                    description = "Error in the Rerrieval of Logins Count"
                };
                await logger.SaveData(errorEntry);
            }

            return loginCountsList;

        }
    }
}

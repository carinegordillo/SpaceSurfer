using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Data;
using System.Data.Common;

namespace SS.Backend.SystemObservability
{
    public class ViewDurationService : IViewDurationService
    {
        private ConfigService configService;
        private readonly ISystemObservabilityDAO _systemObservabilityDAO;


        public ViewDurationService(ISystemObservabilityDAO systemObservabilityDAO)
        {
            _systemObservabilityDAO = systemObservabilityDAO;
        }

        public async Task<IEnumerable<ViewDuration>> GetTop3ViewDuration(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            List<ViewDuration> viewDurationList = new List<ViewDuration>();

            try 
            {

                response = await _systemObservabilityDAO.RetrieveTop3ViewDurations(username, timeSpan);

                if (!response.HasError && response.ValuesRead != null)
                {
                    foreach (DataRow row in response.ValuesRead.Rows)
                    {

                        var viewDuration = new ViewDuration
                        {
                            ViewName = (string)row["ViewName"],
                            DurationInSeconds = (int)row["DurationInSeconds"]
                        };

                        viewDurationList.Add(viewDuration);
                    }

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data",
                        description = "Successful Retrieval of Top 3 Views"
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
                        description = "Unsuccessful Retrieval of Top 3 Views"
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
                    description = "Error in the Rerrieval of Top 3 Views"
                };
                await logger.SaveData(errorEntry);
            }

            return viewDurationList;
        }

        public async Task<Response> InsertViewDuration(string username, string viewName, int durationInSeconds)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {
                response = await _systemObservabilityDAO.InsertViewDuration(username,viewName,durationInSeconds);

                if (!response.HasError)
                {
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Insertion of View Duration"
                    };

                    await logger.SaveData(entry);
                }
                else 
                {
                    response.ErrorMessage += $"Could Not Insert View Duration Information";

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Insertion of View Duration"
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
                    description = "Service Error In View Duration"
                };

                await logger.SaveData(errorEntry);
            }

            return response;
        }
    }
}

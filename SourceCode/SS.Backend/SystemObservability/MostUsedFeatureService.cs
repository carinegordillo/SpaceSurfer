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
    public class MostUsedFeatureService : IMostUsedFeatureService
    {
        private ConfigService configService;
        private readonly ISystemObservabilityDAO _systemObservabilityDAO;
        public MostUsedFeatureService(ISystemObservabilityDAO systemObservabilityDAO) 
        {
            _systemObservabilityDAO = systemObservabilityDAO;
        }

        public async Task<IEnumerable<MostUsedFeature>> GetMostUsedFeatures(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            List<MostUsedFeature> usedFeaturesList = new List<MostUsedFeature>();

            Response response = new Response();

            try
            {
                response = await _systemObservabilityDAO.RetrieveMostUsedFeatures(username, timeSpan);

                if (!response.HasError && response.ValuesRead != null)
                {
                    foreach (DataRow row in response.ValuesRead.Rows)
                    {

                        var usedFeatures = new MostUsedFeature
                        {
                            FeatureName = (string)row["FeatureName"],
                            FeatureCount = (int)row["FeatureCount"]
                        };

                        usedFeaturesList.Add(usedFeatures);
                    }

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data",
                        description = "Successful Retrieval of Most Used Features Service"
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
                        description = "Unsuccessful Retrieval of Most Used Features Service"
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
                    description = "Error in the Rerrieval of Most Used Features Service"
                };
                await logger.SaveData(errorEntry);
            }

            return usedFeaturesList;

        }

        public async Task<Response> InsertUsedFeature(string username, string featureName)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {
                response = await _systemObservabilityDAO.InsertUsedFeature(username, featureName);


                if (!response.HasError)
                {
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Service For Insertion of Feature Use"
                    };

                    await logger.SaveData(entry);
                }
                else
                {
                    response.ErrorMessage += $"Could Not Insert Feature Use Information";

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Service For Insertion of Feature Use"
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
                    description = "Service Error In Most Used Feature Service"
                };

                await logger.SaveData(errorEntry);
            }

            return response;

        }
    }
}

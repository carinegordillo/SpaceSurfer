using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Data;

namespace SS.Backend.SystemObservability
{
    public class CompanySpaceCountService : ICompanySpaceCountService
    {
        private ConfigService configService;
        private readonly ISystemObservabilityDAO _systemObservabilityDAO;

        public CompanySpaceCountService(ISystemObservabilityDAO systemObservabilityDAO)
        {
            _systemObservabilityDAO = systemObservabilityDAO;
        }

        public async Task<IEnumerable<CompanySpaceCount>> GetTop3CompaniesWithMostSpaces(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);

            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            List<CompanySpaceCount> companySpaceCountList = new List<CompanySpaceCount>();

            try
            {

                response = await _systemObservabilityDAO.RetrieveCompanySpaceCount(username, timeSpan);

                if (!response.HasError && response.ValuesRead != null)
                {
                    foreach (DataRow row in response.ValuesRead.Rows)
                    {

                        var companySpaceCount = new CompanySpaceCount
                        {
                            CompanyName = (string)row["CompanyName"],
                            SpaceCount = (int)row["SpaceCount"]
                        };

                        companySpaceCountList.Add(companySpaceCount);
                    }

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data",
                        description = "Successful Retrieval of Top 3 Companies/Facilities with the Most Spaces In The Service"
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
                        description = "Unsuccessful Retrieval of Top 3 Companies/Facilities with the Most Spaces In The Service"
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
                    description = "Service Error In Retrieval of Top 3 Companies/Facilities with the Most Spaces"
                };
                await logger.SaveData(errorEntry);
            }

            return companySpaceCountList;
        }

    }
}

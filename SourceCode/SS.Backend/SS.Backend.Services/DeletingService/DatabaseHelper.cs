using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService
{
    /// <summary>
    ///     DatabaseHelper class is responsible of retrieving table names from a database
    /// </summary>
    ///
    public class DatabaseHelper : IDatabaseHelper
    {

        private ISqlDAO _sqlDAO;
        private ConfigService? configService;

        public DatabaseHelper(ISqlDAO sqlDAO)
        {
            _sqlDAO = sqlDAO;
        }

        public async Task<Response> DeleteAccount(string username)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {
                var commandBuild = new CustomSqlCommandBuilder();

                var query = commandBuild.BeginDeleteFrom("dbo.userAccount").Where($"user_id IN (SELECT user_id FROM dbo.userHash WHERE hashedUsername = '{username}');")
                        .Build();

                response = await _sqlDAO.SqlRowsAffected(query);


                if (!response.HasError)
                {

                    // Successful Deletion
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Database Helper Account Deletion"
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
                        description = "Unsuccessful Database Helper Account Deletion"
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
                    category = "Data Store",
                    description = "Database Helper Service Encounterd An Error"
                };

                await logger.SaveData(errorEntry);
            }

            //var tableNames = commandBuild.BeginSelect().SelectOne("Username").From("dbo.userAccount ").Where("Username = '@user';").Build();



            return response;

        }
    }
}

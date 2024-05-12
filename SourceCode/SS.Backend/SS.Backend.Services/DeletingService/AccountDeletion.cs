using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService
{
    /// <summary>
    ///     AccountDeletion class responsible for deleting user records from the database
    /// </summary>
    ///
    public class AccountDeletion : IAccountDeletion
    {
        private ConfigService? configService;
        private IDatabaseHelper _databaseHelper;

        public AccountDeletion(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }


        /// <summary>
        ///     DeleteAccount deletes the account by username
        /// </summary>
        ///
        /// <param username > String value representing a username.</param>
        public async Task<Response> DeleteAccount(string username)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);

            // initializes a new instance of the logger
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            // initializes a new instance of the Response
            Response result = new Response();

            try
            {
                Console.WriteLine("Inside AccountDeletion.cs - begin DeleteAccount");
                // Sets the tables names from the Database Helper Response
                result = await _databaseHelper.DeleteAccount(username);

                if (!result.HasError)
                {
                    Console.WriteLine("Inside AccountDeletion.cs - success DeleteAccount");
                    // Successful Deletion
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data",
                        description = "Successful Account Deletion"
                    };

                    await logger.SaveData(entry);
                }
                else
                {
                    Console.WriteLine("Inside AccountDeletion.cs - unsuccessful DeleteAccount");
                    //Unsuccessful Deletion
                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data",
                        description = "Unsuccessful Account Deletion"
                    };

                    await logger.SaveData(errorEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Inside AccountDeletion.cs - inside catch");
                // If an error occurs error
                result.HasError = true;
                result.ErrorMessage = ex.Message;

                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data",
                    description = "Account Deletion Service Encountered An Error"
                };

                await logger.SaveData(errorEntry);
            }

            // Logs entry based on the overallResponse error


            return result;
        }

    }

}

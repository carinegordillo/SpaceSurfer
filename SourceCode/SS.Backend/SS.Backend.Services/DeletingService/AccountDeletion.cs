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
                // Sets the tables names from the Database Helper Response
                result = await _databaseHelper.RetrieveTable(username);

                //if (result.ValuesRead != null)
                //{

                //    // Delete Query Command built [DELETE FROM "Users" WHERE Username = @username]
                //    var deleteCommand = commandBuilder.BeginDelete("dbo.userAccount").Where($"Username = '{username}'").Build();

                //    // Sets the reponse from the executed command
                //    result = await SQLDao.SqlRowsAffected(deleteCommand);

                //    //// Appends the responses into one response
                //    //overallResponse.RowsAffected += response.RowsAffected;
                //    //overallResponse.HasError |= response.HasError;
                //}
                //else
                //{
                //    // Handle the case where tables.Task is not completed or its result is null
                //    throw new InvalidOperationException("Failed to retrieve table.");
                //}

                // await tables;
                //// Check if the task completed successfully and its result is not null
                //// if (tables != null && tables.Result != null && tables.Result.ValuesRead != null)
                //// {
                //    // Iterates through every table that deals with the value being deleted
                //    for (int i = tables.Result.ValuesRead.Count - 1; i >= 0; i--)
                //    {
                //        

                //    }
                //}

                if (!result.HasError)
                {

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

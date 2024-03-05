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

        ConfigService configService = new ConfigService("C:/Users/brand/Documents/GitHub/SpaceSurfer/SourceCode/SS.Backend/config.local.txt");

        /// <summary>
        ///     DeleteAccount deletes the account by username
        /// </summary>
        ///
        /// <param username > String value representing a username.</param>
        public async Task<Response> DeleteAccount(string username)
        {
            // initializes a new instance of the Response
            Response result = new Response();

            try
            {

                // initializes a new instance of SqlDAO
                SqlDAO SQLDao = new SqlDAO(configService);

                // initializes a new instance of the logger
                Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

                // initializes a new instance of the Custom Command Builder
                var commandBuilder = new CustomSqlCommandBuilder();

                var command = commandBuilder.BeginSelectAll()
                        .From("dbo.userAccount ")
                        .Where($"Username = '{username}'")
                        .Build();
                // initializes a new instance of Database Helper
                //DatabaseHelper dbHelper = new DatabaseHelper();

                // Sets the tables names from the Database Helper Response
                result = await SQLDao.ReadSqlResult(command);

                // Sets the value to the username
                //var value = new Dictionary<string, object>
                //{
                //    { "Username", username}
                //};


                if (result.ValuesRead != null || result.ValuesRead.Rows.Count != null)
                {

                    // Delete Query Command built [DELETE FROM "Users" WHERE Username = @username]
                    var deleteCommand = commandBuilder.BeginDelete("dbo.userAccount").Where($"Username = '{username}'").Build();

                    // Sets the reponse from the executed command
                    result = await SQLDao.SqlRowsAffected(deleteCommand);

                    //// Appends the responses into one response
                    //overallResponse.RowsAffected += response.RowsAffected;
                    //overallResponse.HasError |= response.HasError;
                }
                else
                {
                    // Handle the case where tables.Task is not completed or its result is null
                    throw new InvalidOperationException("Failed to retrieve table.");
                }

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

                if (result.HasError == false)
                {

                    // Successful Deletion
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Deletion"
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
                        description = "Unsuccessful Deletion"
                    };

                    await logger.SaveData(errorEntry);
                }
            }
            catch (Exception ex)
            {
                // If an error occurs error
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }

            // Logs entry based on the overallResponse error


            return result;
        }

    }

}

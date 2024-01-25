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
        // Temporary User Variable
        Credential temp = Credential.CreateSAUser();

        /// <summary>
        ///     DeleteAccount deletes the account by username
        /// </summary>
        ///
        /// <param username > String value representing a username.</param>
        public async Task<Response> DeleteAccount(string username)
        {
            // initializes a new instance of the Response
            Response overallResponse = new Response();

            // initializes a new instance of SqlDAO
            SealedSqlDAO SQLDao = new SealedSqlDAO(temp);

            // initializes a new instance of the logger
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(temp)));

            // initializes a new instance of the Custom Command Builder
            CustomSqlCommandBuilder commandBuilder = new CustomSqlCommandBuilder();

            // initializes a new instance of Database Helper
            DatabaseHelper dbHelper = new DatabaseHelper();

            // Sets the response error to false
            overallResponse.HasError = false;

            // Sets the tables names from the Database Helper Response
            var tables = dbHelper.RetrieveTableNames();

            // Sets the value to the username
            var value = new Dictionary<string, object>
            {
                { "Username", username}
            };

            try
            {
                // Iterates through every table that deals with the value being deleted
                for (int i = tables.Result.ValuesRead.Count - 1; i >= 0; i--)
                {
                    // Initializing Table Name 
                    var table = tables.Result.ValuesRead[i][2];

                    // Delete Query Command built [DELETE FROM "Users" WHERE Username = @username]
                    var deleteCommand = commandBuilder.BeginDelete("dbo." + table).Where("Username = @Username").AddParameters(value).Build();

                    // Sets the reponse from the executed command
                    var response = await SQLDao.SqlRowsAffected(deleteCommand);

                    // Appends the responses into one response
                    overallResponse.RowsAffected += response.RowsAffected;
                    overallResponse.HasError |= response.HasError;

                }

            }
            catch (Exception ex)
            {
                // If an error occurs error
                overallResponse.HasError = true;
                overallResponse.ErrorMessage = ex.Message;
            }

            // Logs entry based on the overallResponse error
            if (overallResponse.HasError == false)
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

            return overallResponse;
        }

    }

}

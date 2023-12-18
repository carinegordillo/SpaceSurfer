using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService
{
    public class Deleter : IDeleter
    {
        // Temporary User Variable
        Credential temp = Credential.CreateSAUser();


        public async Task<Response> DeleteAccount(string username)
        {
            // Creates a new instance of SqlDAO
            SealedSqlDAO SQLDao = new SealedSqlDAO(temp);

            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(temp)));

            // Creates a new instance of the Response
            Response overallResponse = new Response();

            overallResponse.HasError = false;

            // Creates a new instance of the Custom Command Builder
            var commandBuilder = new CustomSqlCommandBuilder();

            var dbHelper = new DatabaseHelper();

            var tables = dbHelper.RetrieveTableNames();

            // Sets the value to the username
            var value = new Dictionary<string, object>
            {
                { "Username", username}
            };

            try
            {

                for (int i = tables.Result.ValuesRead.Count - 1; i >= 0; i--)
                {
                    var table = tables.Result.ValuesRead[i][2];

                    // Delete Query Command built [DELETE FROM "Users" WHERE Username = @username]
                    var deleteCommand = commandBuilder.BeginDelete("dbo." + table).Where("Username = @Username").AddParameters(value).Build();

                    var response = await SQLDao.SqlRowsAffected(deleteCommand);

                    overallResponse.RowsAffected += response.RowsAffected;
                    overallResponse.HasError |= response.HasError;

                }

            }
            catch (Exception ex)
            {
                overallResponse.HasError = true;
                overallResponse.ErrorMessage = ex.Message;
            }

            if (overallResponse.HasError == false)
            {
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
                LogEntry entry = new()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data Store",
                    description = "Unsuccessful Deletion"
                };

                await logger.SaveData(entry);
            }

            return overallResponse;
        }

    }

}

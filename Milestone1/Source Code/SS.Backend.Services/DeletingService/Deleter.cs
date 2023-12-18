using SS.Backend.DataAccess;
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

            // Creates a new instance of the Response
            Response response = new Response();

            // Creates a new instance of the Custom Command Builder
            var commandBuilder = new CustomSqlCommandBuilder();


            // Sets the value to the username
            var value = new Dictionary<string, object>
            {
                { "Username", username}
            };

            // Delete Query Command built [DELETE FROM "Users" WHERE Username = @username]
            var deleteCommand = commandBuilder.BeginDelete("dbo." + TableName).Where("Username = @Username").AddParameters(value).Build();

            return await SQLDao.SqlRowsAffected(deleteCommand);
        }
    }
}

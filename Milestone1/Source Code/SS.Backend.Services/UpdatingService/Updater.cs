using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;


namespace SS.Backend.Services.UpdatingService
{
    public class Updater : IUpdater
    {
        Credential removeMeLater = Credential.CreateSAUser();

         public async Task<Response> UpdateDisplayName(string username, string newEmail, int newAge){

            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            var columnValues = new Dictionary<string, object>{
            { "Email", newEmail },
            { "Age", newAge }};

            var updateCommand = commandBuilder.BeginUpdate("Users")
                                            .Set(columnValues)
                                            .Where("Username = @Username")
                                            .AddParameters(columnValues)
                                            .Build();


            return await SQLDao.SqlRowsAffected(updateCommand);
        }
    }
}
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;


namespace SS.Backend.Services.ModificationService
{
    public class AccountModifier : IAccountModifier
    {
        Credential removeMeLater = Credential.CreateSAUser();

         public async Task<Response> UpdateDisplayName(string username, string newEmail, int newAge){

            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            var columnValues = new Dictionary<string, object>{
            { "firstName", newEmail },
            { "Age", newAge }};

            var whereValues = new Dictionary<string, object>{
            { "@Username", newEmail }};



            var updateCommand = commandBuilder.BeginUpdate("TABLENAME")
                                            .Set(columnValues)
                                            .AddParameters(columnValues)
                                            .Where("Username = @Username")
                                            .Build();


            return await SQLDao.SqlRowsAffected(updateCommand);
        }
    }

}
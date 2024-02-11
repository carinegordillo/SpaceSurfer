using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;


namespace SS.Backend.UserManagement
{
    public class ProfileModifier : IProfileModifier
    {
        Credential removeMeLater = Credential.CreateSAUser();

        public async Task<Response> ModifyFirstName(string hashedUsername, string newFirstName){

            Response response = await GenProfileModifier("hashedUsername",hashedUsername,"firstName",newFirstName,"dbo.userProfile");

            return response;
        }

        public async Task<Response> ModifyLastName(string hashedUsername, string newLastName){

            Response response = await GenProfileModifier("hashedUsername",hashedUsername,"lastName",newLastName,"dbo.userProfile");

            return response;
        }

        public async Task<Response> ModifyBackupEmail(string hashedUsername, string newBackupEmail){

            Response response = await GenProfileModifier("hashedUsername",hashedUsername,"backupEmail","newEamils@yahoo","dbo.userProfile");

            return response;

        }

        public async Task<Response> ReadRequests(string tableName){

            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            
            //var columnValues = new Dictionary<string, object>{{whereClause, requestType}};

            SqlCommand selectRequestsCommand = commandBuilder.BeginSelectAll()
                                            .From($"{tableName}")
                                            .Build();

            return await SQLDao.ReadSqlResult(selectRequestsCommand);


        }

        public async Task<Response> GenProfileModifier(string whereClause, object whereClauseval, string fieldName, object newValue, string profileTableName)
        {
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            
            var columnValues = new Dictionary<string, object>{
                { fieldName, newValue },{whereClause,whereClauseval}};

            SqlCommand updateCommand = commandBuilder.BeginUpdate(profileTableName)
                                            .Set(columnValues)
                                            .Where($"{whereClause} = @{whereClause}")
                                            .AddParameters(columnValues)
                                            .Build();


            return await SQLDao.SqlRowsAffected(updateCommand);
        }

    }


}
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;

namespace SS.Backend.UserManagement
{

    public class UserManagementRepository : IUserManagementRepository
    {

        Credential removeMeLater = Credential.CreateSAUser();


        public async Task<Response> GeneralModifier(string whereClause, object whereClauseval, string fieldName, object newValue, string tableName)
        {
            string configFilePath = "/Users/carinegordillo/config.txt";
            ConfigService configService = new ConfigService(configFilePath);
            testSealedSqlDAO SQLDao = new testSealedSqlDAO(configService);



            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            
            var columnValues = new Dictionary<string, object>{
                { fieldName, newValue },{whereClause,whereClauseval}};

            SqlCommand updateCommand = commandBuilder.BeginUpdate(tableName)
                                            .Set(columnValues)
                                            .Where($"{whereClause} = @{whereClause}")
                                            .AddParameters(columnValues)
                                            .Build();

            response = await SQLDao.SqlRowsAffected(updateCommand);

            if (response.HasError == false){
                response.ErrorMessage += "- General Modifier - command successful -";
            }
            else{
                 response.ErrorMessage += $"- General Modifier - command : {updateCommand.CommandText} not successful -";

            }
            return response;
        }

        public async Task<Response> ReadUserTable(string tableName)
        {

            string configFilePath = "/Users/carinegordillo/config.txt";
            ConfigService configService = new ConfigService(configFilePath);
            testSealedSqlDAO SQLDao = new testSealedSqlDAO(configService);

            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            


            SqlCommand selectRequestsCommand = commandBuilder.BeginSelectAll()
                                            .From($"{tableName}")
                                            .Build();

            response = await SQLDao.ReadSqlResult(selectRequestsCommand);

            if (response.HasError == false){
                response.ErrorMessage += "- ReadUserTable- command successful -";
            }
            else{
                 response.ErrorMessage += "- ReadUserTable - command not successful -";

            }
            return response;
        }



        public async Task<Response> createAccountRecoveryRequest(UserRequestModel userRequest, string tableName)
        {

            string configFilePath = "/Users/carinegordillo/config.txt";
            ConfigService configService = new ConfigService(configFilePath);
            testSealedSqlDAO SQLDao = new testSealedSqlDAO(configService);
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
                        {
                            { "userHash", userRequest.UserHash },
                            { "requestDate", userRequest.RequestDate},
                            { "status", userRequest.Status },
                            { "requestType", userRequest.RequestType},
                            { "additionalInformation", userRequest.AdditionalInformation }
                        };

            SqlCommand InsertRequestsCommand = commandBuilder.BeginInsert(tableName)
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();

            response = await SQLDao.SqlRowsAffected(InsertRequestsCommand);

            if (response.HasError == false){
                response.ErrorMessage += "- createAccountRecoveryRequest - command successful - ";
            }
            else{
                 response.ErrorMessage += $"- createAccountRecoveryRequest - command : {InsertRequestsCommand} not successful - ";

            }
            return response;
        }



        public async Task<Response> sendRequest(string name, string position)
        {
            

            string configFilePath = "/Users/carinegordillo/config.txt";
            ConfigService configService = new ConfigService(configFilePath);
            testSealedSqlDAO SQLDao = new testSealedSqlDAO(configService);
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
                        {
                            { "Name", name },
                            { "Position", position}
                        };

            SqlCommand InsertRequestsCommand = commandBuilder.BeginInsert("dbo.EmployeesDummyTable")
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();

            response = await SQLDao.SqlRowsAffected(InsertRequestsCommand);

            if (response.HasError == false){
                response.ErrorMessage += "- createAccountRecoveryRequest - command successful - ";
            }
            else{
                 response.ErrorMessage += $"- createAccountRecoveryRequest - command : {InsertRequestsCommand} not successful - ";

            }
            return response;
        }


    }
}
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;


namespace SS.Backend.UserManagement
{

    public class UserManagementRepository : IUserManagementRepository
    {

        public async Task<Response> GeneralModifier(string whereClause, object whereClauseval, string fieldName, object newValue, string tableName)
        {
            string configFilePath = "/Users/carinegordillo/config.txt";
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO sqldao = new SqlDAO(configService);



            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            
            var columnValues = new Dictionary<string, object>{
                { fieldName, newValue },{whereClause,whereClauseval}};

            var updateCommand = commandBuilder.BeginUpdate(tableName)
                                            .Set(columnValues)
                                            .Where($"{whereClause} = @{whereClause}")
                                            .AddParameters(columnValues)
                                            .Build();

            response = await sqldao.SqlRowsAffected(updateCommand);

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
            SqlDAO sqldao = new SqlDAO(configService);

            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            


            var selectRequestsCommand = commandBuilder.BeginSelectAll()
                                            .From($"{tableName}")
                                            .Build();

            response = await sqldao.ReadSqlResult(selectRequestsCommand);

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
            SqlDAO sqldao = new SqlDAO(configService);
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

            var InsertRequestsCommand = commandBuilder.BeginInsert(tableName)
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();

            response = await sqldao.SqlRowsAffected(InsertRequestsCommand);

            if (response.HasError == false){
                response.ErrorMessage += "- createAccountRecoveryRequest - command successful - ";
            }
            else{
                 response.ErrorMessage += $"- createAccountRecoveryRequest - command : {InsertRequestsCommand.CommandText} not successful - ";

            }
            return response;
        }



        public async Task<Response> sendRequest(string employeeName, string position)
        {
            

            string configFilePath = "/Users/carinegordillo/config.txt";
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO sqldao = new SqlDAO(configService);
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
                        {
                            { "Name", employeeName },
                            { "Position", position}
                        };

            var InsertRequestsCommand = commandBuilder.BeginInsert("dbo.EmployeesDummyTable")
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();

            response = await sqldao.SqlRowsAffected(InsertRequestsCommand);

            if (response.HasError == false){
                response.ErrorMessage += "- sendRequest - command successful - ";
            }
            else{
                 response.ErrorMessage += $"- sendRequest - command : {InsertRequestsCommand.CommandText} not successful - ";

            }
            return response;
        }

        public async Task<Response> readTableWhere(string whereClause, object whereClauseval, string tableName)
        {
            string configFilePath = "/Users/carinegordillo/config.txt";
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO sqldao = new SqlDAO(configService);
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {{ whereClause, whereClauseval }};

            var command = commandBuilder.BeginSelectAll()
                        .From(tableName)
                        .Where($"{whereClause} = @{whereClause}")
                        .AddParameters(parameters)
                        .Build();
            
            response = await sqldao.ReadSqlResult(command);

            if (response.HasError == false){
                response.ErrorMessage += "- readTableWhere- command successful -";
            }
            else{
                 response.ErrorMessage += $"- readTableWhere- {command.CommandText} -  command not successful -";

            }
            return response;



        }



    }
}
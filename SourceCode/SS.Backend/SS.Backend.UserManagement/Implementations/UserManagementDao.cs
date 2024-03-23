using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;


namespace SS.Backend.UserManagement
{

    public class UserManagementDao : IUserManagementDao
    {
        private ISqlDAO _sqldao;


        public UserManagementDao(ISqlDAO sqldao)
        {
            _sqldao = sqldao;
        }


        public async Task<Response> GeneralModifier(string whereClause, object whereClauseval, string fieldName, object newValue, string tableName)
        {

            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            UpdateOnlyBuiltSqlCommands updateOnlysqlCommands = new UpdateOnlyBuiltSqlCommands(commandBuilder);

            SqlCommand updateCommand = updateOnlysqlCommands.GenericUpdateOne(whereClause, whereClauseval, fieldName, newValue, tableName);

            response = await _sqldao.SqlRowsAffected(updateCommand);

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

            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            ReadOnlyBuiltSqlCommands readOnlysqlCommands = new ReadOnlyBuiltSqlCommands(commandBuilder);

            SqlCommand selectRequestsCommand = readOnlysqlCommands.GenericReadAllFrom(tableName);

            response = await _sqldao.ReadSqlResult(selectRequestsCommand);

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

            CreateOnlyBuiltSqlCommands createOnlysqlCommands = new CreateOnlyBuiltSqlCommands(commandBuilder);

            var InsertRequestsCommand = createOnlysqlCommands.GenericInsert(parameters, "dbo.userRequests");

            response = await _sqldao.SqlRowsAffected(InsertRequestsCommand);

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
            
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
                        {
                            { "Name", employeeName },
                            { "Position", position}
                        };

            CreateOnlyBuiltSqlCommands createOnlysqlCommands = new CreateOnlyBuiltSqlCommands(commandBuilder);

            var InsertRequestsCommand = createOnlysqlCommands.GenericInsert(parameters, "dbo.userRequests");
            
            response = await _sqldao.SqlRowsAffected(InsertRequestsCommand);

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

            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            ReadOnlyBuiltSqlCommands readOnlysqlCommands = new ReadOnlyBuiltSqlCommands(commandBuilder);

            SqlCommand command = readOnlysqlCommands.GenericReadWhereSingular(whereClause, whereClauseval, tableName);
            
            response = await _sqldao.ReadSqlResult(command);

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
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
        public async Task<Response> CreateAccount(UserInfo userInfo, CompanyInfo? companyInfo)
        {   
            UserPepper userPepper = new UserPepper();
            Hashing hashing = new Hashing();
            string pepper = "DA06";


#pragma warning disable CS8604 // Possible null reference argument.
            var validPepper = new UserPepper
            {
                hashedUsername = hashing.HashData(userInfo.username, pepper)
            };
#pragma warning restore CS8604 // Possible null reference argument.
            Dictionary<string, object> userAccount_success_parameters;


#pragma warning disable CS8604 // Possible null reference argument.
            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", userInfo.firstname},
                { "LastName", userInfo.lastname}, 
                {"backupEmail", userInfo.backupEmail},
                {"appRole", userInfo.role}, 
            };
#pragma warning restore CS8604 // Possible null reference argument.

#pragma warning disable CS8604 // Possible null reference argument.
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", userInfo.status} 
            };
#pragma warning restore CS8604 // Possible null reference argument.

            var hashedAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"username", userInfo.username},
                {"user_id", 1}
            };


            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                // { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };
            Console.WriteLine("TABLE DATE ",  tableData);

            if (userInfo.role != 4) {
                userAccount_success_parameters = new Dictionary<string, object>
                {
                    { "username", userInfo.username},
                    {"birthDate", userInfo.dob},
                };
                tableData.Add("userAccount", userAccount_success_parameters);
            } 

            if (companyInfo != null)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var companyProfile_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"companyName", companyInfo.companyName},
                    {"address", companyInfo.address},
                    {"openingHours", companyInfo.openingHours},
                    {"closingHours", companyInfo.closingHours},
                    {"daysOpen", companyInfo.daysOpen}
                };
#pragma warning restore CS8604 // Possible null reference argument.

                // Add the companyProfile dictionary to tableData
                tableData.Add("companyProfile", companyProfile_success_parameters);
            }

            var builder = new CustomSqlCommandBuilder();
            Response tablesresponse = new Response();

            // Iterate through each table entry in the provided data
            foreach (var tableEntry in tableData)
            {
                string tableName = tableEntry.Key; // Table name
                Dictionary<string, object> parameters = tableEntry.Value; // Parameters for the table

                // Build the INSERT SQL command for each table
                var insertCommand = builder.BeginInsert(tableName)
                    .Columns(parameters.Keys) // Specify columns
                    .Values(parameters.Keys) // Specify values (same as columns for parameterized queries)
                    .AddParameters(parameters) // Add parameters
                    .Build();

                // Execute the INSERT command
                tablesresponse = await _sqldao.SqlRowsAffected(insertCommand);

                // Check for errors and return if any
                if (tablesresponse.HasError)
                {
                    tablesresponse.ErrorMessage += $"{tableName}: error inserting data; ";
                    return tablesresponse;
                }
            }
            return tablesresponse;
        }

    }
}
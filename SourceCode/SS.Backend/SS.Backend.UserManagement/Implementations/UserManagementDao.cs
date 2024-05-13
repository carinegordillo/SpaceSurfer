using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.Services.LoggingService;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

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
        public async Task<Response> CreateAccount(UserInfo userInfo, CompanyInfo? companyInfo,  string? manager_hashedUsername)
        {   
            UserPepper userPepper = new UserPepper();
            Hashing hashing = new Hashing();
            string pepper = "DA06";
            var builder = new CustomSqlCommandBuilder();
            Response tablesresponse = new Response();


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
            if (userInfo.role == 1 || userInfo.role == 5) {
                userAccount_success_parameters = new Dictionary<string, object>
                {
                    { "username", userInfo.username},
                    {"birthDate", userInfo.dob},
                };
                tableData.Add("userAccount", userAccount_success_parameters);
            } else if (userInfo.role == 4){
                // Fetch the companyID for role Employee
                var companyIDResponse = await getEmployeeCompanyID(userInfo, manager_hashedUsername);
                if (companyIDResponse.HasError || companyIDResponse.ValuesRead.Rows.Count == 0) {
                    return new Response { HasError = true, ErrorMessage = companyIDResponse.ErrorMessage ?? "Failed to fetch company ID" };
                }
                if(companyIDResponse.ValuesRead != null)
                {
                    foreach (DataRow row in companyIDResponse.ValuesRead.Rows)
                    {
                        int companyID = Convert.ToInt32(row["companyID"]);
                        if (companyID > 0)
                        {
                            userAccount_success_parameters = new Dictionary<string, object>
                            {
                                { "username", userInfo.username},
                                {"birthDate", userInfo.dob},
                                {"companyID", companyID}
                            };
                            tableData.Add("userAccount", userAccount_success_parameters);
                            
                        }
                    }
                }
            } else if (userInfo.role == 2 || userInfo.role == 3){
                var companyProfile_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"companyName", companyInfo.companyName},
                    {"address", companyInfo.address},
                    {"openingHours", companyInfo.openingHours},
                    {"closingHours", companyInfo.closingHours},
                    {"daysOpen", companyInfo.daysOpen}, 
                    {"companyType", userInfo.role}
                };
                var insertCompanyProfileCommand = builder.BeginInsert("companyProfile")
                    .Columns(companyProfile_success_parameters.Keys)
                    .Values(companyProfile_success_parameters.Keys)
                    .AddParameters(companyProfile_success_parameters)
                    .Build();

                tablesresponse = await _sqldao.SqlRowsAffected(insertCompanyProfileCommand);
                if (tablesresponse.HasError)
                {
                    tablesresponse.ErrorMessage += "companyProfile: error inserting data; ";
                    return tablesresponse;
                }
                var companyIDResponse = await getEmployeeCompanyID(userInfo, validPepper.hashedUsername);
                if (companyIDResponse.HasError || companyIDResponse.ValuesRead.Rows.Count == 0) {
                    return new Response { HasError = true, ErrorMessage = companyIDResponse.ErrorMessage ?? "Failed to fetch company ID for manager" };
                }
            
                if(companyIDResponse.ValuesRead != null)
                {
                    foreach (DataRow row in companyIDResponse.ValuesRead.Rows)
                    {
                        int companyID = Convert.ToInt32(row["companyID"]);
                        if (companyID > 0)
                        {
                            userAccount_success_parameters = new Dictionary<string, object>
                            {
                                { "username", userInfo.username},
                                {"birthDate", userInfo.dob},
                                {"companyID", companyID}
                            };
                            tableData.Add("userAccount", userAccount_success_parameters);
                            
                        }
                    }
                }
            } 
            foreach (var tableEntry in tableData)
            {
                string tableName = tableEntry.Key;
                Dictionary<string, object> parameters = tableEntry.Value; 

                var insertCommand = builder.BeginInsert(tableName)
                    .Columns(parameters.Keys) 
                    .Values(parameters.Keys) 
                    .AddParameters(parameters) 
                    .Build();

                tablesresponse = await _sqldao.SqlRowsAffected(insertCommand);

                if (tablesresponse.HasError)
                {
                    tablesresponse.ErrorMessage += $"{tableName}: error inserting data; ";
                    return tablesresponse;
                }
            }
            return tablesresponse;
        }
        public async Task<Response> getEmployeeCompanyID(UserInfo userInfo, string manager_hashedUsername) {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO SQLDao = new SqlDAO(configService);
            Response response = new Response();

            try {
                var builder = new CustomSqlCommandBuilder();
                var parameters = new Dictionary<string, object> {
                    {"hashedUsername", manager_hashedUsername}
                };

                var selectCommand = builder.BeginSelect()
                                        .SelectColumns("companyID") 
                                        .From("companyProfile")
                                        .Where("hashedUsername = @hashedUsername")
                                        .AddParameters(parameters) 
                                        .Build();

                var queryResponse = await SQLDao.ReadSqlResult(selectCommand);
                if (queryResponse.HasError) {
                    response.HasError = true;
                    response.ErrorMessage = "Failed to retrieve company ID: " + queryResponse.ErrorMessage;
                } else if (queryResponse.ValuesRead != null && queryResponse.ValuesRead.Rows.Count > 0) {
                    response.ValuesRead = queryResponse.ValuesRead;
                    response.HasError = false;
                } else {
                    response.HasError = true;
                    response.ErrorMessage = "No company associated with the provided manager username.";
                }
            } catch (Exception ex) {
                response.HasError = true;
                response.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            }
            return response;
        }

        public async Task<Response> DeleteRequestWhere(string whereClause, object whereClauseval, string tableName){
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            DeleteOnlyBuiltSqlCommands deleteOnlysqlCommands = new DeleteOnlyBuiltSqlCommands(commandBuilder);

            SqlCommand command = deleteOnlysqlCommands.DeleteRowWhere(whereClause, whereClauseval, "userRequests");
            
            response = await _sqldao.SqlRowsAffected(command);

            if (response.HasError == false){
                response.ErrorMessage += "- DeleteRequestWhere- command successful -";
            }
            else{
                 response.ErrorMessage += $"- DeleteRequestWhere- {command.CommandText} -  command not successful -";
            }
            return response;
        }

        public async Task<string> GetEmailByHash(string hashedUsername)
        {
            string? userUsername = null;
            Response tablesresponse = new Response();
            var builder = new CustomSqlCommandBuilder();
            var command = builder.BeginSelect()
                                .SelectColumns("username")
                                .From("dbo.userHash")
                                .Where("hashedUsername = @HashedUsername")
                                .AddParameters(new Dictionary<string, object>
                                {
                                    { "HashedUsername", hashedUsername}
                                })
                                .Build();

            var username = await _sqldao.ReadSqlResult(command);

            if (!username.HasError)
            {
                foreach (DataRow row in username.ValuesRead.Rows)
                {
                    userUsername = row["username"] != DBNull.Value ? Convert.ToString(row["username"]) : null;
                }
            }
            else
            {
                throw new Exception("Couldn't read userHash: " + username.ErrorMessage);
            }

            return userUsername ?? string.Empty;
        }

        public async Task<string> GetHashByEmail(string email)
        {
            string? userUsername = null;
            Response tablesresponse = new Response();
            var builder = new CustomSqlCommandBuilder();
            var command = builder.BeginSelect()
                                .SelectColumns("hashedUsername")
                                .From("dbo.userHash")
                                .Where("username = @username")
                                .AddParameters(new Dictionary<string, object>
                                {
                                    { "username", email}
                                })
                                .Build();

            var username = await _sqldao.ReadSqlResult(command);

            if (!username.HasError)
            {
                foreach (DataRow row in username.ValuesRead.Rows)
                {
                    userUsername = row["hashedUsername"]?.ToString();
                }
            }
            else
            {
                throw new Exception("Couldn't read userHash: " + username.ErrorMessage);
            }

            return userUsername ?? "could not find User";
        }

    }
}
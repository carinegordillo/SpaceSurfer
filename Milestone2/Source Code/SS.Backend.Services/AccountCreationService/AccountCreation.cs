using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using SS.Backend.Services.LoggingService;


namespace SS.Backend.Services.AccountCreationService
{
    public class AccountCreation : IAccountCreation
    {
        Credential temp = Credential.CreateSAUser();
        private readonly UserInfo _userInfo;
        private readonly ICustomSqlCommandBuilder _commandBuilder;

        public AccountCreation(UserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        //checking for null and white space 
        public bool CheckNullWhiteSpace(string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
        public string CheckUserInfoValidity(UserInfo userInfo)
        {
            string errorMsg = "";
            int allValid = 0;
            int totalStringFields = 0; 
            foreach (PropertyInfo prop in userInfo.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    totalStringFields++; // Count string fields
                    string value = prop.GetValue(userInfo) as string;

                    if (!string.IsNullOrEmpty(value) && CheckNullWhiteSpace(value) && !value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                    {
                        allValid++;
                    }
                    else
                    {
                        errorMsg += $"Invalid {prop.Name.ToLower()}; ";
                    }
                }
            }
            if (allValid == totalStringFields)
            {
                errorMsg = "Pass";
            }else{
                errorMsg = "fail";
            }
            return errorMsg;
        }

        //pepper 
        //  UserInfo hashedUsername = new UserInfo(hashedUser) 

        
        //this method takes builds a dictionary with several sql commands to insert all at once 
        public async Task<Response> InsertIntoMultipleTables(Dictionary<string, Dictionary<string, object>> tableData)
        {
            
            SealedSqlDAO SQLDao = new SealedSqlDAO(temp);
            var builder = new CustomSqlCommandBuilder();
            Response tablesresponse = new Response();
            // Response transactionResponse = await SQLDao.BeginTransactionAsync();
            // await SQLDao.BeginTransactionAsync();

            foreach (var tableEntry in tableData)
            {
                string tableName = tableEntry.Key;
                Dictionary<string, object> parameters = tableEntry.Value;
                var insertCommand =  builder.BeginInsert(tableName)
                    .Columns(parameters.Keys)
                    .Values(parameters.Keys)
                    .AddParameters(parameters)
                    .Build();

                tablesresponse = await SQLDao.SqlRowsAffected(insertCommand);
                if (tablesresponse.HasError)
                {
                    tablesresponse.ErrorMessage += $"{tableName}: error inserting data; ";
                    // SQLDao.RollbackTransaction();
                    return tablesresponse;
                }
            }
            // SQLDao.CommitTransaction();
            return tablesresponse;
        }


        public async Task<Response> CreateUserAccount(UserPepper userPepper, UserInfo userInfo, Dictionary<string, Dictionary<string, object>> tableData)
        {
            Response response = new Response();

            SealedSqlDAO SQLDao = new SealedSqlDAO(temp);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(temp)));


            string validationMessage = CheckUserInfoValidity(userInfo);
            if (validationMessage != "Pass")
            {
                response.HasError = true;
                response.ErrorMessage = "Invalid User Info entry: " + validationMessage;
            }
       
            //if all methods retyrn success
            response  = await InsertIntoMultipleTables(tableData);
            if (response.HasError == false)
            {
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Info",
                    username = userInfo.username,
                    category = "Data Store",
                    description = "Successful account creation"
                };
                await logger.SaveData(entry);
            }
            else{
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = userInfo.username,
                    category = "Data Store",
                    description = "Error inserting user in data store."
                };
                await logger.SaveData(entry);
            }
            return response;            
         
        }
        
    }
}

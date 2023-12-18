using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.Backend.Services.AccountCreationService
{
    public class AccountCreation : IAccountCreation
    {
        Credential removeMeLater = Credential.CreateSAUser();
        private readonly UserInfo _userInfo;
        private readonly ICustomSqlCommandBuilder _commandBuilder;

        // public AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder)
        // {
        //     _sqlDao = sqlDao;
        //     _commandBuilder = commandBuilder;
        // }
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

            //change so its not explicitly checking each field by name user variable and do loop 
            if (userInfo.username != "" && CheckNullWhiteSpace(userInfo.username) == true && userInfo.username != "NULL" && userInfo.username != "null")
            {
                allValid ++;
            }
            else {errorMsg += "Invalid email address."; }
 
            if (userInfo.firstname != "" && CheckNullWhiteSpace(userInfo.firstname) == true && userInfo.firstname != "NULL" && userInfo.firstname != "null")
            {
                allValid ++;
            }
            else {errorMsg += "Invalid first name";}
            if (userInfo.lastname != "" && CheckNullWhiteSpace(userInfo.lastname) == true && userInfo.lastname != "NULL" && userInfo.lastname != "null")
            {
                allValid ++;
            }
            else {errorMsg += "Invalid last name."; }
        
            if (allValid == 3)
            {
                errorMsg = "Pass";
            }
            return errorMsg;
        }

        //pepper 
        //  UserInfo hashedUsername = new UserInfo(hashedUser) 

         


        //this method takes builds a dictionary with several sql commands to insert all at once 
        public async Task<Response> InsertIntoMultipleTables(Dictionary<string, Dictionary<string, object>> tableData)
        {
            
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();
            Response tablesresponse = new Response();
            // Response transactionResponse = await SQLDao.BeginTransactionAsync();
            // await SQLDao.BeginTransactionAsync();

            // Check if the transaction started successfully
            // if (transactionResponse.HasError)
            // {
            //     return transactionResponse;
            // }

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


        public async Task<Response> CreateUserAccount(UserInfo userInfo, Dictionary<string, Dictionary<string, object>> tableData)
        {
            Response response = new Response();
            string validationMessage = CheckUserInfoValidity(userInfo);
            if (validationMessage != "Pass")
            {
                response.HasError = true;
                response.ErrorMessage = "Invalid User Info entry: " + validationMessage;
            }
            //if all methods retyrn success
            response  = await InsertIntoMultipleTables(tableData);
            // if (response.HasError == false)
            // {
            //     response = await InsertIntoMultipleTables(ProfiletableData);
    
            // }
            return response;            
         
        }
        
    }
}

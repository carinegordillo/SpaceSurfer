using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using SS.Backend.Services.LoggingService;
using System.Text.RegularExpressions;



namespace SS.Backend.Services.AccountCreationService
{
    public class AccountCreation : IAccountCreation
    {
        // Credential temp = Credential.CreateSAUser();
        string configFilePath = "C:/Users/kayka/Downloads/config.local.txt";
        ConfigService configService = new ConfigService("C:/Users/kayka/Downloads/config.local.txt");
        private readonly UserInfo _userInfo;
        private readonly ICustomSqlCommandBuilder _commandBuilder;

        // public AccountCreation(UserInfo userInfo)
        // {
        //     _userInfo = userInfo;
        // }

        public bool CheckNullWhiteSpace(string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
        public string CheckUserInfoValidity(UserInfo userInfo)
        {
            string errorMsg = "";
            foreach (PropertyInfo prop in userInfo.GetType().GetProperties())
            {
                var value = prop.GetValue(userInfo);
                switch (prop.Name)
                {
                    case "firstname":
                    case "lastname":
                        if (!IsValidName(value as string))
                        {
                            errorMsg += $"Invalid {prop.Name.ToLower()}; ";
                        }
                        break;
                    case "username":
                        if (!IsValidEmail(value as string))
                        {
                            string testing = "random gibbering";
                            errorMsg += "Invalid email";
                        }
                        break;
                    case "dob":
                        if (!IsValidDateOfBirth(value as DateTime?))
                        {
                            errorMsg += "Invalid date of birth; ";
                        }
                        break;
                    case "companyName":
                        if (userInfo.role == 2 || userInfo.role == 3){
                                if(!IsValidCompanyName(value as string))
                            {
                                errorMsg += $"Invalid {prop.Name.ToLower()}; ";
                            }
                            break;
                        }
                        break;
                        
                    case "address":
                        if (userInfo.role == 2 || userInfo.role == 3){
                            if(!IsValidAddress(value as string))
                            {
                                errorMsg += $"Invalid {prop.Name.ToLower()}; ";
                            }
                            break;
                        }
                        break;
                    // case "openingHours":
                    // case "closingHours":
                    // case "daysOpen": //check if these need their own function
                    //     if (CheckNullWhiteSpace(value as string))
                    //     {
                    //         errorMsg += $"Invalid {prop.Name.ToLower()}; ";
                    //     }
                    //     break;
                }
            }
            return string.IsNullOrEmpty(errorMsg) ? "Pass" : errorMsg;
        }

        private bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) &&
                name.Length >= 1 &&
                name.Length <= 50 &&
                Regex.IsMatch(name, @"^[a-zA-Z]+$");
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Length < 3)
            {
                return false;
            }

            string pattern = @"^[a-zA-Z0-9\.-]+@[a-zA-Z0-9\.-]+$";
            return Regex.IsMatch(email, pattern);
        }
        private bool IsValidDateOfBirth(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
            {
                return false;
            }

            var validStartDate = new DateTime(1970, 1, 1);
            var validEndDate = DateTime.Now;
            return dateOfBirth >= validStartDate && dateOfBirth <= validEndDate;
        }
        private bool IsValidCompanyName(string name)
        {
            return 
                name.Length >= 1 &&
                name.Length <= 60;
        }
        private bool IsValidAddress(string name)
        {
            //implement Geolocation API
            return name == "Irvine";
        }

        //this method takes builds a dictionary with several sql commands to insert all at once 
        public async Task<Response> InsertIntoMultipleTables(Dictionary<string, Dictionary<string, object>> tableData)
        {
            
            // SealedSqlDAO SQLDao = new SealedSqlDAO(temp);
            SqlDAO SQLDao = new SqlDAO(configService);
            var builder = new CustomSqlCommandBuilder();
            Response tablesresponse = new Response();

            //for each table 
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
                    return tablesresponse;
                }
            }
            return tablesresponse;
        }


        public async Task<Response> CreateUserAccount(UserInfo userInfo)
        {
            Response response = new Response();

            SqlDAO SQLDao = new SqlDAO(configService);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));


            string validationMessage = CheckUserInfoValidity(userInfo);
            if (validationMessage != "Pass")
            {
                response.HasError = true;
                response.ErrorMessage = "Invalid User Info entry: " + validationMessage;
                return response;
            }
       
            //generating sql command 
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation();
            Hashing hashing = new Hashing();

            
            var builder = new CustomSqlCommandBuilder();
            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

          
            var validPepper = new UserPepper
            {
                hashedUsername = hashing.HashData(userInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "username", userInfo.username},
                {"birthDate", userInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", userInfo.firstname},
                { "LastName", userInfo.lastname}, 
                {"backupEmail", userInfo.backupEmail},
                {"appRole", userInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", userInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"username", userInfo.username},
            };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };


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

        public async Task<Response> ReadUserTable(string tableName)
        {

            SqlDAO SQLDao = new SqlDAO(configService);
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            var insertCommand =  commandBuilder.BeginSelectAll()
                                            .From(tableName)
                                            .Build();

            response = await SQLDao.ReadSqlResult(insertCommand);
            if (response.HasError)
            {
                response.ErrorMessage += $"{tableName}: error inserting data; ";
                return response;
            }else{
                response.ErrorMessage += "- ReadUserTable- command successful -";
            }
          
            return response;

        }
    }
}

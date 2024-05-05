using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using SS.Backend.Services.LoggingService;
using SS.Backend.Services.EmailService;
using System.Reflection;
using System.Text.RegularExpressions;


namespace SS.Backend.UserManagement
{
    public class AccountCreation : IAccountCreation
    {
        private IUserManagementDao _userManagementDao;
        public AccountCreation(IUserManagementDao userManagementDao)
        {
            _userManagementDao = userManagementDao;
        }


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
                if (value as string != null){
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
                                errorMsg += "Invalid email";
                            }
                            break;
                        case "dob":
                            if (!IsValidDateOfBirth(value as DateTime?))
                            {
                                errorMsg += "Invalid date of birth; ";
                            }
                            break;
                        // case "companyName":
                        //     if (userInfo.role == 2 || userInfo.role == 3)
                        //     {
                        //         if (!IsValidCompanyName(value as string))
                        //         {
                        //             errorMsg += $"Invalid {prop.Name.ToLower()}; ";
                        //         }
                        //         break;
                        //     }
                        //     break;

                        // case "address":
                        //     if (userInfo.role == 2 || userInfo.role == 3)
                        //     {
                        //         if (!IsValidAddress(value as string))
                        //         {
                        //             errorMsg += $"Invalid {prop.Name.ToLower()}; ";
                        //         }
                        //         break;
                        //     }
                        //     break;
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
                
            }
            return string.IsNullOrEmpty(errorMsg) ? "Pass" : errorMsg;
        }

        private bool IsValidName(string? name)
        {
            return !string.IsNullOrWhiteSpace(name) &&
                name.Length >= 1 &&
                name.Length <= 50 &&
                Regex.IsMatch(name, @"^[a-zA-Z]+$");
        }
        private bool IsValidEmail(string? email)
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
        // private bool IsValidCompanyName(string? name)
        // {
        //     return
        //         name.Length >= 1 &&
        //         name.Length <= 60;
        // }
        private bool IsValidAddress(string? name)
        {
            //implement Geolocation API
            return name == "Irvine";
        }

        //this method takes builds a dictionary with several sql commands to insert all at once 
        public async Task<Response> InsertIntoMultipleTables(Dictionary<string, Dictionary<string, object>> tableData)
        {

            
            // SealedSqlDAO SQLDao = new SealedSqlDAO(temp);
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO SQLDao = new SqlDAO(configService);

            var builder = new CustomSqlCommandBuilder();
            Response tablesresponse = new Response();

            //for each table 
            foreach (var tableEntry in tableData)
            {
                string tableName = tableEntry.Key;
                Dictionary<string, object> parameters = tableEntry.Value;
                var insertCommand = builder.BeginInsert(tableName)
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


        public async Task<Response> CreateUserAccount(UserInfo userInfo, CompanyInfo? companyInfo, string? manager_hashedUsername)
        {
            Response response = new Response();
            string validationMessage = CheckUserInfoValidity(userInfo);
            if (validationMessage != "Pass")
            {
                response.HasError = true;
                response.ErrorMessage = "Invalid User Info entry: " + validationMessage;
                Console.WriteLine("VALIDATION!!!!! ", response.ErrorMessage);
                return response;
            }

            if (userInfo.role != 4){
                response  = await _userManagementDao.CreateAccount(userInfo, companyInfo, null);
            }else if (userInfo.role == 4){
                response  = await _userManagementDao.CreateAccount(userInfo, companyInfo, manager_hashedUsername);
            }

            
            Console.WriteLine("THIS IS THE REPONSE:::::", response.ErrorMessage);

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
                //await logger.SaveData(entry);
            }
            else
            {
                LogEntry entry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = userInfo.username,
                    category = "Data Store",
                    description = "Error inserting user in data store."
                };
            }

            string? targetEmail = userInfo.username;
            string? subject = $@"Verify your Space Surfer Account";
            string? msg = $@"
                Dear {userInfo.firstname},

                An account with your email has recently been registered within Space Surfer. 
                In order to enjoy and utilize the application please follow the url below in order to verify your account. 

                http://localhost:3000/VerifyAccount/index.html

                If you have any questions or need assistance, please don't hesitate to contact us at spacesurfers5@gmail.com.

                Thank you for choosing SpaceSurfer.

                Best regards,
                SpaceSurfer Team";

            try
            {
                // Send email
                await MailSender.SendEmail(targetEmail, subject, msg);
             
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }



        public async Task<Response> VerifyAccount(string username)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO SQLDao = new SqlDAO(configService);
            Response response = new Response();

            UserPepper userPepper = new UserPepper();
            Hashing hashing = new Hashing();

            
            var builder = new CustomSqlCommandBuilder();
            string pepper = "DA06";


            var validPepper = new UserPepper
            {
                hashedUsername = hashing.HashData(username, pepper)
            };

            var parameters = new Dictionary<string, object>
            {
                {"isActive", "yes"}, 
                {"hashedUsername", validPepper.hashedUsername} // Use as a parameter for the WHERE clause
            };

            var updateCommand = builder.BeginUpdate("activeAccount") // Specify the table name
                                .Set(new Dictionary<string, object> { {"isActive", true} }) // Set the isActive column
                                .Where("hashedUsername = @hashedUsername") // Specify the condition
                                .AddParameters(parameters) // Add the parameters
                                .Build();

            response = await SQLDao.SqlRowsAffected(updateCommand); // Execute the update command

            if (response.HasError)
            {
                response.ErrorMessage += "Error updating isActive column; ";
            }
            else
            {
                response.ErrorMessage += "Update isActive operation successful; ";
            }
            return response;
        }

        public async Task<Response> ReadUserTable(string tableName)
        {

            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
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
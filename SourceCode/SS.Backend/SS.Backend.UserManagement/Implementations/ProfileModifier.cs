using SS.Backend.SharedNamespace;
using System.Text.RegularExpressions;
using SS.Backend.Services.LoggingService;



namespace SS.Backend.UserManagement
{
    
    public class ProfileModifier : IProfileModifier
    {
        private readonly IUserManagementDao _userManagementDao;
        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();
        private LogEntry logEntry;

        
        public ProfileModifier(IUserManagementDao userManagementDao, ILogger logger)
        {
            _userManagementDao = userManagementDao;
            _logger = logger;
        }

        public async Task<Response> ModifyProfile(EditableUserProfile userInfo)
        {
            
            var response = new Response();
            var modifyResponse = new Response();
            string changedValues = "";
            string unchangedValues = "";

            bool hasValidationError = false; 
            bool anySuccess = false; 

            // Check firstname
            if (!string.IsNullOrWhiteSpace(userInfo.firstname))
            {
                if (IsValidName(userInfo.firstname))
                {
                    modifyResponse = await _userManagementDao.GeneralModifier("hashedUsername", userInfo.username, "firstname", userInfo.firstname, "dbo.userProfile");
                    if (!modifyResponse.HasError)
                    {
                        changedValues += "firstname, ";
                        anySuccess = true;
                    }
                    else
                    {
                        unchangedValues += "firstname, ";
                    }
                }
                else
                {
                    hasValidationError = true;
                    response.ErrorMessage += "Invalid first name format. ";
                }
            }

            
            if (!string.IsNullOrWhiteSpace(userInfo.lastname))
            {
                if (IsValidName(userInfo.lastname))
                {
                    modifyResponse = await _userManagementDao.GeneralModifier("hashedUsername", userInfo.username, "lastname", userInfo.lastname, "dbo.userProfile");
                    if (!modifyResponse.HasError)
                    {
                        changedValues += "lastname, ";
                        anySuccess = true;
                    }
                    else
                    {
                        unchangedValues += "lastname, ";
                    }
                }
                else
                {
                    hasValidationError = true;
                    response.ErrorMessage += "Invalid last name format. ";
                }
            }

            
            // if (!string.IsNullOrWhiteSpace(userInfo.backupEmail))
            // {
            //     if (IsValidEmail(userInfo.backupEmail))
            //     {
            //         modifyResponse = await _userManagementDao.GeneralModifier("hashedUsername", userInfo.username, "backupEmail", userInfo.backupEmail, "dbo.userProfile");
            //         if (!modifyResponse.HasError)
            //         {
            //             changedValues += "backupEmail, ";
            //             anySuccess = true;
            //         }
            //         else
            //         {
            //             unchangedValues += "backupEmail, ";
            //         }
            //     }
            //     else
            //     {
            //         hasValidationError = true;
            //         response.ErrorMessage += "Invalid backup email format. ";
            //     }
            // }

            
            if (hasValidationError)
            {
                logEntry = logBuilder.Error().Business().User(userInfo.username).Description($"User failed to update their profile with {unchangedValues}.").Build();
                response.HasError = true;
                response.ErrorMessage += "Could not update profile";
            }
            else if (anySuccess)
            {
                logEntry = logBuilder.Info().Business().User(userInfo.username).Description($"User updated their profile with {changedValues}.").Build();
                response.HasError = false;
                response.ErrorMessage = "Profile updated successfully";
            }
            else
            {
                logEntry = logBuilder.Info().Business().User(userInfo.username).Description($"User did not change any profile attributes.").Build();
                response.HasError = false;
                response.ErrorMessage = "No changes made to the profile";
            }

            if (logEntry != null && _logger != null)
            {
               
                _logger.SaveData(logEntry);
            }
            else
            {
                Console.WriteLine("logEntry is null");
            }
            

            return response;
        }


            private bool IsValidName(string name)
            {
                return !string.IsNullOrWhiteSpace(name) && name.All(char.IsLetter);
            }

            private bool IsValidEmail(string email)
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            }



        public async Task<Response> getUserProfile(string hashedUsername){


            Response response = await (_userManagementDao.readTableWhere("hashedUsername", hashedUsername, "dbo.userProfile"));

            return response;
        }



    }


}
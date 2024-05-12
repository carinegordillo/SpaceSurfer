using System.Data;
using SS.Backend.SharedNamespace;
using SS.Backend.Services.LoggingService;


namespace SS.Backend.UserManagement
{
    public class AccountRecovery : IAccountRecovery
    {
        private IAccountRecoveryModifier _accountRecoveryModifier;
        private IUserManagementDao _userManagementDao;
        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();
        private LogEntry logEntry;

        public AccountRecovery(IAccountRecoveryModifier accountRecoveryModifier, IUserManagementDao userManagementDao, ILogger logger)
        {
            _accountRecoveryModifier = accountRecoveryModifier;
            _userManagementDao = userManagementDao;
            _logger = logger;
        }

        /*
         * This method creates a recovery request for a user.
         * automatically sends the request status to pending
         * updates activeAccount table to mark account as pending
         * @param userHash The user's hash.
         * @param additionalInfo Additional information to be added to the request.
         * @return Response object.
         */


        public async Task<Response> createRecoveryRequest(string email, string additionalInfo = "")
        {

            string userHash = await _userManagementDao.GetHashByEmail(email);

            Console.WriteLine(userHash);

            var userRequest = new UserRequestModel
            {
                UserHash = userHash,
                RequestDate = DateTime.UtcNow,
                Status = "Pending",
                RequestType = "Recovery",
                AdditionalInformation = additionalInfo
            };

            Response response = new Response();
            Response response2 = new Response();
            
            response = await _userManagementDao.createAccountRecoveryRequest(userRequest,"dbo.userRequests" );

            if (response.HasError == false)
            {
                response.ErrorMessage += "Recovery request initiated.";
                response2 = await _accountRecoveryModifier.PendingRequest(userHash);
                if(response2.HasError == false)
                {
                    response.ErrorMessage += response2.ErrorMessage;
                }
                else
                {
                    response.ErrorMessage += response2.ErrorMessage;
                }
            }
            else
            {
                response.ErrorMessage += "Failed to initiate recovery request.";
            }
            
            //logging
            if (response.HasError == false)
            {
                logEntry = logBuilder.Info().DataStore().Description($"Initiated recovery request successfully.").User(userHash).Build();
            }
            else
            {
                logEntry = logBuilder.Error().DataStore().Description($"Failed to initiate recovery request.").User(userHash).Build();
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            
            return response;
        }
        



        /*
        * This method is used to recover a user account, and update the status of the user request in the userRequests table to accepted or denied
        * @param userHash - the hashed username of the user
        * @param adminDecision - the decision of the admin to accept or deny the request
        */

        public async Task<Response> RecoverAccount(string userHash, bool adminDecision)
        {
            Response response = new Response();


            if (adminDecision)
            {
                try
                {
                    response = await _accountRecoveryModifier.EnableAccount(userHash);
                }
                catch (Exception e)
                {
                    response.HasError = true;
                    response.ErrorMessage += e.Message + "- Recovery Request  Failed -";
                }
            
                
            }
            else
            {
                response = await _accountRecoveryModifier.ResolveRequest(userHash, "Denied");
                response.ErrorMessage = "Recovery request denied by admin.";
                
            }

            //logging
            if (response.HasError == false)
            {
                logEntry = logBuilder.Info().DataStore().Description($"Successful account recovery request.").User(userHash).Build();
            }
            else
            {
                logEntry = logBuilder.Error().DataStore().Description($"Failed to recover account.").User(userHash).Build();
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            return response;
            
        }

        /* 
        *
        * This method is used to read all the userRequests table from the database
        * @return Response - the response object
        */

        public async Task<List<UserRequestModel>> ReadUserRequests(){

            
            Response response = new Response();
            
            response = await _userManagementDao.ReadUserTable("dbo.userRequests");

            List<UserRequestModel> requestList = new List<UserRequestModel>();


            if (response.HasError == false)
            {
                if (response.ValuesRead != null)
                {
                    foreach (DataRow row in response.ValuesRead.Rows)
                    {
                        
                        string userHash = Convert.ToString(row["userHash"]);
                        Console.WriteLine(userHash);
                        Console.WriteLine("HERE");

                        string userName = null;
                        try {userName = await _userManagementDao.GetEmailByHash(userHash);}
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            userName = "Failed to retrieve username";
                        }
                        Console.WriteLine(userName);
                        #pragma warning disable CS8601 // Possible null reference assignment.
                        var userRequest = new UserRequestModel
                        {
                            RequestId = Convert.ToInt32(row["request_id"]),
                            UserHash = userHash,
                            RequestDate = Convert.ToDateTime(row["requestDate"]),
                            Status = Convert.ToString(row["status"]),
                            RequestType = Convert.ToString(row["requestType"]),
                            ResolveDate = row["resolveDate"] != DBNull.Value ? Convert.ToDateTime(row["resolveDate"]) : (DateTime?)null,
                            AdditionalInformation = row["additionalInformation"] != DBNull.Value ? Convert.ToString(row["additionalInformation"]) : null,
                            UserName = userName
                        };
                        #pragma warning restore CS8601 // Possible null reference assignment.
                        requestList.Add(userRequest);
                    }
                }
                Console.WriteLine("- ReadUserRequests successful. -");
            }
            else
            {
                Console.WriteLine( "- ReadUserRequests Failed - ");
            }

            return requestList;
        }

        public async Task<Response> ReadUserPendingRequests(){

            
            Response response = new Response();
            
            response = await _userManagementDao.readTableWhere("status", "Pending", "dbo.userRequests");

            if (response.HasError == false)
            {
                response.ErrorMessage += "- ReadUserPendingRequests successful. -";
            }
            else
            {
                response.ErrorMessage += "- ReadUserPendingRequests Failed - ";
            }

            return response;
        }
        
        

        public async Task<Response> ReadDummyTable(){

            Response response = new Response();
            
            response = await _userManagementDao.ReadUserTable("dbo.EmployeesDummyTable");

            if (response.HasError == false)
            {
                response.ErrorMessage += "- ReadDummyTable successful. -";
            }
            else
            {
                response.ErrorMessage += "- ReadDummyTable Failed - ";
            }

            return response;
        }

        public async Task<Response> sendDummyRequest(string name, string position)
        {

            Response response = new Response();
            response = await _userManagementDao.sendRequest(name, position);

            return response;

        }

        public async Task<Response> deleteUserRequestByuserHash(string userHash){
            Response response = new Response();
   
            response = await _userManagementDao.DeleteRequestWhere("userHash", userHash, "dbo.userRequests");

            if (response.HasError == false)
            {
                
                response.ErrorMessage += "- deleteUserRequestByuserHash successful. -";
            }
            else
            {
                response.ErrorMessage += "- deleteUserRequestByuserHash Failed - ";
            }

            //logging
            if (response.HasError == false)
            {
                logEntry = logBuilder.Info().DataStore().Description($"Delete user request by user hash successfully.").User(userHash).Build();
            }
            else
            {
                logEntry = logBuilder.Error().DataStore().Description($"Failed to delete user request by user hash.").User(userHash).Build();
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }

            return response;
            
        }

        public async Task<UserAccountDetails> ReadUserAccount(string userHash)
        {
            UserAccountDetails userAccountDetails = null;

            try
            {
                var activeResponse = await _userManagementDao.readTableWhere("hashedUsername", userHash, "dbo.activeAccount");

                if (!activeResponse.HasError)
                {
                    string isActive = null;

                    foreach (DataRow row in activeResponse.ValuesRead.Rows)
                    {
                        isActive = row["isActive"]?.ToString() ?? string.Empty;
                    }
                    string username = await _userManagementDao.GetEmailByHash(userHash);

                    var userResponse = await _userManagementDao.readTableWhere("username", username, "dbo.userAccount");

                    if (!userResponse.HasError)
                    {
                        foreach (DataRow row in userResponse.ValuesRead.Rows)
                        {
                            userAccountDetails = new UserAccountDetails
                            {
                                UserId = Convert.ToInt32(row["user_id"]),
                                Username = Convert.ToString(row["username"]),
                                BirthDate = Convert.ToDateTime(row["birthDate"]),
                                CompanyId = row["companyID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["companyID"]),
                                IsActive = isActive
                            };
                        }
                    }
                    else
                    {
                        throw new Exception("Couldn't read userAccount: " + userResponse.ErrorMessage);
                    }
                }
                else
                {
                    throw new Exception("Couldn't read activeAccount: " + activeResponse.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occurred: " + ex.Message);
            }

            return userAccountDetails;
        }



    }
}


using SS.Backend.SharedNamespace;


namespace SS.Backend.UserManagement
{
    public class AccountRecovery : IAccountRecovery
    {
        private IAccountRecoveryModifier _accountRecoveryModifier;
        private IUserManagementDao _userManagementDao;

        public AccountRecovery(IAccountRecoveryModifier accountRecoveryModifier, IUserManagementDao userManagementDao)
        {
            _accountRecoveryModifier = accountRecoveryModifier;
            _userManagementDao = userManagementDao;
        }

        /*
         * This method creates a recovery request for a user.
         * automatically sends the request status to pending
         * updates activeAccount table to mark account as pending
         * @param userHash The user's hash.
         * @param additionalInfo Additional information to be added to the request.
         * @return Response object.
         */


        public async Task<Response> createRecoveryRequest(string userHash, string additionalInfo = "")
        {
    
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
            return response;
            
        }

        /* 
        *
        * This method is used to read all the userRequests table from the database
        * @return Response - the response object
        */

        public async Task<Response> ReadUserRequests(){

            
            Response response = new Response();
            
            response = await _userManagementDao.ReadUserTable("dbo.userRequests");

            if (response.HasError == false)
            {
                response.ErrorMessage = "- ReadUserRequests successful. -";
            }
            else
            {
                response.ErrorMessage = "- ReadUserRequests Failed - ";
            }

            return response;
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

            return response;
            
        }


    }
}

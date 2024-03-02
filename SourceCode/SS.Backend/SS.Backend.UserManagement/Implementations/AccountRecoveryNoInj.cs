using SS.Backend.SharedNamespace;


namespace SS.Backend.UserManagement
{
    public class AccountRecoveryNoInj : IAccountRecovery
    {


        /*
         * This method creates a recovery request for a user.
         * automatically sends the request status to pending
         * @param userHash The user's hash.
         * @param additionalInfo Additional information to be added to the request.
         * @return Response object.
         */


        public async Task<Response> createRecoveryRequest(string userHash, string additionalInfo = "")
        {
            IAccountRecoveryModifier accountRecoveryModifier = new AccountRecoveryModifier();
    
            var userRequest = new UserRequestModel
            {
                UserHash = userHash,
                RequestDate = DateTime.UtcNow,
                Status = "Pending",
                RequestType = "Recovery",
                AdditionalInformation = additionalInfo
            };

            IUserManagementRepository userManagementRepository = new UserManagementRepository();
            Response response = new Response();
            
            response = await userManagementRepository.createAccountRecoveryRequest(userRequest,"dbo.userRequests" );

            if (response.HasError == false)
            {
                response.ErrorMessage += "Recovery request initiated.";
            }
            else
            {
                response.ErrorMessage += "Failed to initiate recovery request.";
            }
    
            
            return response;
        }
        

        /* 
         * This method sends a recovery request to the userManagementRepository, updates activeAccount table to mark account as pending
         * @param userHash - the hashed username of the user
         * @return Response - the response object
         */

        public async Task<Response> sendRecoveryRequest(string userHash)
        {
            IAccountRecoveryModifier accountRecoveryModifier = new AccountRecoveryModifier();

            Response response = new Response();
            response = await accountRecoveryModifier.PendingRequest(userHash);

            if (response.HasError == false)
            {
                response.ErrorMessage = "- Recovery request initiated. -";
            }
            else
            {
                response.ErrorMessage = "- Failed to initiate recovery request.- ";
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
            IAccountRecoveryModifier accountRecoveryModifier = new AccountRecoveryModifier();

            if (adminDecision)
            {
                
                response = await accountRecoveryModifier.EnableAccount(userHash);
            }
            else
            {
                response = new Response { HasError = true, ErrorMessage = "Recovery request denied by admin." };
            }
            return response;
            
        }

        /* 
        *
        * This method is used to read all the userRequests table from the database
        * @return Response - the response object
        */

        public async Task<Response> ReadUserRequests(){

            IUserManagementRepository userManagementRepository = new UserManagementRepository();
            
            Response response = new Response();
            
            response = await userManagementRepository.ReadUserTable("dbo.userRequests");

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

            IUserManagementRepository userManagementRepository = new UserManagementRepository();
            
            Response response = new Response();
            
            response = await userManagementRepository.readTableWhere("status", "Pending", "dbo.userRequests");

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

            IUserManagementRepository userManagementRepository = new UserManagementRepository();
            Response response = new Response();
            
            response = await userManagementRepository.ReadUserTable("dbo.EmployeesDummyTable");

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

            IUserManagementRepository userManagementRepository = new UserManagementRepository();

            Response response = new Response();
            response = await userManagementRepository.sendRequest(name, position);

            return response;

        }

        

        



        

    }
}
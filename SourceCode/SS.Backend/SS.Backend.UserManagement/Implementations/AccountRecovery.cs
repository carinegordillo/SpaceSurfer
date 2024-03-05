
using SS.Backend.SharedNamespace;


namespace SS.Backend.UserManagement
{
    public class AccountRecovery : IAccountRecovery
    {
        private AccountRecoveryModifier _accountRecoveryModifier;

        public AccountRecovery(AccountRecoveryModifier accountRecoveryModifier)
        {
            _accountRecoveryModifier = accountRecoveryModifier;
        }

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
        


        public async Task<Response> sendRecoveryRequest(string userHash)
        {
            
            Response response = new Response();
            response = await _accountRecoveryModifier.PendingRequest(userHash);

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

        public async Task<Response> RecoverAccount(string userHash, bool adminDecision)
        {
            Response response = new Response();

            if (adminDecision)
            {
                
                response = await _accountRecoveryModifier.EnableAccount(userHash);
            }
            else
            {
                response = new Response { HasError = true, ErrorMessage = "Recovery request denied by admin." };
            }
            return response;
            
        }

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

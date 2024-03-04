//using SS.Backend.SharedNamespace;


//namespace SS.Backend.UserManagement
//{
//    public class AccountRecoveryNoInj : IAccountRecovery
//    {


//        public async Task<Response> createRecoveryRequest(string userHash, string additionalInfo = "")
//        {
//            IAccountRecoveryModifier accountRecoveryModifier = new AccountRecoveryModifier();

//            var userRequest = new UserRequestModel
//            {
//                UserHash = userHash,
//                RequestDate = DateTime.UtcNow,
//                Status = "Pending",
//                RequestType = "Recovery",
//                AdditionalInformation = additionalInfo
//            };

//            IUserManagementRepository userManagementRepository = new UserManagementRepository();
//            Response response = new Response();

//            response = await userManagementRepository.createAccountRecoveryRequest(userRequest,"dbo.userRequests" );

//            if (response.HasError == false)
//            {
//                response.ErrorMessage += "Recovery request initiated.";
//            }
//            else
//            {
//                response.ErrorMessage += "Failed to initiate recovery request.";
//            }


//            return response;
//        }



//        public async Task<Response> sendRecoveryRequest(string userHash)
//        {
//            IAccountRecoveryModifier accountRecoveryModifier = new AccountRecoveryModifier();

//            Response response = new Response();
//            response = await accountRecoveryModifier.PendingRequest(userHash);

//            if (response.HasError == false)
//            {
//                response.ErrorMessage = "- Recovery request initiated. -";
//            }
//            else
//            {
//                response.ErrorMessage = "- Failed to initiate recovery request.- ";
//            }

//            return response;
//        }

//        public async Task<Response> RecoverAccount(string userHash, bool adminDecision)
//        {
//            Response response = new Response();
//            IAccountRecoveryModifier accountRecoveryModifier = new AccountRecoveryModifier();

//            if (adminDecision)
//            {

//                response = await accountRecoveryModifier.EnableAccount(userHash);
//            }
//            else
//            {
//                response = new Response { HasError = true, ErrorMessage = "Recovery request denied by admin." };
//            }
//            return response;

//        }

//        public async Task<Response> ReadUserRequests(){

//            IUserManagementRepository userManagementRepository = new UserManagementRepository();

//            Response response = new Response();

//            response = await userManagementRepository.ReadUserTable("dbo.userRequests");

//            if (response.HasError == false)
//            {
//                response.ErrorMessage = "- ReadUserRequests successful. -";
//            }
//            else
//            {
//                response.ErrorMessage = "- ReadUserRequests Failed - ";
//            }

//            return response;
//        }



//    }
//}
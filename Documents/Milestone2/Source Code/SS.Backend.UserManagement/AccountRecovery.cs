using SS.Backend.SharedNamespace;


namespace SS.Backend.UserManagement
{
    public class AccountRecovery : IAccountRecovery
    {
        private AccountStatusModifier _accountStatusModifier;

        public AccountRecovery(AccountStatusModifier accountStatusModifier)
        {
            _accountStatusModifier = accountStatusModifier;
        }


        public async Task<Response> sendRecoveryRequest(string userHash)
        {
            
            Response response = new Response();
            response = await _accountStatusModifier.PendingRequest(userHash);

            if (response.HasError == false)
            {
                response.ErrorMessage = "Recovery request initiated.";
            }
            else
            {
                response.ErrorMessage = "Failed to initiate recovery request.";
            }

            return response;
        }

        public async Task<Response> RecoverAccount(string userHash, bool adminDecision)
        {
            Response response = new Response();

            if (adminDecision)
            {
                
                response = await _accountStatusModifier.EnableAccount(userHash);
            }
            else
            {
                response = new Response { HasError = true, ErrorMessage = "Recovery request denied by admin." };
            }
            return response;
            
        }

    }
}
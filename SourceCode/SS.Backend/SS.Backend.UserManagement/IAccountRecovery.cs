
using SS.Backend.SharedNamespace;

namespace SS.Backend.UserManagement
{
    public interface IAccountRecovery
    {
        public Task<Response> sendRecoveryRequest(string userHash);
        public Task<Response> RecoverAccount(string userHash, bool adminDecision);
    }
}
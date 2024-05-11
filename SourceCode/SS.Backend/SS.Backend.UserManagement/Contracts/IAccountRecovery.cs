
using SS.Backend.SharedNamespace;

namespace SS.Backend.UserManagement
{
    public interface IAccountRecovery
    {
        public Task<Response> RecoverAccount(string userHash, bool adminDecision);
        public Task<Response> createRecoveryRequest(string userHash, string additionalInfo);
        public Task<List<UserRequestModel>> ReadUserRequests();
        public  Task<Response> ReadUserPendingRequests();
        public  Task<Response> ReadDummyTable();
        public  Task<Response> sendDummyRequest(string name, string position);
        public  Task<Response> deleteUserRequestByuserHash(string userHash);
        public  Task<UserAccountDetails> ReadUserAccount(string userHash);
    }
}
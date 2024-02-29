
using SS.Backend.SharedNamespace;

namespace SS.Backend.UserManagement
{
    public interface IAccountRecovery
    {
        public Task<Response> sendRecoveryRequest(string userHash);
        public Task<Response> RecoverAccount(string userHash, bool adminDecision);
        public Task<Response> createRecoveryRequest(string userHash, string additionalInfo);
        public  Task<Response> ReadUserRequests();
        public  Task<Response> ReadDummyTable();
        public  Task<Response> sendDummyRequest(string name, string position);
    }
}
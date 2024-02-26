
using SS.Backend.SharedNamespace;

namespace SS.Backend.UserManagement
{

    public interface IUserManagementRepository
    {
        public Task<Response> GeneralModifier(string whereClause, object whereClauseval, string fieldName, object newValue, string tableName);
        public Task<Response> ReadUserTable(string tableName);
        public Task<Response> createAccountRecoveryRequest(UserRequestModel userRequest, string tableName);
        

    }
}
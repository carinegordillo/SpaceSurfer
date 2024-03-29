
using SS.Backend.SharedNamespace;

namespace SS.Backend.UserManagement
{

    public interface IUserManagementDao
    {
        public Task<Response> GeneralModifier(string whereClause, object whereClauseval, string fieldName, object newValue, string tableName);
        public Task<Response> ReadUserTable(string tableName);
        public Task<Response> createAccountRecoveryRequest(UserRequestModel userRequest, string tableName);
        public  Task<Response> sendRequest(string name, string position);
        public  Task<Response> readTableWhere(string whereClause, object whereClauseval, string tableName);
        

    }
}
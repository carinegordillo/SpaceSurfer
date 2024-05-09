using SS.Backend.SharedNamespace;

namespace SS.Backend.UserManagement
{
    public interface IAccountCreation
    {
        public Task<Response> CreateUserAccount(UserInfo userInfo, CompanyInfo? companyInfo, string? manager_hashedUsername);
        public Task<Response> ReadUserTable(string tableName);
        public Task<Response> VerifyAccount(string username);
        // public Task<Response> getEmployeeCompanyID(UserInfo userInfo, string manager_hashedUsername);
    }
}
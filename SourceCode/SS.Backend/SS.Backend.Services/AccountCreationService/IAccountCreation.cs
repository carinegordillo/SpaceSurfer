using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.AccountCreationService
{
    public interface IAccountCreation
    {
        public Task<Response> CreateUserAccount(UserInfo userInfo, CompanyInfo? companyInfo);
        public Task<Response> ReadUserTable(string tableName);
        public Task<Response> VerifyAccount(string username);
        public Task<Response> getEmployeeCompanyID(UserInfo userInfo, string manager_hashedUsername);
    }
}
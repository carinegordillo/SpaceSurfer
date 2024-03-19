using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.AccountCreationService
{
    public interface IAccountCreation
    {
        public Task<Response> CreateUserAccount(UserInfo userInfo, CompanyInfo? companyInfo);
        public Task<Response> ReadUserTable(string tableName);
    }
}
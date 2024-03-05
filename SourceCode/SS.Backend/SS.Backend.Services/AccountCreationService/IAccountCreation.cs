using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.AccountCreationService
{
    public interface IAccountCreation
    {
        public Task<Response> CreateUserAccount(UserInfo userInfo);
        public Task<Response> ReadUserTable(string tableName);
    }
}
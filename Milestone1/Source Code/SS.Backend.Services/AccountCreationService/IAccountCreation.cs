using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.AccountCreationService
{
    public interface IAccountCreation
    {
        public Task<Response> CreateUserAccount(UserInfo userInfo, Dictionary<string, Dictionary<string, object>> tableData);
    }
}
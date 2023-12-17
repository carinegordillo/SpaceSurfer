using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService
{
    internal interface IDeleter
    {
        public Task<Response> DeleteAccount(string username);

    }
}

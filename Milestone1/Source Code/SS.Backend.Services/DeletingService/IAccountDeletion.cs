using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService
{
    public interface IAccountDeletion
    {
        public Task<Response> DeleteAccount(string username);

    }
}

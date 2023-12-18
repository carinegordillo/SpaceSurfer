using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService
{
    public interface IDatabaseHelper
    {
        public Task<Response> RetrieveTableNames();

    }
}
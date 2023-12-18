using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService
{
    public interface ITableNames
    {
        public Task<Response> RetrieveTableNames();

    }
}
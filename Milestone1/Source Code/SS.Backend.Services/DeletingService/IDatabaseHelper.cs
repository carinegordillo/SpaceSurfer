using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService
{
    /// <summary>
    ///     IDatabaseHelper an interface responsible of getting table names from a database.
    /// </summary>
    ///
    public interface IDatabaseHelper
    {
        public Task<Response> RetrieveTableNames();

    }
}
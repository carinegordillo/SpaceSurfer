using Microsoft.Data.SqlClient;
using SS.Backend.SharedNamespace;

namespace SS.Backend.DataAccess
{
    public interface ISqlDAO
    {
        public Task<Response> SqlRowsAffected(SqlCommand sql);

        public Task<Response> ReadSqlResult(SqlCommand sql);

    }
}
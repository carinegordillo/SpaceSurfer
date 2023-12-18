using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public interface ISqlDAO
    {
        public Task<Response> SqlRowsAffected(SqlCommand sql);

        public Task<Response> ReadSqlResult(SqlCommand sql);

    }
}
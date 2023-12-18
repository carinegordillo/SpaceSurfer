using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.Services.DeletingService
{
    public class TableNames : ITableNames
    {
        private readonly SealedSqlDAO SQLDao;

        public TableNames(SealedSqlDAO sqlDao)
        {
            this.SQLDao = sqlDao;
        }

        public async Task<Response> RetrieveTableNames()
        {
            // Your SQL query to retrieve table names
            string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";

            var command = new SqlCommand(query);

            return await SQLDao.ReadSqlResult(command);
        }
    }
}

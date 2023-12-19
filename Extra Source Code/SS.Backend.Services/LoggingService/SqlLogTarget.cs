using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.Services.LoggingService
{
    public class SqlLogTarget : ILogTarget
    {
        private readonly ISqlDAO _SqlDAO;
        public SqlLogTarget(ISqlDAO sqlDAO)
        {
            _SqlDAO = sqlDAO;
        }
        public async Task<Response> WriteData(LogEntry log)
        {
            Response result = new Response();
            try
            {
                string sql = "INSERT INTO dbo.Logs VALUES (@Timestamp, @LogLevel, @Username, @Category, @Description); SELECT SCOPE_IDENTITY();";
                var command = new SqlCommand(sql);

                command.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);
                command.Parameters.AddWithValue("@LogLevel", log.level);
                command.Parameters.AddWithValue("@Username", log.username);
                command.Parameters.AddWithValue("@Category", log.category);
                command.Parameters.AddWithValue("@Description", log.description);

                return await _SqlDAO.SqlRowsAffected(command);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = "Failed to log to the database:" + ex.Message;
                return result;
            }
        }
    }
}

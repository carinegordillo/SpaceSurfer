using SS.SharedNamespace;
using System.Data;
using System.Data.SqlClient;
using SS.Logging.LogTarget;


namespace SS.Logging.DataAccess
{

    public class SqlDAO : ISqlDAO, ILogTarget
    {
        private readonly string connectionString;

        public SqlDAO(Credential user)
        {
            this.connectionString = "Data Source=localhost; Initial Catalog=SS_Server; User Id=sa; Password=Mamba562;";
        }
        private async Task<Response> ExecuteSql(SqlConnection connection, SqlCommand command)
        {

            Response result = new Response();
            connection.Open();

            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                result.HasError = false;
                result.ErrorMessage = "Nothing was changed on the table.";
            }
            else if (rowsAffected >= 1)
            {
                result.HasError = false;
                result.ErrorMessage = rowsAffected + " rows were affected in the table.";
            }

            return result;
        }

        public async Task<Response> WriteData(LogEntry log)
        {
            Response result = new Response();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string sql = "INSERT INTO dbo.Logs VALUES (@Timestamp, @LogLevel, @Username, @Category, @Description); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@LogLevel", log.level);
                        command.Parameters.AddWithValue("@Username", log.username);
                        command.Parameters.AddWithValue("@Category", log.category);
                        command.Parameters.AddWithValue("@Description", log.description);

                        connection.Open();

                        object logIdObject = await command.ExecuteScalarAsync().ConfigureAwait(false);

                        int logId = logIdObject != null ? Convert.ToInt32(logIdObject) : -1;

                        result.Log_ID = logId;
                        result.LogEntry = log;
                        result.HasError = false;
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;

                    Console.WriteLine($"Exception in WriteData: {ex}");
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
            return result;
        }

        public async Task<Response> ReadData_Singular(int id)
        {
            Response result = new Response();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string sql = "SELECT * FROM dbo.Logs WHERE Log_ID = @id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        await connection.OpenAsync().ConfigureAwait(false);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (reader.Read())
                            {
                                DateTime timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"));
                                string level = reader.GetString(reader.GetOrdinal("Log Level"));
                                string username = reader.GetString(reader.GetOrdinal("Username"));
                                string category = reader.GetString(reader.GetOrdinal("Category"));
                                string description = reader.GetString(reader.GetOrdinal("Description"));

                                Console.WriteLine($"Timestamp: {timestamp} | Log Level: {level} | Username: {username} | Category: {category} | Description: {description}");

                                LogEntry logEntry = new LogEntry
                                {
                                    level = level,
                                    username = username,
                                    category = category,
                                    description = description
                                };

                                result.Log_ID = id;
                                result.LogEntry = logEntry;
                                result.HasError = false;
                            }
                            else
                            {
                                Console.WriteLine("Log not found.");
                                result.HasError = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage = $"Exception in ReadData_Singular: {ex}";

                    Console.WriteLine(result.ErrorMessage);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            return result;
        }

        public async Task<Response> ReadData_Multiple(int num)
        {
            Response result = new Response();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string sql = "SELECT TOP (@num) * FROM dbo.Logs";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@num", num);

                        await connection.OpenAsync().ConfigureAwait(false);

                        List<LogEntry> logs_read = new List<LogEntry>();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (reader.Read())
                            {
                                DateTime timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"));
                                string level = reader.GetString(reader.GetOrdinal("Log Level"));
                                string username = reader.GetString(reader.GetOrdinal("Username"));
                                string category = reader.GetString(reader.GetOrdinal("Category"));
                                string description = reader.GetString(reader.GetOrdinal("Description"));

                                Console.WriteLine($"Timestamp: {timestamp} | Log Level: {level} | Username: {username} | Category: {category} | Description: {description}");
                                Console.WriteLine();

                                LogEntry log = new LogEntry
                                {
                                    level = level,
                                    username = username,
                                    category = category,
                                    description = description
                                };

                                logs_read.Add(log);
                            }
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("No logs found.");
                            }
                        }

                        result.LogEntries = logs_read;
                        result.HasError = false;
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage = $"Exception in ReadData_Multiple: {ex}";

                    Console.WriteLine(result.ErrorMessage);
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            return result;
        }

        public async Task<Response> UpdateData(int id, string column, string oldData, string newData)
        {
            Response result = new Response();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "";
                switch (column)
                {
                    case "LogLevel":
                        sql = "UPDATE Logs SET [Log Level] = @newData WHERE Log_ID = @id AND [Log Level] = @oldData;";
                        break;
                    case "Username":
                        sql = "UPDATE Logs SET [Username] = @newData WHERE Log_ID = @id AND [Username] = @oldData;";
                        break;
                    case "Category":
                        sql = "UPDATE Logs SET [Category] = @newData WHERE Log_ID = @id AND [Category] = @oldData;";
                        break;
                    case "Description":
                        sql = "UPDATE Logs SET [Description] = @newData WHERE Log_ID = @id AND [Description] = @oldData;";
                        break;
                }

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@newData", newData);
                    command.Parameters.AddWithValue("@oldData", oldData);

                    result = await ExecuteSql(connection, command).ConfigureAwait(false);
                }
            }

            return result;
        }

        public async Task<Response> DeleteData(int id)
        {
            Response result = new Response();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM dbo.Logs WHERE Log_ID = @id;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    result = await ExecuteSql(connection, command).ConfigureAwait(false);
                }
            }

            return result;
        }
    }
}

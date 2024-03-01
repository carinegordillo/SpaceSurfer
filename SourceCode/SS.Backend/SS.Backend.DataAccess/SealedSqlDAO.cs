/*
using SS.Backend.SharedNamespace;
using System.Data;
using System.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public sealed class SealedSqlDAO : ISqlDAO
    {
        private readonly string connectionString;

        private SqlTransaction transaction;

        public SealedSqlDAO(Credential user)
        {
            this.connectionString = string.Format(@"Data Source=localhost;Initial Catalog=SS_Server;User Id={0};Password={1};", user.user, user.pass);
        }

        public async Task<Response> SqlRowsAffected(SqlCommand sql)
        {
            Response result = new Response();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        await connection.OpenAsync();
                    }

                    sql.Connection = connection;

                    var rowsAffected = await sql.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        result.HasError = false;
                        result.RowsAffected = rowsAffected;
                    }
                    else
                    {
                        result.HasError = true;
                        result.RowsAffected = 0;
                        result.ErrorMessage += " - Error in SqlRowsEffected -";
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage += " - Error in SqlRowsEffected - "+ ex.Message;
                }
            }

            return result;
        }

        public async Task<Response> ReadSqlResult(SqlCommand sql)
        {
            Response result = new Response();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        await connection.OpenAsync();
                    }

                    sql.Connection = connection;

                    using (SqlDataReader reader = await sql.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        List<List<object>> allValuesRead = new List<List<object>>();

                        while (reader.Read())
                        {
                            List<object> valuesRead = new List<object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                valuesRead.Add(reader[i]);
                            }

                            allValuesRead.Add(valuesRead);
                        }

                        if (allValuesRead.Count > 0)
                        {
                            result.HasError = false;
                            result.ValuesRead = allValuesRead;
                        }
                        else
                        {
                            result.HasError = true;
                            result.ErrorMessage += "No rows found.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage += "Error in Read Sql Results" +ex.Message;
                }
            }

            return result;
        }
            public async Task<Response> ExecuteSqlAsync(string sql, Dictionary<string, object> parameters)
            {
                Response response = new Response();
                SqlConnection connection = new SqlConnection(connectionString);
                try
                {
                    
                    await connection.OpenAsync();
                    transaction = connection.BeginTransaction();

                    using (var command = new SqlCommand(sql, connection, transaction))
                    {
                        // Add parameters to the command
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        response.RowsAffected = rowsAffected;

                        if (rowsAffected > 0)
                        {
                            response.HasError = false;
                            // Handle the LastInsertedId if needed
                        }
                        else
                        {
                            response.HasError = true;
                            response.ErrorMessage += "No rows affected.";
                        }
                        
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    response.HasError = true;
                    response.ErrorMessage += " could not connect to DB: "+ ex.Message;
                    // Log the error
                }
                finally
                {
                    connection?.Close();
                }

                return response;
            }

    }
}
*/
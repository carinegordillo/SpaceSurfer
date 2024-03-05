/*
using SS.Backend.SharedNamespace;
using System.Data;
using Microsoft.Data.SqlClient;


namespace SS.Backend.DataAccess
{
    public sealed class SealedSqlDAO
    {
        private readonly string connectionString;

        public SealedSqlDAO(ConfigService configService)
        {
            this.connectionString = configService.GetConnectionString();
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
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage += " - Error in SqlRowsEffected - " + ex.Message;
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

                    using (SqlDataReader reader = await sql.ExecuteReaderAsync())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        if (dataTable.Rows.Count > 0)
                        {
                            result.HasError = false;
                            //result.ValuesRead = allValuesRead;
                        }
                        else
                        {
                            result.HasError = true;
                            result.ErrorMessage = "No rows found.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage += "Error in Read Sql Results" + ex.Message;
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
                response.ErrorMessage += " could not connect to DB: " + ex.Message;
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
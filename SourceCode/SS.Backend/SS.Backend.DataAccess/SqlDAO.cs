using Microsoft.Data.SqlClient;
using SS.Backend.SharedNamespace;
using System.Data;

namespace SS.Backend.DataAccess
{
    public sealed class SqlDAO : ISqlDAO
    {
        private readonly string connectionString;

        public SqlDAO(ConfigService configService)
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
                    result.ErrorMessage = ex.Message;
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
                            result.ValuesRead = dataTable;
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
                    result.ErrorMessage = ex.Message;
                }
            }

            return result;
        }


    }
}

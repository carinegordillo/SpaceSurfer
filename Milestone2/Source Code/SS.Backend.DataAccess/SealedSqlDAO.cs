using SS.Backend.SharedNamespace;
using System.Data;
using System.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public sealed class SealedSqlDAO : ISqlDAO
    {
        private readonly string connectionString;

        public SealedSqlDAO(Credential user)
        {
            this.connectionString = string.Format(@"Data localhost\SpaceSurfer;Initial Catalog=SS_Server;User Id={0};Password={1};", user.user, user.pass);
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

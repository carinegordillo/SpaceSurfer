namespace SS.Logging.DataAccess
{
    using System;
    using System.Data.SqlClient;
    public class Result
    {
        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public string StatusCode { get; set; }

    }
    public class DataAccessClass
    {
        public Result SaveData(string classification, string username, string category, string description, string DB_user, string DB_pass)
        {
            string connectionString = string.Format(@"Data Source=localhost\SpaceSurfer;Initial Catalog=SS_Server;User Id={0};Password={1};", DB_user, DB_pass);

            var result = new Result();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string sql = "INSERT INTO dbo.LoggingTable (Classification, Username, Category, Description) VALUES (@Classification, @Username, @Category, @Description)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Classification", classification);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Category", category);
                        command.Parameters.AddWithValue("@Description", description);

                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine("Data saved successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving data: {ex.Message}");
                    result.ErrorMessage = ex.Message;
                }
                return result;
            }
        }

        public Result UpdateDescription(string oldData, string newData, string DB_user, string DB_pass)
        {
            string connectionString = string.Format(@"Data Source=localhost\SpaceSurfer;Initial Catalog=SS_Server;User Id={0};Password={1};", DB_user, DB_pass);

            var result = new Result();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string sql = "UPDATE LoggingTable SET description = @newData WHERE description = @oldData;";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@newData", newData);
                        command.Parameters.AddWithValue("@oldData", oldData);

                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine("Data updated successfully.");
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = ex.Message;
                }
                return result;
            }
        }
    }

}
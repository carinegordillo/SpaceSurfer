namespace SS.Logging.DataAccess
{
    using System;
    using System.Data.SqlClient;

    public class DataAccessClass
    {

        string connectionString = @"Data Source=localhost\SpaceSurfer;Initial Catalog=SS_Server;User Id=sa;Password=r@ysbb@ll2013;";

        public void SaveData(string username, string password, string timestamp)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string sql = "INSERT INTO LogsLogin (Username, Password, Timestamp) VALUES (@Username, @Password, @Timestamp)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Parameterized query to prevent SQL injection
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@Timestamp", timestamp);

                        command.ExecuteNonQuery(); // Execute the query
                    }

                    Console.WriteLine("Data saved successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving data: {ex.Message}");
                    // Handle the exception (log, throw, or handle as appropriate for your application)
                }
            }
        }
    }

}
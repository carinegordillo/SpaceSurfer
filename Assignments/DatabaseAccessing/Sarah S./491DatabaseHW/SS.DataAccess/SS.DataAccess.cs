using System.Data.SqlClient;

namespace SS.Logging.DataAccessing
{
    public class DataAccess
    {
        public void SaveData(string userName, string password, DateTime dateTime)
        {
            // connect to the database server
            string connectionString = "Data Source=localhost; Initial Catalog=SS_Server; User Id=sa; Password=D0ck3rHaras;";

            try
            {
                // new connection to database
                using (SqlConnection connect = new SqlConnection(connectionString))
                {
                    connect.Open();
                    string queryCommand = "INSERT INTO dbo.Login (Name, Password, Time) VALUES (@Name,@Password,@Time) ";

                    using (SqlCommand cmd = new SqlCommand(queryCommand, connect))
                    {

                        cmd.Parameters.AddWithValue("@Name", userName);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Time", dateTime);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        Console.WriteLine($"{rowsAffected} row(s) inserted successfully.");
                    }

                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error saving information: {exc.Message}");
            }
        }
    }
};
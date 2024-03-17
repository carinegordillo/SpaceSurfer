using System.Data.SqlClient;

namespace SS.Logging.DataAccessing
{
    public class DataAccessor
    {
        public void SaveData(string username, string password, DateTime currentTime)
        {
            string conString = @"Server = localhost\SpaceSurfer; Initial Catalog = SpaceSurfer; User Id = SSRegularUser; Password = abcd";

            try
            {
                using (SqlConnection connect = new SqlConnection(conString))
                {
                    connect.Open();

                    string sqlCmd = "INSERT INTO Login (UserName, Password, LogTime) VALUES (@UserName, @Password, @Time) ";

                    using (SqlCommand cmd = new SqlCommand(sqlCmd, connect))
                    {
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Time", currentTime);

                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error storing data: {ex.Message}");
            }
        }
    }
};
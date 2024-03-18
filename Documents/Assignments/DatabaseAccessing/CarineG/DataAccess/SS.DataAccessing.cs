
using System.Data.SqlClient;
namespace SS.Logging.DataAccessing
{
    public class DataAccess
    {

        public void SaveData(string userName, string password, DateTime dateTime)
        {
            string conString = "Data Source=localhost; Initial Catalog=SS_Server; User Id=sa; Password=Mamba562;";

            try
            {
                using (SqlConnection connect = new SqlConnection(conString))
                {

                    connect.Open();
                   
                    string queryCmd = "insert into dbo.Login (Name, Password, Time) VALUES (@Name,@Password,@Time) ";

                    using (SqlCommand command= new SqlCommand(queryCmd, connect))
                    {
                        command.Parameters.AddWithValue("@Name", userName);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@Time", dateTime);

                        int rowsAffected = command.ExecuteNonQuery();

                        Console.WriteLine($"{rowsAffected} row(s) inserted successfully.");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving information: {ex.Message}");
            }
        }
    }
};
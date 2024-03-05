using Microsoft.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public class userProfile
    {
        public string username { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string backup { get; set; }
        public int role { get; set; }
    }
    public class testdao
    {
        private readonly string connectionString;

        public testdao(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<List<userProfile>> getUser()
        {
            List<userProfile> profiles = new List<userProfile>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT * FROM [SS_Server].[dbo].[userProfile] WHERE hashedUsername='helloworld'\r\n";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                userProfile profile = new userProfile
                                {
                                    username = reader.GetString(0),
                                    fname = reader.GetString(1),
                                    lname = reader.GetString(2),
                                    backup = reader.GetString(3),
                                    role = reader.GetInt32(4)
                                };
                                profiles.Add(profile);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching profiles: " + ex.Message);
            }

            return profiles;
        }
    }
}

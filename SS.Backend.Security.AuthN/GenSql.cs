using System.Data.SqlClient;

namespace SS.Backend.Security.AuthN
{
    public class GenSql
    {
        public SqlCommand GenerateInsertQuery(string username, string hashedOTP, string salt)
        {
            // Generate SQL insert query for storing authentication data
            string sql = $"INSERT INTO OTP (Username, OTP, Salt, Timestamp) VALUES (@username, @hashedOTP, @salt, @timestamp)";
            var command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@hashedOTP", hashedOTP);
            command.Parameters.AddWithValue("@salt", salt);
            command.Parameters.AddWithValue("@timestamp", DateTime.UtcNow);

            return command;
        }

        public SqlCommand GenerateUpdateQuery(string username, string hashedOTP, string salt)
        {
            // Generate SQL insert query for storing authentication data
            string sql = $"UPDATE OTP SET OTP = @hashedOTP, Salt = @salt, Timestamp = @timestamp WHERE Username = @username";
            var command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@hashedOTP", hashedOTP);
            command.Parameters.AddWithValue("@salt", salt);
            command.Parameters.AddWithValue("@timestamp", DateTime.UtcNow);

            return command;
        }

        public SqlCommand GenerateReadFromOTPQuery(string username)
        {
            // Generate SQL insert query for storing authentication data
            string sql = $"SELECT * FROM OTP where Username = @username";
            var command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@username", username);

            return command;
        }

        public SqlCommand GenerateReadHashedOTPQuery(string username)
        {
            // Generate SQL select query to read hashed OTP
            string sql = $"SELECT OTP, Salt, Timestamp FROM OTP WHERE Username = @username";
            var command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@username", username);

            return command;
        }

        public SqlCommand GenerateReadRolesQuery(string username)
        {
            // Generate SQL select query to read user roles
            string sql = $"SELECT Role FROM UserRoles WHERE Username = @username";
            var command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@username", username);

            return command;
        }
    }
}

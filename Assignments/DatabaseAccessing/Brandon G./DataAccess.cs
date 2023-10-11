/// Brandon Galich
/// 013540499
/// CECS 491A
/// 10/11/2023
/// 
/// <summary>
///     The namespace <c>SS.Logging.DataAccessing</c> contains classes and components
/// related to data access and storage in the logging system.
/// </summary>
/// 
/// <remarks>
///     This namespace provides functionality for interacting with data sources,
/// such as databases, to store and retrieve log data.
/// </remarks>
using System.Data.SqlClient;

namespace SS.Logging.DataAccessing
{
    public class DataAccess
    {
        /// <summary>
        ///     SaveData connects to the database and execute the query command with values from the parameters
        /// </summary>
        ///
        /// <param name = "userName" > String value representing a username.</param>
        /// <param name = "password"> String value representing the user's password.</param>
        /// <param name = "dateTime"> Structure representing the date and time.</param>
        public void SaveData(string userName, string password, DateTime dateTime)
        {
            // Command to connect to the database server
            string conString = @"Server = localhost\SpaceSurfer; Initial Catalog = SS_Server; User Id = SSRegUser; Password = zxcvbnm";

            try
            {
                // Creates a new connection to the SQL database
                using (SqlConnection connect = new SqlConnection(conString))
                {
                    // SQL database is being connected
                    connect.Open();

                    // Command to insert three values into the Logging Table into the corresponding columns
                    string queryCmd = "INSERT INTO Logging (UserName, Password, LogTime) VALUES (@UserName, @Password, @Time) ";

                    using (SqlCommand cmd = new SqlCommand(queryCmd, connect))
                    {
                        // Query with parameters to safeguard against SQL injection attacks.
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Time", dateTime);

                        // Executes the query command with the updated parameters
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                // Handles the exception
                Console.WriteLine($"Error saving information: {ex.Message}");
            }
        }
    }
};
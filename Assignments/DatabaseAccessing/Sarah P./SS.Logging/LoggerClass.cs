using SS.Logging.DataAccess;

namespace SS.Logging
{
    public class LoggerClass
    {
        private readonly DataAccessClass dataAccess;

        public LoggerClass(DataAccessClass dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public void CreateLog(string username, string password, string timestamp)
        {
            // Logging logic
            Console.WriteLine($"Creating log: {username} {password} {timestamp}");

            // Call the data access class to save log data
            dataAccess.SaveData(username, password, timestamp);
        }
    }
}
using SS.Logging.DataAccessing;

namespace SS.Logging
{
    public class LoggingClass
    {
        private readonly DataAccessor _dataAccess;

        public LoggingClass()
        {
            _dataAccess = new DataAccessor();
        }

        public void Log(string userName, string passWord, DateTime currentTime)
        {
            Console.WriteLine($"Logging: {userName} {passWord} {currentTime}");

            _dataAccess.SaveData(userName, passWord, currentTime);
        }
    }
};
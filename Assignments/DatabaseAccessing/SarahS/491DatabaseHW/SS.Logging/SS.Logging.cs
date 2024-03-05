

namespace SS.Logging
{
    using SS.DataAccess;
    public class Logger
    {
        // Set the data 
        private readonly DataAccess _dataAccess;

        public Logger()
        {
            _dataAccess = new DataAccess();
        }

        // log user inputs
        public void Log(string uName, string pw, DateTime timeNow)
        {
            Console.WriteLine($"Username: {uName}\nPassword: {pw}\nTime: {timeNow}");
            _dataAccess.SaveData(uName, pw, timeNow);
        }
    }
};
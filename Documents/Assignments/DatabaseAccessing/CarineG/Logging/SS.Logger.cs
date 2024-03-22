

using SS.Logging.DataAccessing;

namespace SS.Logger
{
    public class Logger
    {
        private readonly DataAccess _dataAccess;

        public Logger()
        {
            _dataAccess = new DataAccess();
        }

        /// 
        public void Log(string name, string pass, DateTime currTime)
        {
            Console.WriteLine($"Username: {name}\nPassword: {pass}\nTime: {currTime}");

            _dataAccess.SaveData(name, pass, currTime);
        }
    }
};
/// Brandon Galich
/// 013540499
/// CECS 491A
/// 10/11/2023
/// 
/// <summary>
///     The <c>SS.Logging</c> namespace contains classes and components related to logging and data access.
/// </summary>
/// <remarks>
///     This namespace provides functionality for logging various types of information and interacting
/// with data sources to store log data.
/// </remarks>


using SS.Logging.DataAccessing;

namespace SS.Logging
{
    /// <summary>
    ///     A Logger class responsible for logging user information.
    /// </summary>
    public class Logger
    {
        // Setting the data using the DataAcess component to interact with the database.
        private readonly DataAccess _dataAccess;

        public Logger()
        {
            // Initializing a new instance
            _dataAccess = new DataAccess();
        }

        /// <summary>
        ///     Logs user information to the data source.
        /// </summary>
        /// 
        /// <param name = "uName"> The username to log.</param>
        /// <param name = "pw"> The password to log.</param>
        /// <param name = "timeNow"> The timestamp of the log entry.</param>
        /// 
        public void Log(string uName, string pw, DateTime timeNow)
        {
            // Logging the information
            Console.WriteLine($"Username: {uName}\nPassword: {pw}\nTime: {timeNow}");

            // Calling the method of DataAccess to input the data into the database
            _dataAccess.SaveData(uName, pw, timeNow);
        }
    }
};
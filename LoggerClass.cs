using SS.Logging.DataAccess;

namespace SS.Logging
{
    public class Result
    {
        public bool HasError { get; set; }

        public string? ErrorMessage { get; set; }

        public string StatusCode { get; set; }

    }
    public class LoggerClass
    {
        private readonly DataAccessClass dataAccess;

        public LoggerClass(DataAccessClass dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public Result CreateLog(string classification, string username, string category, string description, string DB_user, string DB_pass)
        {
            var result = new Result();

            try
            {
                // Call the data access class to save log data
                dataAccess.SaveData(classification, username, category, description, DB_user, DB_pass);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        public Result UpdateLog(string oldData, string newData, string DB_user, string DB_pass)
        {
            var result = new Result();

            try
            {
                // Call the data access class to update log data
                dataAccess.UpdateDescription(oldData, newData, DB_user, DB_pass);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
    }
}
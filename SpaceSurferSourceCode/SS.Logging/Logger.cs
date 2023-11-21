using SS.Logging.DataAccess;
using SS.SharedNamespace;
using SS.Logging.LogTarget;

namespace SS.Logging
{
    public class Logger : ILogger
    {
        private readonly ILogTarget _logTarget;
        public Logger(ILogTarget logTarget)
        {
            _logTarget = logTarget;
        }

        public bool CheckLogValidity(LogEntry log)
        {
            int bothValid = 0;
            if (log.level == "Info" || log.level == "Debug" || log.level == "Warning" || log.level == "Error")
            {
                bothValid++;
            }
            if (log.category == "View" || log.category == "Business" || log.category == "Server" || log.category == "Data" || log.category == "Data Store")
            {
                bothValid++;
            }
            if (log.getDescriptionLength <= 2000)
            {
                bothValid++;
            }
            if (bothValid == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<Response> SaveData(LogEntry log)
        {
            Response result = new Response();

            if (CheckLogValidity(log))
            {
                result = await _logTarget.WriteData(log).ConfigureAwait(false);
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = "Invalid log entry.";
            }
            return result;
        }
    }
}
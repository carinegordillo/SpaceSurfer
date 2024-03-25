using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.LoggingService
{
    public class Logger : ILogger
    {
        private readonly ILogTarget _logTarget;
        public Logger(ILogTarget logTarget)
        {
            _logTarget = logTarget;
        }

        public bool CheckNullWhiteSpace(string? str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
        public string CheckLogValidity(LogEntry log)
        {
            string errorMsg = "";
            int allValid = 0;
            if (log.level == "Info" || log.level == "Debug" || log.level == "Warning" || log.level == "Error")
            {
                allValid++;
            }
            else { errorMsg += "Invalid log level."; }
            if (log.category == "View" || log.category == "Business" || log.category == "Server" || log.category == "Data" || log.category == "Data Store")
            {
                allValid++;
            }
            else { errorMsg += "Invalid category."; }
            if (log.getDescriptionLength <= 2000 && !string.IsNullOrWhiteSpace(log.description) && log.description != "NULL" && log.description != "null")
            {
                allValid++;
            }
            else { errorMsg += "Invalid description."; }
            if (!string.IsNullOrWhiteSpace(log.username) && log.username != "NULL" && log.username != "null")
            {
                allValid++;
            }
            else { errorMsg += "Invalid username."; }
            if (allValid == 4)
            {
                errorMsg = "Pass";
            }
            return errorMsg;
        }

        public async Task<Response> SaveData(LogEntry log)
        {
            Response result = new Response();

            if (CheckLogValidity(log) == "Pass")
            {
                result = await _logTarget.WriteData(log).ConfigureAwait(false);
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage += "Invalid log entry: " + CheckLogValidity(log);
            }
            return result;
        }

    }
}

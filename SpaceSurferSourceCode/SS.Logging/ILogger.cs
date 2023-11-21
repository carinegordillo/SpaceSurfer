using SS.Logging.DataAccess;
using SS.SharedNamespace;

namespace SS.Logging
{
    public interface ILogger
    {
        public Task<Response> SaveData(LogEntry log);

    }
}


using SS.SharedNamespace;

namespace SS.Logging.LogTarget
{
    public interface ILogTarget
    {
        public Task<Response> WriteData(LogEntry log);

    }
    
}


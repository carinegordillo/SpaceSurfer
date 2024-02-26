
using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.LoggingService
{
    public interface ILogTarget
    {
        public Task<Response> WriteData(LogEntry log);

    }
}

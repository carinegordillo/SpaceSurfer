using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.LoggingService
{
    public interface ILogger
    {
        public Task<Response> SaveData(LogEntry log);

    }
}

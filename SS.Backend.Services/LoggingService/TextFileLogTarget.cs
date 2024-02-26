using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.LoggingService
{
    public class TextFileLogTarget : ILogTarget
    {
        private readonly string _filePath;

        public TextFileLogTarget(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<Response> WriteData(LogEntry log)
        {
            var logMessage = $"{log.timestamp}: [{log.level}] {log.description}";
            await File.AppendAllTextAsync(_filePath, logMessage + "\n");

            return new Response { HasError = false };
        }
    }
}

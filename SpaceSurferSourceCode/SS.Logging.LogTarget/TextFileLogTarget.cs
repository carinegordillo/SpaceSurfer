using SS.SharedNamespace;
using System.IO;
using System.Threading.Tasks;

namespace SS.Logging.LogTarget
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
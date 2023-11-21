using SS.SharedNamespace;
using System.IO;
using System.Threading.Tasks;

namespace SS.Logging.LogTarget
{
    public class CSVFileLogTarget : ILogTarget
    {
        private readonly string _filePath;

        public CSVFileLogTarget(string filePath)
        {
            _filePath = filePath;

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "Timestamp,Level,Username,Category,Description\n");
            }
            
        }

        public async Task<Response> WriteData(LogEntry log)
        {
            var logLine = $"{log.timestamp:yyyy-MM-dd HH:mm:ss},{log.level},{log.username},{log.category},{log.description.Replace(",", ";")}\n";
                
                await File.AppendAllTextAsync(_filePath, logLine);

                return new Response { HasError = false };;
        }
    }
}
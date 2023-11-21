using SS.SharedNamespace;
using System.IO;
using System.Threading.Tasks;

namespace SS.Logging.LogTarget
{
    public class ConsoleLogTarget : ILogTarget
    {
        public Task<Response> WriteData(LogEntry log)
        {
            // Format the log message (you can adjust the format as needed)
            var logMessage = $"{log.timestamp}: [{log.level}] {log.description}";

            // Write the log message to the console
            Console.WriteLine("in COnsole Log Target");
            Console.WriteLine(logMessage);

            // Return a completed task with a successful response
            return Task.FromResult(new Response { HasError = false });
        }
    }
}
using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        Credential SAUser = Credential.CreateSAUser();

        var dataAccess = new SqlDAO(SAUser);

        var log = new LogEntry
        {
            level = "Debug",
            username = "test@email",
            category = "View",
            description = "Testing File Logger"
        };
        Stopwatch timer = new Stopwatch();
        var textLogTarget = new TextFileLogTarget("\"C:\\Users\\brand\\Documents\\Examples\\SS.Logging,DataAccess\\File_Logger_Test.txt\"");
        Logger logger = new Logger(textLogTarget);

        Console.WriteLine("Finished.");

    }

}
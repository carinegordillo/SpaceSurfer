using SS.Logging;
using SS.Logging.DataAccess;
internal class Program
{
    private static void Main(string[] args)
    {

        // Create an instance of the data access class
        var dataAccess = new DataAccessClass();

        // Create an instance of the logger class, injecting the data access instance
        var logger = new LoggerClass(dataAccess);

        Console.WriteLine("How many users do you want to create?");
        int inputs = Convert.ToInt32(Console.ReadLine());

        for (int i = 0; i < inputs; i++)
        {
            Console.WriteLine("Username: ");
            string user = Console.ReadLine();
            Console.WriteLine("Password: ");
            string password = Console.ReadLine();
            DateTime timestamp = DateTime.Now;
            string timestring = timestamp.ToString();

            logger.CreateLog(user, password, timestring);
        }

        // Wait for user input to see the console output
        Console.ReadLine();

    }
}
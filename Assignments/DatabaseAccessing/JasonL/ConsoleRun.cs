using SS.Logging;

class ConsoleRun
{
    static void Main(string[] args)
    {
        bool Continue = true;

        while (Continue)
        {
            LoggingClass logger = new LoggingClass();

            Console.WriteLine("Enter username:");
            var username = Console.ReadLine() + "";

            Console.WriteLine("Enter password:");
            var password = Console.ReadLine() + "";

            DateTime currentTime = DateTime.Now;

            logger.Log(username, password, currentTime);

            Console.WriteLine("Register Again? y/n");
            var choice = Console.ReadLine() + "";

            if (choice.ToLower() == "n")
            {
                Continue = false;
            }

        }
    }
};
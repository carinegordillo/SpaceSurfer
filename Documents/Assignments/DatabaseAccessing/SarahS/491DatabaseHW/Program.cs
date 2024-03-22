using SS.Logging;
//using SS.DataAccess;
class Program
{
        static void Main(string[] args)
    {
        //continue entering data
        bool Continue = true;

        while (Continue)
        {
            // Initialize new instance
            Logger logger = new Logger();

            // Initialize user inputs
            Console.WriteLine("Enter name:");
            var name = Console.ReadLine() + "";

            Console.WriteLine("Enter password:");
            var password = Console.ReadLine() + "";

            DateTime timeNow = DateTime.Now;

            // Logs data into the database and displays an output
            logger.Log(name, password, timeNow);

            Console.WriteLine("Register another user? Y/N");
            var choice = Console.ReadLine() + "";

            if (choice.ToUpper() == "N")
            {
                Continue = false;
            }

        }
    }
};

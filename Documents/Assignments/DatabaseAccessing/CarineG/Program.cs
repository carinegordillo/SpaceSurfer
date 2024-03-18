

using SS.Logger;

class Program
{
    // Main Method
    static void Main(string[] args)
    {
        // Condition for continuosly entering data.
        bool Continue = true;

        while (Continue)
        {
            // Initializing a new instance
            Logger logger = new Logger();

            // Initializing user input for username
            Console.WriteLine("Enter name:");
            var name = Console.ReadLine() + "";

            // Initializing user input for password
            Console.WriteLine("Enter password:");
            var password = Console.ReadLine() + "";

            // Initializing variable for date and time
            DateTime now = DateTime.Now;

            // Logs data into the database and displays an output
            logger.Log(name, password, now);

            // User input to continue entering more data
            Console.WriteLine("Register another user? y/n");
            var choice = Console.ReadLine() + "";

            if (choice.ToLower() == "n")
            {
                Continue = false;
            }

        }
    }
};

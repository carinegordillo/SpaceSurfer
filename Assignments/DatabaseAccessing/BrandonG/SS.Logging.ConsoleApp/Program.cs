/// Brandon Galich
/// 013540499
/// CECS 491A
/// 10/11/2023s
/// 
/// <summary>
/// This file contains the main entry point of the SpaceSurfer application.
/// </summary>
/// <remarks>
/// The SpaceSurfer application is responsible for collecting and logging user information,
/// and this file contains the main program logic.
/// </remarks>

using SS.Logging;

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
            Console.WriteLine("Enter username:");
            var name = Console.ReadLine() + "";

            // Initializing user input for password
            Console.WriteLine("Enter password:");
            var password = Console.ReadLine() + "";

            // Initializing variable for date and time
            DateTime now = DateTime.Now;

            // Logs data into the database and displays an output
            logger.Log(name, password, now);

            // User input to continue entering more data
            Console.WriteLine("Continue? y/n");
            var choice = Console.ReadLine() + "";

            if (choice.ToLower() == "n")
            {
                Continue = false;
            }

        }
    }
};
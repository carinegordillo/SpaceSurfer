
using SS.Backend.DataAccess;

internal class Program
{
    public static async Task Main(string[] args)
    {
        testdao dao = new testdao("Data Source=localhost\\SpaceSurfers;Initial Catalog=SS_Server;User ID=sa;Password=r@ysbb@ll2013; TrustServerCertificate=True;");

        List<userProfile> profiles = new List<userProfile>();
        profiles = await dao.getUser();

        foreach (var profile in profiles)
        {
            Console.WriteLine($"Username: {profile.username}, First Name: {profile.fname}, Last Name: {profile.lname}, Backup: {profile.backup}, Role: {profile.role}");
        }
    }

    //Response result = new Response();
    //var builder = new CustomSqlCommandBuilder();
    ////string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.local.txt");
    //string configFilePath = "C:\\Users\\epik1\\source\\repos\\SS.Backend\\config.local.txt";
    //ConfigService configService = new ConfigService(configFilePath);
    //SqlDAO dao = new SqlDAO(configService);

    //var selectCommand = builder
    //            .BeginSelectAll()
    //            .From("userProfile")
    //            .Where($"hashedUsername = 'helloworld'")
    //            .Build();
    //result = await dao.ReadSqlResult(selectCommand);

    //result.PrintDataTable();

    //Response result = new Response();
    //var builder = new CustomSqlCommandBuilder();
    //string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.local.txt");
    //ConfigService configService = new ConfigService(configFilePath);
    //GenOTP genotp = new GenOTP();
    //Hashing hasher = new Hashing();
    //SqlDAO dao = new SqlDAO(configService);
    //SqlLogTarget target = new SqlLogTarget(dao);
    //Logger log = new Logger(target);
    //SSAuthService auth = new SSAuthService(genotp, hasher, dao, log);
    //AuthenticationRequest request = new AuthenticationRequest();
    //SSPrincipal principal = new SSPrincipal();

    //Console.WriteLine("Enter username: ");
    //string username = Console.ReadLine();

    //request.UserIdentity = username;
    //request.Proof = null;

    //(string otp, result) = await auth.SendOTP_and_SaveToDB(request);

    //if (result.HasError == false)
    //{
    //    Console.WriteLine($"You have received OTP: {otp}");
    //    Console.WriteLine("Enter password: ");
    //    string password = Console.ReadLine();
    //    request.Proof = password;

    //    (principal, result) = await auth.Authenticate(request);

    //    if (result.HasError == false)
    //    {
    //        Console.WriteLine("Successful authentication!");
    //        var requiredClaims = new Dictionary<string, string>
    //        {
    //            {"Role", "Admin"}
    //        };
    //        bool isAuthZ = await auth.IsAuthorize(principal, requiredClaims);

    //        if (isAuthZ)
    //        {
    //            Console.WriteLine("Successful authorization!");
    //        }
    //        else
    //        {
    //            Console.WriteLine("Failed to authorize.");
    //            Console.WriteLine(result.ErrorMessage);
    //        }
    //    }
    //    else
    //    {
    //        Console.WriteLine("Failed to authenticate.");
    //        Console.WriteLine(result.ErrorMessage);
    //    }
    //}
    //else
    //{
    //    Console.WriteLine("Send and save failed");
    //    Console.WriteLine(result.ErrorMessage);
    //}


}
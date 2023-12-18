using SS.Backend.DataAccess;
using SS.Backend.Security.AuthN;
using SS.Backend.SharedNamespace;

internal class Program
{
    public static async Task Main(string[] args)
    {
        Response result = new Response();
        Credential cred = new Credential("sa", "r@ysbb@ll2013");
        GenOTP genotp = new GenOTP();
        Hashing hasher = new Hashing();
        SqlDAO sqldao = new SqlDAO(cred);
        Authenticator authn = new Authenticator(genotp, hasher, sqldao);

        Console.WriteLine("Enter username: ");
        string username = Console.ReadLine();

        AuthenticationRequest request = new AuthenticationRequest();
        SSPrincipal principal = new SSPrincipal();
        request.UserIdentity = username;
        request.Proof = null;

        (string? sentOTP, result) = await authn.SendOTP_and_SaveToDB(request).ConfigureAwait(false);

        if (result.HasError == false)
        {
            Console.WriteLine($"You have received your OTP: {sentOTP}");
            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();
            request.Proof = password;

            (principal, result) = await authn.Authenticate(request).ConfigureAwait(false);

            if (result.HasError == false && principal != null)
            {
                Console.WriteLine($"Successful authentication!\nRoles for {principal.UserIdentity}:");

                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"{claim.Key}: {claim.Value}");
                }
            }
            else if (result.HasError == true)
            {
                Console.WriteLine($"Failure in Authenticate\nError message: {result.ErrorMessage}");
            }
            else
            {
                Console.WriteLine("Authentication failed. Principal is null.");
            }
        }
        else if (result.HasError == true)
        {
            Console.WriteLine($"Failure in SendOTP_and_SaveToDB\nError message: {result.ErrorMessage}");
        }

    }

}
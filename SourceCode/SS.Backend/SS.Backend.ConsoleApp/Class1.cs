// namespace Company.Security;

// //have a random number generator
// //public class RandomValue
// //{
// //    //byte[256] key;
// //    public static byte[] GenerateRandom(int size)
// //    {
// //        var rng = new RandomNumberGenerator.GetBytes(size);

// //        //Encoding.UTF8.GetString(rng); --> gives you a random string

// //        return 
// //    }
// //}

// public interface Class1
// {
//     // ValueTuple (specifies the identifiers) to return back two values, returns back an object that returns both entities
//     (string user, string roleName) Authenticate(string user, string password); //userID and proof, should spit out what authorizer needs, could be string (could be the role of the user)
// }

// public interface IAuthorizer
// {
//     bool IsAuthorize(string user, string securityContext); //securityContext can be an object, for this particular user, i want to know it has the securityContext
// }

// //if (auth.Authenticate("",""))
// //{
// //    if(IAuthenticator.IsAuthorize())
// //    {
// //        //do stuff
// //    }
// //}gitg

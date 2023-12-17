using System.Security.Cryptography;
using System.Text;

namespace SS.Backend.SharedNamespace
{
    public class Hashing
    {
        public string GenSalt()
        {
            byte[] salt = new byte[16];
            new Random().NextBytes(salt);
            return Convert.ToBase64String(salt);
        }
        public string HashData(string otp, string salt)
        {
            // Convert the strings to byte arrays
            byte[] otpBytes = Encoding.UTF8.GetBytes(otp);
            byte[] saltBytes = Convert.FromBase64String(salt);

            using (var keyDerivation = new Rfc2898DeriveBytes(otpBytes, saltBytes, 10000))
            {
                byte[] key = keyDerivation.GetBytes(32);

                using (var hmac = new HMACSHA256(key))
                {
                    byte[] hashBytes = hmac.ComputeHash(otpBytes);
                    return Convert.ToBase64String(hashBytes);
                }
            }
        }

    }
}

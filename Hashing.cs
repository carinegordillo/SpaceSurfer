using System.Security.Cryptography;

namespace SS.Backend.SharedNamespace
{
    public class Hashing
    {
        public byte[] PBKDF2(byte[] data, byte[] spice, int iteration, int length)
        {
            using (var key = new Rfc2898DeriveBytes(data, spice, iteration))
            {
                return key.GetBytes(length);
            }
        }

        public byte[] HMAC256(byte[] data, byte[] key)
        {
            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(data);
            }
        }
    }
}

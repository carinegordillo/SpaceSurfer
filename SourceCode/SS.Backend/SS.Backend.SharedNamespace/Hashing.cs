using System;
using System.Security.Cryptography;
using System.Text;

namespace SS.Backend.SharedNamespace
{
    public class Hashing
    {
        /// <summary>
        /// This method generates a salt that is 16 bytes
        /// </summary>
        /// <returns>Returns the salt</returns>
        public string GenSalt()
        {
            byte[] salt = new byte[16];
            new Random().NextBytes(salt);
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// This method hashes the data passed into it.
        /// </summary>
        /// <param name="data">The data to be hashed</param>
        /// <param name="salt">The salt used to append to the data to hash</param>
        /// <returns>The hashed data</returns>
        public string HashData(string data, string salt)
        {
            // Convert the strings to byte arrays
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Use recommended constructor with specified hash algorithm and iteration count
            using (var keyDerivation = new Rfc2898DeriveBytes(dataBytes, saltBytes, 10000, HashAlgorithmName.SHA256))
            {
                byte[] key = keyDerivation.GetBytes(32);

                using (var hmac = new HMACSHA256(key))
                {
                    byte[] hashBytes = hmac.ComputeHash(dataBytes);
                    return Convert.ToBase64String(hashBytes);
                }
            }
        }
    }
}

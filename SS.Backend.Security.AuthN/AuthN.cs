using SS.Backend.SharedNamespace;
using System.Text;

namespace SS.Backend.Security.AuthN
{
    public class AuthN : IAuthN
    {
        public string byteArrayToString(byte[] arr)
        {
            string res = Encoding.UTF8.GetString(arr);
            return res;
        }

        public bool compare(byte[] user_hash, byte[] db_hash)
        {
            bool match = user_hash.SequenceEqual(db_hash);
            return match;
        }

        public byte[] genOTP()
        {
            byte[] otp = new byte[8];
            new Random().NextBytes(otp);
            return otp;
        }

        Hashing hashFunctions = new Hashing();
        public byte[] hashOTP(byte[] otp)
        {
            byte[] salt = new byte[16];
            new Random().NextBytes(salt);
            byte[] key = hashFunctions.PBKDF2(otp, salt, 10000, 32);
            return hashFunctions.HMAC256(otp, key);
        }

    }
}

namespace SS.Backend.Security
{
    public class GenOTP
    {

        private static readonly Random random = new Random();
        public string generateOTP()
        {
            const string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            char[] otp = new char[8];
            for (int i = 0; i < 8; i++)
            {
                otp[i] = characters[random.Next(characters.Length)];
            }

            return new string(otp);
        }
    }
}
namespace SS.Backend.SharedNamespace
{
    public class GenOTP
    {

        private static readonly Random random = new Random();

        /// <summary>
        /// This method generates a random OTP made up of alphanumerical characters
        /// </summary>
        /// <returns>Returns an OTP that is 8 characters long</returns>
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

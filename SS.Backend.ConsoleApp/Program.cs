using SS.Backend.Security.AuthN;

internal class Program
{
    private static void Main(string[] args)
    {
        AuthN authn = new AuthN();
        byte[] salt1 = authn.genSalt();
        byte[] otp1 = authn.genOTP();
        byte[] key1 = authn.PBKDF2(otp1, salt1, 10000, 32);

        byte[] salt2 = authn.genSalt();
        byte[] otp2 = authn.genOTP();
        byte[] key2 = authn.PBKDF2(otp1, salt1, 10000, 32);

        byte[] hash1 = authn.HMAC256(otp1, key1);
        byte[] hash2 = authn.HMAC256(otp2, key2);

        Console.WriteLine("OTP 1: " + authn.byteArrayToString(otp1));
        Console.WriteLine("OTP 2: " + authn.byteArrayToString(otp2));
        Console.WriteLine("Compare: " + authn.compare(hash1, hash2));
        Console.WriteLine("Hash 1: " + authn.byteArrayToString(hash1));
        Console.WriteLine("Hash 2: " + authn.byteArrayToString(hash2));
    }

}
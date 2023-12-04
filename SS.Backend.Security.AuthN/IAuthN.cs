namespace SS.Backend.Security.AuthN
{
    public interface IAuthN
    {

        public byte[] genOTP();

        public bool compare(byte[] user_hash, byte[] db_hash);

        public string byteArrayToString(byte[] arr);

        public byte[] hashOTP(byte[] otp);

    }
}
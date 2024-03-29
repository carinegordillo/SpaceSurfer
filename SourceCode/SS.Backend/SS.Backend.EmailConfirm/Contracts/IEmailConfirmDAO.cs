using SS.Backend.SharedNamespace;

namespace SS.Backend.EmailConfirm
{
    public interface IEmailConfirmDAO
    {
        public Task<Response> GetReservationInfo(int reservationID);
        public Task<Response> GetConfirmInfo(int reservationID);
        public Task<Response> InsertConfirmationInfo(int reservationID, string otp)
        public Task<Response> UpdateConfirmStatus(int reservationID);
        public Tasks<Response> UpdateOtp(int reservationID, string otp);

    }
}
using SS.Backend.ReservationManagement;
using SS.Backend.SharedNamespace;

namespace SS.Backend.EmailConfirm
{
    public interface IEmailConfirmDAO
    {
        public Task<Response> GetReservationInfo(int reservationID);
        public Task<Response> GetConfirmInfo(int reservationID);
        public Task<Response> InsertConfirmationInfo(int reservationID, string otp, byte[] file);
        public Task<Response> UpdateConfirmStatus(int reservationID);
        public Task<Response> UpdateOtp(int reservationID, string otp);
        public Task<Response> GetUsername(string userHash);
        public Task<(UserReservationsModel,Response)> GetUserReservationByID(int reservationID);

    }
}
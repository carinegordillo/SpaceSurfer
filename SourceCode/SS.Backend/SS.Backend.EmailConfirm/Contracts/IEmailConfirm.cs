using SS.Backend.SharedNamespace;

namespace SS.Backend.EmailConfirm
{
    public interface IEmailConfirmService
    {
        public Task<(string ics, string otp, Response res)> CreateConfirmation(int reservationID);

        public Task<(string ics, string otp, Response res)> ResendConfirmation(int reservationID);
        public Task<Response> ConfirmReservation(int reservationID, string otp);
        
    }
}
using SS.Backend.SharedNamespace;

namespace SS.Backend.EmailConfirm
{
    public interface IEmailConfirm
    {
        public Task<Response> CreateConfirmation(int reservationID, string? calendarLink);
        public Task<string otp, Response res> CreateOtp();
        
    }
}
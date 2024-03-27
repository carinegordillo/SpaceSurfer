using SS.Backend.SharedNamespace;

namespace SS.Backend.EmailConfirm
{
    public interface IEmailConfirm
    {
        public Task<Response> SendConfirmation(string reservationInfo, string? calendarLink);
    }
}
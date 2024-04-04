using SS.Backend.ReservationManagement;
using SS.Backend.SharedNamespace;


namespace SS.Backend.EmailConfirm
{
    public interface IEmailConfirmSender
    {
        public Task<Response> SendConfirmation(UserReservationsModel reservation);
        
    }
}


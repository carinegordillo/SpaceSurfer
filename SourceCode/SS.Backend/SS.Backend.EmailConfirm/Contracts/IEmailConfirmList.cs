using SS.Backend.ReservationManagement;
using SS.Backend.SharedNamespace;


namespace SS.Backend.EmailConfirm
{
    public interface IEmailConfirmList
    {
        public Task<IEnumerable<UserReservationsModel>> ListConfirmations (string hashedUsername);
    }
}

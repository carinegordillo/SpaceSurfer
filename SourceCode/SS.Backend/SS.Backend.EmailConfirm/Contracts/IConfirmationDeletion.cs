using SS.Backend.ReservationManagement;
using SS.Backend.SharedNamespace;


namespace SS.Backend.EmailConfirm
{
    public interface IConfirmationDeletion
    {
        public Task<Response> CancelConfirmedReservation (string hashedUsername, int reservationID);
        public Task<Response> DeleteConfirmedReservation (int reservationID);
    }
}

using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{

    public interface IReservationDeletionService
    {
     public Task<Response> DeleteReservationAsync(string userHash, int reservationID);
    }

}
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{

    public interface IReservationCancellationService
    {
     public Task<Response> CancelReservationAsync(string tableName, int reservationID);
    }

}
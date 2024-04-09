using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;

namespace SS.Backend.ReservationManagers
{
    public interface IReservationDeletionManager
    {
      public  Task<Response> DeleteSpaceSurferSpaceReservationAsync(string userHash, int reservationID);
    }

}
using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;

namespace SS.Backend.ReservationManagers
{
    public interface IReservationCancellationManager
    {
      public  Task<Response> CancelSpaceSurferSpaceReservationAsync(UserReservationsModel userReservationsModel, string? tableNameOverride = null);
    }

}
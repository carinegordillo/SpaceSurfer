using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;

namespace SS.Backend.ReservationManagers
{

    public interface IReservationModificationManager
    {
       public  Task<Response> ModifySpaceSurferSpaceReservationAsync(UserReservationsModel userReservationsModel, string? tableNameOverride = null);
    }

}
using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;

namespace SS.Backend.ReservationManagers
{

    public interface IReservationReaderManager
    {
      public  Task<IEnumerable<UserReservationsModel>> GetAllUserSpaceSurferSpaceReservationAsync(string userName, string? tableNameOverride = null);
      public  Task<IEnumerable<UserReservationsModel>> GetAllUserActiveSpaceSurferSpaceReservationAsync(string userName, string? tableNameOverride = null);
    }

}
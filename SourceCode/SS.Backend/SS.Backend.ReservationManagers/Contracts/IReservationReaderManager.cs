using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;

namespace SS.Backend.ReservationManagers
{

    public interface IReservationReaderManager
    {
      public  Task<Response> GetAllUserSpaceSurferSpaceReservationAsync(string userName, string? tableNameOverride = null);
      public  Task<Response> GetAllUserActiveSpaceSurferSpaceReservationAsync(string userName, string? tableNameOverride = null);
    }

}
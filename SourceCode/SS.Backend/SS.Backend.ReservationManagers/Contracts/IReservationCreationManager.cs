using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;

namespace SS.Backend.ReservationManagers
{
    public interface IReservationCreationManager
    {
        Task<Response> CreateSpaceSurferSpaceReservationAsync(UserReservationsModel userReservationsModel, string? tableNameOverride = null);
        Task<Response> AddToWaitlist(UserReservationsModel userReservationsModel);
    }
}

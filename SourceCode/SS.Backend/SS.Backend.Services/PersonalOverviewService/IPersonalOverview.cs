using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.PersonalOverviewService
{
    public interface IPersonalOverview
    {
        public Task<IEnumerable<ReservationInformation>> GetUserReservationsAsync(string username, DateOnly? fromDate = null, DateOnly? toDate = null);

        public Task<Response> DeleteUserReservationsAsync(string username, int reservationID);
    }
}

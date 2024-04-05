using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.PersonalOverviewService
{
    public interface IPersonalOverview
    {
        public Task<IEnumerable<ReservationInformation>> GetUserReservationsAsync(string userId, DateOnly? fromDate = null, DateOnly? toDate = null);
    }
}

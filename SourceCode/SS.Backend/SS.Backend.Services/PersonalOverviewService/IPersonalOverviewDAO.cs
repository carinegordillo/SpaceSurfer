using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.PersonalOverviewService
{
    public interface IPersonalOverviewDAO
    {
        public Task<Response> GetReservationList(string hashedUsername, DateOnly? fromDate = null, DateOnly? toDate = null);
    }
}

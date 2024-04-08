using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.PersonalOverviewService
{
    public interface IPersonalOverviewDAO
    {
        public Task<Response> GetReservationList(string username, DateOnly? fromDate = null, DateOnly? toDate = null);

        public Task<Response> DeleteReservation(string username, int reservationID);
    }
}

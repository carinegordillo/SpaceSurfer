
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationStatusUpdater
    {
        public  Task<Response> updateReservtionStatuses(string tableName);
    }
}

using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationStatusUpdater
    {
        public  Task<Response> UpdateReservtionStatuses(string tableName, string storedProcedureName);
    }
}
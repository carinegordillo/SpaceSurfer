
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationCreatorService
    {
        public  Task<Response> CreateReservationWithAutoIDAsync(string tableName, UserReservationsModel userReservationsModel);
        public  Task<Response> CreateReservationWithManualIDAsync(string tableName, UserReservationsModel userReservationsModel);
       
    }
}
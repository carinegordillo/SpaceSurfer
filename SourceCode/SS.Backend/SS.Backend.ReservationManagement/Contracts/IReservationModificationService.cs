
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{

    public interface IReservationModificationService
    {
     public  Task<Response> ModifyReservationTimes(string tableName, UserReservationsModel userReservationsModel);

    }

}
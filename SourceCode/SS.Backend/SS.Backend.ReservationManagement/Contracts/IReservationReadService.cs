using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{

    public interface IReservationReadService
    {

     public  Task<Response> GetAllUserReservations(string tableName, string userHash);
     public  Task<Response> GetUserActiveReservations(string tableName, string userHash);
    }

}
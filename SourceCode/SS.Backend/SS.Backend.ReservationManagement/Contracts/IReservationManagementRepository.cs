
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationManagementRepository
    {
        public  Task<Response> ExecuteReadReservationTables(SqlCommand command);
        public  Task<Response> ExecuteUpdateReservationTables(SqlCommand command);
        public  Task<Response> ExecuteInsertIntoReservationsTable(SqlCommand InsertCommand);
        public  Task<Response> ReadReservationsTable(string whereClause, object whereClauseval);
    }
}
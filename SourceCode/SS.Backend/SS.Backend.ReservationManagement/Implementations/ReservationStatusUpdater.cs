using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;


namespace SS.Backend.ReservationManagement{

public class ReservationStatusUpdater : IReservationStatusUpdater
{
    private ISqlDAO _sqldao;

        public ReservationStatusUpdater(ISqlDAO sqldao)
        {
            _sqldao = sqldao;
        }

        public async Task<Response> updateReservtionStatuses(string tableName)
        {
            Console.WriteLine(tableName);
            Response response = new Response();
            var sql = $@"UPDATE {tableName}
            SET Status = 'Passed'
            WHERE DATETIMEFROMPARTS(
                    YEAR(reservationDate), 
                    MONTH(reservationDate), 
                    DAY(reservationDate), 
                    DATEPART(HOUR, reservationEndTime), 
                    DATEPART(MINUTE, reservationEndTime), 
                    DATEPART(SECOND, reservationEndTime), 
                    0) < SYSDATETIME()
            AND Status = 'Active'";

            SqlCommand command = new SqlCommand(sql);

            try
            {
                response = await _sqldao.SqlRowsAffected(command);
                if (response.HasError == false)
                {
                    response.ErrorMessage += "- Reservation Statuses updated successfully -";
                }
                else
                {
                    response.ErrorMessage += "- Reservation Statuses not updated -";
                }
                
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage += $"Error updating statuses: {ex.Message}";
            }
            return response;
        
        }

    }
}

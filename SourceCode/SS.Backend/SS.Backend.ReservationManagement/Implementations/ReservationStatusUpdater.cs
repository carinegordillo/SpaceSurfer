using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;


namespace SS.Backend.ReservationManagement{

public class ReservationStatusUpdater : IReservationStatusUpdater 
{
    private IReservationManagementRepository _reservationManagementRepository;

        public ReservationStatusUpdater(IReservationManagementRepository reservationManagementRepository)
        {
            _reservationManagementRepository = reservationManagementRepository;
        }

        public async Task<Response> UpdateReservtionStatuses(string tableName)
        {
            Response response = new Response();


            CustomSqlCommandBuilder commandBuilder = new CustomSqlCommandBuilder();
            SqlCommand command = commandBuilder.BeginStoredProcedure("UpdateReservationStatus").Build();

            try{
    
                response = await _reservationManagementRepository.ExecuteUpdateReservationTables(command);

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

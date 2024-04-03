using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SS.Backend.ReservationManagement
{
    /// <summary>
    /// Updates reservation statuses in the database.
    /// </summary>
    public class ReservationStatusUpdater : IReservationStatusUpdater 
    {
        private IReservationManagementRepository _reservationManagementRepository;

        public ReservationStatusUpdater(IReservationManagementRepository reservationManagementRepository)
        {
            _reservationManagementRepository = reservationManagementRepository;
        }

        /// <summary>
        /// Updates reservation statuses in the specified table using the specified stored procedure.
        /// </summary>
        /// <param name="tableName">The name of the table to update.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <returns>A response indicating the success or failure of the update operation.</returns>
        public async Task<Response> UpdateReservtionStatuses(string tableName, string storedProcedureName)
        {
            Response response = new Response(); 

            CustomSqlCommandBuilder commandBuilder = new CustomSqlCommandBuilder();
            SqlCommand command = commandBuilder.BeginStoredProcedure(storedProcedureName).Build();

            try
            {
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

using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SS.Backend.ReservationManagement
{
    /// <summary>
    /// Service class for modifying reservations.
    /// </summary>
    public class ReservationModificationService : IReservationModificationService
    {
        private IReservationManagementRepository _reservationManagementRepository;

        public ReservationModificationService(IReservationManagementRepository reservationManagementRepository)
        {
            _reservationManagementRepository = reservationManagementRepository;
        }

        /// <summary>
        /// Modifies the reservation times in the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="userReservationsModel">The model containing the updated reservation times.</param>
        /// <returns>A response indicating the success or failure of the modification.</returns>
        public async Task<Response> ModifyReservationTimes(string tableName, UserReservationsModel userReservationsModel)
        {
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();

            var newValues = new Dictionary<string, object>
            {
                { "reservationStartTime", userReservationsModel.ReservationStartTime },
                { "reservationEndTime", userReservationsModel.ReservationEndTime },
            };

            var UpdateReservationCommand = commandBuilder.BeginUpdate(tableName)
                                                            .Set(newValues)
                                                            .Where($"reservationID = {userReservationsModel.ReservationID}")
                                                            .AddParameters(newValues)
                                                            .Build();
            
            Console.WriteLine(UpdateReservationCommand.CommandText);

            response = await _reservationManagementRepository.ExecuteUpdateReservationTables(UpdateReservationCommand);

            if (response.HasError == false)
            {
                response.HasError = false;
            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = $"- CreateReservationWithManualIDAsync - command : {UpdateReservationCommand.CommandText} not successful - ";
            }

            return response;
        }
    }
}

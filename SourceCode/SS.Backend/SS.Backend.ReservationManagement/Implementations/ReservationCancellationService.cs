using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;


namespace SS.Backend.ReservationManagement{

public class ReservationCancellationService : IReservationCancellationService
{
    private ISqlDAO _sqldao;

        public ReservationCancellationService(ISqlDAO sqldao)
        {
            _sqldao = sqldao;
        }

        public Response checkReservationStatus(UserReservationsModel reservation)
        {
            // Directly compare with enum values
            if (reservation.Status == ReservationStatus.Cancelled)
            {
                return new Response { HasError = true, ErrorMessage = "Reservation has already been cancelled" };
            }
            else if (reservation.Status == ReservationStatus.Active)
            {
                return new Response { HasError = false, ErrorMessage = "Reservation is active" };
            }
            else if (reservation.Status == ReservationStatus.Passed)
            {
                return new Response { HasError = false, ErrorMessage = "Reservation date has passed" };
            }
            else
            {
                return new Response { HasError = true, ErrorMessage = "Invalid Status" };
            }
        }


        public async Task<Response> CancelReservationAsync(string tableName, int reservationID)
        {
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
                        {
                            { "status", "Cancelled" }
                        };

    
            var updateCommand = commandBuilder.BeginUpdate(tableName)
                                            .Set(parameters)
                                            .Where($"reservationID = {reservationID}")
                                            .AddParameters(parameters)
                                            .Build();

            response = await _sqldao.SqlRowsAffected(updateCommand);

            if (response.HasError == false)
            {
                response.ErrorMessage += "- CancelReservationAsync - command successful -";
                response.HasError = false;
            }
            else
            {
                response.ErrorMessage += $"- CancelReservationAsync - command : {updateCommand.CommandText} not successful -";
                response.HasError = true;

            }
            return response;
        }
    }
}

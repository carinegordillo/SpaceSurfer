using SS.Backend.DataAccess;
using SS.Backend.Waitlist;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;


namespace SS.Backend.ReservationManagement{

public class ReservationCancellationService : IReservationCancellationService
{
        private IReservationManagementRepository _reservationManagementRepository;
        private readonly WaitlistService _waitlist;

        public ReservationCancellationService(IReservationManagementRepository reservationManagementRepository, WaitlistService waitlist)
        {
            _reservationManagementRepository = reservationManagementRepository;
            _waitlist = waitlist;

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

            response = await _reservationManagementRepository.ExecuteUpdateReservationTables(updateCommand);

            

            if (response.HasError == false)
            {
                response.ErrorMessage += $"- CancelReservationAsync - command successful {response.ErrorMessage} -";
                response.HasError = false;

                await _waitlist.UpdateWaitlist_ApprovedUserLeft(tableName, reservationID);
            }
            else
            {
                response.ErrorMessage += $"- CancelReservationAsync - command : {updateCommand.CommandText} not successful - {response.ErrorMessage} -";
                response.HasError = true;

            }
            return response;
        }
    }
}

using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;

/* 

*/

namespace SS.Backend.ReservationManagement{


    public class ReservationReadService 
    {
        private IReservationManagementRepository _reservationManagementRepository;

        public ReservationReadService(IReservationManagementRepository reservationManagementRepository)
        {
            _reservationManagementRepository = reservationManagementRepository;
        }

        public async Task<Response> GetAllUserReservations(string tableName, string userHash){
            Response response = new Response();

            response = await _reservationManagementRepository.ReadReservationsTable("userHash", userHash, tableName);

            if (response.HasError == true){
                response.HasError = true;
                response.ErrorMessage += $"- GetAllUserReservations - command : not successful - ";
            }
            return response;
        }

        public async Task<Response> GetUserActiveReservations(string tableName, string userHash){
            Response response = new Response();

            Dictionary<string, object> parameters = new Dictionary<string, object>
                        {
                            { "userHash", userHash },
                            { "status", "Active" }
                        };

            response = await _reservationManagementRepository.ReadReservationsTableWithMutliple(tableName, parameters);

            if (response.HasError == true){
                response.ErrorMessage += $"- GetReservationByID - command : not successful - ";
            }
            return response;
        }
    }
}

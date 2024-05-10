using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;

/* 
Reads reservations from the database
*/

namespace SS.Backend.ReservationManagement{


    public class ReservationReadService : IReservationReadService
    {
        private IReservationManagementRepository _reservationManagementRepository;
    

        public ReservationReadService(IReservationManagementRepository reservationManagementRepository)
        {
            _reservationManagementRepository = reservationManagementRepository;
        }

        public async Task<Response> GetAllUserReservations(string tableName, string userHash){

            Response response = new Response();
            Response updateResponse = new Response();

            IReservationStatusUpdater _reservationStatusUpdater = new ReservationStatusUpdater(_reservationManagementRepository);

            updateResponse = await _reservationStatusUpdater.UpdateReservtionStatuses(tableName, "UpdateReservtaionStatusesPROD");

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

        public async Task<Response> GetReservationByID(string tableName, int reservationID){
            Response response = new Response();

            response = await _reservationManagementRepository.ReadReservationsTable("reservationID", reservationID, tableName);

            if (response.HasError == true){
                response.ErrorMessage += $"- GetReservationByID - command : not successful - ";
            }
            return response;
        }
    }
}

using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;

/* 

*/

namespace SS.Backend.ReservationManagement{


    public class ReservationModificationService : IReservationModificationService
    {
        private IReservationManagementRepository _reservationManagementRepository;

        public ReservationModificationService(IReservationManagementRepository reservationManagementRepository)
        {
            _reservationManagementRepository = reservationManagementRepository;
        }

        //check that the reservation hasnt passed yet 



        public async Task<Response> ModifyReservationTimes(string tableName, UserReservationsModel userReservationsModel){
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

            if (response.HasError == false){
                response.HasError = false;
    
            }
            else{
                response.HasError = true;
                response.ErrorMessage = $"- CreateReservationWithManualIDAsync - command : {UpdateReservationCommand.CommandText} not successful - ";
                

            }

            return response;

        }
    }
}

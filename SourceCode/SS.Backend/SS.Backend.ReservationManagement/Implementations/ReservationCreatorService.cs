using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;

/* 
objects needed :
user reservation model - give detials about the user reservtaions 
compnay profile - give details about the company profile 
space

*/

namespace SS.Backend.ReservationManagement{


    public class ReservationCreatorService : IReservationCreatorService
    {
        private IReservationManagementRepository _reservationManagementRepository;

        public ReservationCreatorService(IReservationManagementRepository reservationManagementRepository)
        {
            _reservationManagementRepository = reservationManagementRepository;
        }


        public async Task<Response> CreateReservationWithAutoIDAsync(string tableName, UserReservationsModel userReservationsModel){
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();


            var parameters = new Dictionary<string, object>
                        {
                            { "companyID", userReservationsModel.CompanyID },
                            { "floorPlanID", userReservationsModel.FloorPlanID},
                            { "spaceID", userReservationsModel.SpaceID },
                            { "reservationStartTime", userReservationsModel.ReservationStartTime },
                            { "reservationEndTime", userReservationsModel.ReservationEndTime },
                            { "status", userReservationsModel.Status.ToString()},
                            {"userHash", userReservationsModel.UserHash}
                        };

            var InsertReservationCommand = commandBuilder.BeginInsert(tableName)
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();

            response = await _reservationManagementRepository.ExecuteInsertIntoReservationsTable(InsertReservationCommand);


            if (response.HasError == false){
                response.ErrorMessage += "- CreateReservationWithAutoIDAsync - command successful - ";
            }
            else{
                    response.ErrorMessage += $"- CreateReservationWithAutoIDAsync - command : {InsertReservationCommand.CommandText} not successful - \n ErrorMessage {response.ErrorMessage} \n HasError {response.HasError} ";

            }
            return response;


        }

        public async Task<Response> CreateReservationWithManualIDAsync(string tableName, UserReservationsModel userReservationsModel){
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
                        {
                            { "reservationID", userReservationsModel.ReservationID},
                            { "companyID", userReservationsModel.CompanyID },
                            { "floorPlanID", userReservationsModel.FloorPlanID},
                            { "spaceID", userReservationsModel.SpaceID },
                            { "reservationStartTime", userReservationsModel.ReservationStartTime },
                            { "reservationEndTime", userReservationsModel.ReservationEndTime },
                            { "status", userReservationsModel.Status.ToString()},
                             {"userHash", userReservationsModel.UserHash}
                        };

            var InsertReservationCommand = commandBuilder.BeginInsert(tableName)
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();
            
            Console.WriteLine(InsertReservationCommand.CommandText);

            response = await _reservationManagementRepository.ExecuteInsertIntoReservationsTable(InsertReservationCommand);

            if (response.HasError == false){
                response.HasError = false;
    
            }
            else{
                response.HasError = true;
                response.ErrorMessage = $"- CreateReservationWithManualIDAsync - command : {InsertReservationCommand.CommandText} not successful - ";
                

            }

            return response;


        }
    }
}

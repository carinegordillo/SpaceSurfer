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

        private IReservationValidationService _reservationValidationService;

        public ReservationCreatorService(IReservationManagementRepository reservationManagementRepository, IReservationValidationService reservationValidationService)
        {
            _reservationManagementRepository = reservationManagementRepository;
            _reservationValidationService = reservationValidationService;
        }

        //required flags as a parameter 

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



        // public async Task<Response> CheckConflictingReservationsAsync(UserReservationsModel userReservationsModel){
            
            
        //     try
        //     {
    
        //         result = await _sqldao.ReadSqlResult(command);


        //         bool isValid =  _reservationValidationService.HasConflictingReservations(result);

        //         if (isValid == true)
        //         {
                    
        //             result.ErrorMessage += "Conflict with existing reservation.";
        //             result.HasError = true; 
        //         }
        //         else
        //         { Console.WriteLine(command.CommandText); 
                    
        //             result.ErrorMessage += $"No conflicting reservations: { command.CommandText}";
        //             result.HasError = false;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         result.HasError = true;
        //         Console.WriteLine($"Error checking for conflicting reservations: {ex.Message}");
        //         result.ErrorMessage = ex.Message;
        //     }
        //     return result;
        // }
        


        /** check if the reservtaion is within company hours**/
        // public async Task<Response> ValidateWithinHoursAsync(int companyID, UserReservationsModel userReservationsModel){
        //     Response result = new Response();

        //     string query = @"
        //         SELECT openingHours, closingHours
        //         FROM dbo.companyProfile 
        //         WHERE companyID = @companyID";

        //     SqlCommand command = new SqlCommand(query);
        //     command.Parameters.AddWithValue("@companyID", companyID);

        //     try
        //     {
        //         result = await _sqldao.ReadSqlResult(command);


        //         if (result.ValuesRead.Rows.Count > 0)
        //         {
                    
        //             bool isValid =  _reservationValidationService.IsWithinHours(result, userReservationsModel.ReservationStartTime, userReservationsModel.ReservationEndTime);
                    
                    
        //             if (isValid == true)
        //             {
                       
        //                 result.ErrorMessage = "The reservation is within company operating hours.";
        //                 result.HasError = false;
        //             }
        //             else
        //             {
                        
        //                 result.ErrorMessage = "The reservation is outside company operating hours.";
        //                 result.HasError = true;
        //             }
                    
        //         }
        //         else
        //         {
        //             result.ErrorMessage = "Company ID does not exist.";
        //             result.HasError = true;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         result.HasError = true;
        //         result.ErrorMessage += ex.Message;
        //     }
        

        //     return result;
        // }

        // public async Task<Response> ValidateReservationDurationAsync(UserReservationsModel userReservationsModel){
        //     Response result = new Response();

        //      string query = @"
        //         SELECT timeLimit
        //         FROM dbo.companyFloorSpaces 
        //         WHERE spaceID = @spaceID";

        //     SqlCommand command = new SqlCommand(query);
        //     command.Parameters.AddWithValue("@spaceID", userReservationsModel.SpaceID);


        //     try
        //     {
        //         result = await _sqldao.ReadSqlResult(command);

               
        //         if (result.ValuesRead.Rows.Count > 0)
        //         {
        //             bool isValid =  _reservationValidationService.IsValidDuration(userReservationsModel, result);
                    
        //             if (isValid == false)
        //             {
                       
        //                 result.ErrorMessage = $"The reservtaion Duration exceeds the Space Time limit.";
        //                 result.HasError = true;
        //             }
        //             else
        //             {
        //                 result.HasError = false;
        //             }
                    
        //         }
        //         else
        //         {
        //             result.ErrorMessage = "SpaceID does not exist or is Invalid.";
        //             result.HasError = true;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         result.HasError = true;
        //         result.ErrorMessage += ex.Message;
        //     }

        //     return result;
        // }

        // public Response ValidateReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime){
            

        //     Response result = new Response();
            
        //     var isValid =  _reservationValidationService.IsValidReservationLeadTime(userReservationsModel, maxLeadTime, unitOfTime);

        //     if (isValid == true)
        //     {
        //         result.HasError = false;
        //     }
        //     else
        //     {
        //         result.HasError = true;
        //         result.ErrorMessage = "Reservation exceeds the allowed lead time.";
        //     }

        //     return result;

        // }

        

    }
}

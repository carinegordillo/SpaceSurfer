using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Data;


/*
    This class is responsible for modifying reservations in the database.
    It contains methods for creating reservations, checking for conflicting reservations, 
    checking if the reservation is within company hours, checking if the reservation duration is valid,
    and checking if the reservation lead time is valid.

    Users can only modify :
    - 
*/


namespace SS.Backend.ReservationManagement{


    public class ReservationModificationService 
    {
        private ISqlDAO _sqldao;

        private IReservationValidationService _reservationValidationService;

        public ReservationModificationService(ISqlDAO sqldao, IReservationValidationService reservationValidationService)
        {
            _sqldao = sqldao;
            _reservationValidationService = reservationValidationService;
        }

        public async Task<Response> CreateReservationWithAutoIDAsync(string tableName, UserReservationsModel userReservationsModel){
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();


            var parameters = new Dictionary<string, object>
                        {
                            { "companyID", userReservationsModel.CompanyID },
                            { "floorPlanID", userReservationsModel.FloorPlanID},
                            { "spaceID", userReservationsModel.SpaceID },
                            { "reservationDate", userReservationsModel.ReservationDate},
                            { "reservationStartTime", userReservationsModel.ReservationStartTime },
                            { "reservationEndTime", userReservationsModel.ReservationEndTime },
                            { "status", userReservationsModel.Status.ToString()},
                        };

            var InsertRequestsCommand = commandBuilder.BeginInsert(tableName)
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();

            response = await _sqldao.SqlRowsAffected(InsertRequestsCommand);

            if (response.HasError == false){
                response.ErrorMessage += "- CreateReservationWithAutoIDAsync - command successful - ";
            }
            else{
                    response.ErrorMessage += $"- CreateReservationWithAutoIDAsync - command : {InsertRequestsCommand.CommandText} not successful - ";

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
                            { "reservationDate", userReservationsModel.ReservationDate},
                            { "reservationStartTime", userReservationsModel.ReservationStartTime },
                            { "reservationEndTime", userReservationsModel.ReservationEndTime },
                            { "status", userReservationsModel.Status.ToString()},
                        };

            var InsertRequestsCommand = commandBuilder.BeginInsert(tableName)
                                                            .Columns(parameters.Keys)
                                                            .Values(parameters.Keys)
                                                            .AddParameters(parameters)
                                                            .Build();

            response = await _sqldao.SqlRowsAffected(InsertRequestsCommand);

            if (response.HasError == false){
                response.HasError = false;
    
            }
            else{
                response.HasError = true;
                response.ErrorMessage = $"- CreateReservationWithManualIDAsync - command : {InsertRequestsCommand.CommandText} not successful - ";
                

            }

            return response;


        }




        public async Task<Response> CheckConflictingReservationsAsync(int floorPlanID, string spaceID, TimeSpan proposedStart, TimeSpan proposedEnd){
            
            Response result = new Response();

            string query = @"
                SELECT reservationID
                FROM dbo.Reservations 
                WHERE floorPlanID = @floorPlanID AND spaceID = @spaceID AND status = 'Active'
              AND (reservationStartTime < @reservationEndTime AND reservationEndTime > @reservationStartTime)";

  
            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@floorPlanID", floorPlanID);
            command.Parameters.AddWithValue("@spaceID", spaceID);
            command.Parameters.AddWithValue("@reservationStartTime", proposedStart);
            command.Parameters.AddWithValue("@reservationEndTime", proposedEnd);

            try
            {
                // Assuming _sqldao.ReadSqlResult executes the command and returns a DataTable
                result = await _sqldao.ReadSqlResult(command);

                bool isValid =  _reservationValidationService.HasConflictingReservations(result);

                if (isValid == true)
                {
                    // Conflicts exist
                    result.ErrorMessage = "Conflict with existing reservation.";
                    result.HasError = true; 
                }
                else
                {
                    // No conflicts
                    result.ErrorMessage = "No conflicting reservations.";
                    result.HasError = false;
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                Console.WriteLine($"Error checking for conflicting reservations: {ex.Message}");
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        


        /** check if the reservtaion is within company hours**/
        public async Task<Response> ValidateWithinHoursAsync(int companyID, TimeSpan proposedStart, TimeSpan proposedEnd){
            Response result = new Response();

            string query = @"
                SELECT openingHours, closingHours
                FROM dbo.companyProfile 
                WHERE companyID = @companyID";

            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@companyID", companyID);

            try
            {
                result = await _sqldao.ReadSqlResult(command);


                if (result.ValuesRead.Rows.Count > 0)
                {
                    
                    bool isValid =  _reservationValidationService.IsWithinHours(result, proposedStart, proposedEnd);
                    
                    
                    if (isValid == true)
                    {
                       
                        result.ErrorMessage = "The reservation is within company operating hours.";
                        result.HasError = false;
                    }
                    else
                    {
                        
                        result.ErrorMessage = "The reservation is outside company operating hours.";
                        result.HasError = true;
                    }
                    
                }
                else
                {
                    result.ErrorMessage = "Company ID does not exist.";
                    result.HasError = true;
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage += ex.Message;
            }
        

            return result;
        }

        public async Task<Response> ValidateReservationDurationAsync(UserReservationsModel userReservationsModel){
            Response result = new Response();

             string query = @"
                SELECT timeLimit
                FROM dbo.companyFloorSpaces 
                WHERE spaceID = @spaceID";

            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@spaceID", userReservationsModel.SpaceID);


            try
            {
                result = await _sqldao.ReadSqlResult(command);

               
                if (result.ValuesRead.Rows.Count > 0)
                {
                    bool isValid =  _reservationValidationService.IsValidDuration(userReservationsModel, result);
                    
                    if (isValid == false)
                    {
                       
                        result.ErrorMessage = $"The reservtaion Duration exceeds the Space Time limit.";
                        result.HasError = true;
                    }
                    else
                    {
                        result.HasError = false;
                    }
                    
                }
                else
                {
                    result.ErrorMessage = "SpaceID does not exist or is Invalid.";
                    result.HasError = true;
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage += ex.Message;
            }

            return result;
        }

        public Response ValidateReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime){
            

            Response result = new Response();
            
            var isValid =  _reservationValidationService.IsValidReservationLeadTime(userReservationsModel, maxLeadTime, unitOfTime);

            if (isValid == true)
            {
                result.HasError = false;
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = "Reservation exceeds the allowed lead time.";
            }

            return result;

        }

        

    }
}

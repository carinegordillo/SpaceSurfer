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

namespace SS.Backend.ReservationServices{
    public enum TimeUnit
    {
        Days,
        Hours,
        Minutes
    }

    public class ReservationCreation : IReservationCreation
    {
        private ISqlDAO _sqldao;

        public ReservationCreation(ISqlDAO sqldao)
        {
            _sqldao = sqldao;
        }

        public async Task<Response> CreateReservationWithAutoID(string tableName, UserReservationsModel userReservationsModel){
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
                response.ErrorMessage += "- CreateReservationWithAutoID - command successful - ";
            }
            else{
                    response.ErrorMessage += $"- CreateReservationWithAutoID - command : {InsertRequestsCommand.CommandText} not successful - ";

            }
            return response;


        }

        public async Task<Response> CreateReservationWithManualID(string tableName, UserReservationsModel userReservationsModel){
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();

            Console.WriteLine("inside CreateReservationWithManualID with tableName: " + tableName+"and userReservationsModel: "+userReservationsModel.ReservationID);


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
                response.ErrorMessage += "- CreateReservationWithManualID - command successful - ";
            }
            else{
                response.HasError = true;
                response.ErrorMessage += $"- CreateReservationWithManualID - command : {InsertRequestsCommand.CommandText} not successful - ";
                

            }

            Console.WriteLine(response.ErrorMessage);
            Console.WriteLine("in methos " + response.HasError);
            return response;


        }




        public async Task<Response> CheckConflictingReservations(int floorPlanID, string spaceID, TimeSpan proposedStart, TimeSpan proposedEnd){
            
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
                

                if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0 )
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
        public async Task<Response> ValidateWithinHours(int companyID, TimeSpan proposedStart, TimeSpan proposedEnd){
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
                    
                    TimeSpan openingHours = (TimeSpan)(result.ValuesRead.Rows[0]["openingHours"]); 
                    TimeSpan closingHours = (TimeSpan)(result.ValuesRead.Rows[0]["closingHours"]);
                    
                    
                    if (proposedStart >= openingHours && proposedEnd <= closingHours)
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

        public async Task<Response> ValidateReservationDuration(UserReservationsModel userReservationsModel){
            Response result = new Response();

             string query = @"
                SELECT timeLimit
                FROM dbo.companyFloorSpaces 
                WHERE spaceID = @spaceID";

            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@spaceID", userReservationsModel.SpaceID);

            var userReservationDuration = userReservationsModel.ReservationEndTime - userReservationsModel.ReservationStartTime;

            try
            {
                result = await _sqldao.ReadSqlResult(command);

                if (result.ValuesRead.Rows.Count > 0)
                {
                    
                    int spaceTimeLimitInMinutes = ((int)(result.ValuesRead.Rows[0]["timeLimit"])*60); 
                    
                    if (userReservationDuration.TotalMinutes >= spaceTimeLimitInMinutes)
                    {
                       
                        result.ErrorMessage = $"The Space has a time limit of {spaceTimeLimitInMinutes} minutes. The reservation duration of {userReservationDuration} exceeds the time limit.";
                        result.HasError = true;
                    }
                    else
                    {
                        result.ErrorMessage = $"The Space has a time limit of {spaceTimeLimitInMinutes} minutes. The reservation duration of {userReservationDuration} DOES NOT exceed the time limit.";
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

        public async Task<Response> validateReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime){
            var currentDateTime = DateTime.UtcNow; 
            var maxLeadDateTime = currentDateTime;

            Response result = new Response();

            
            switch (unitOfTime)
            {
                case TimeUnit.Days:
                    maxLeadDateTime = currentDateTime.AddDays(maxLeadTime);
                    break;
                case TimeUnit.Hours:
                    maxLeadDateTime = currentDateTime.AddHours(maxLeadTime);
                    break;
                case TimeUnit.Minutes:
                    maxLeadDateTime = currentDateTime.AddMinutes(maxLeadTime);
                    break;
                default:
                    throw new ArgumentException("Unsupported time unit");
            }

            var reservationDateTime = userReservationsModel.ReservationDate.Date + userReservationsModel.ReservationStartTime;

            if (reservationDateTime <= maxLeadDateTime)
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

        

        // public ConfirmReservation(){ sata

        // }

    }
}

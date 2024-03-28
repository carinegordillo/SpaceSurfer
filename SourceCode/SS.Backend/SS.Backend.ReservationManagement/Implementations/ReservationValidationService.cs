
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using System.Data;


/*
what i need from the company profile:
 - open and closing hours
 - still need to implement the days 
 - time limit for the space 
*/

namespace SS.Backend.ReservationManagement{
    public enum TimeUnit
    {
        Days,
        Hours,
        Minutes
    }

    public class ReservationValidationService : IReservationValidationService
    {
        private IReservationManagementRepository _reservationManagementRepository;
        public ReservationValidationService(IReservationManagementRepository reservationManagementRepository)
        {
            _reservationManagementRepository = reservationManagementRepository;
        }

        public async Task<Response> ValidateNoConflictingReservationsAsync(UserReservationsModel userReservationsModel){
            Response result = new Response();

            Dictionary<string, object> reservationParameters = new Dictionary<string, object>
            {
                { "companyID", userReservationsModel.CompanyID },
                {"floorPlanID", userReservationsModel.FloorPlanID },
                { "spaceID", userReservationsModel.SpaceID },
                { "proposedStartDateTime", userReservationsModel.ReservationStartTime },
                { "proposedEndDateTime", userReservationsModel.ReservationEndTime }
            };

            CustomSqlCommandBuilder commandBuilder = new CustomSqlCommandBuilder();
            SqlCommand command = commandBuilder.BeginStoredProcedure("CheckConflictingReservations")
                                        .AddParameters(reservationParameters)
                                        .Build();
            try{
    
                result = await _reservationManagementRepository.ExecuteReadReservationTables(command);

            }
            catch (Exception ex)
            {
                result.HasError = true;
                Console.WriteLine($"Error checking for conflicting reservations: {ex.Message}");
                result.ErrorMessage = ex.Message;
            }
            

            if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0 )
            {
                result.HasError = true;
                result.ErrorMessage = "There is a conflicting reservation.";
                
            }

            else
            {
                result.HasError = false;
            }

           
    
            return result;
        }
        


        /** check if the reservtaion is within company hours**/

        public async Task<Response> ValidateWithinHoursAsync(UserReservationsModel userReservationsModel){
            Response result = new Response();


            CustomSqlCommandBuilder commandBuilder = new CustomSqlCommandBuilder();

            SqlCommand command = commandBuilder.BeginSelect() 
                .SelectColumns("openingHours, closingHours")
                .From("dbo.companyProfile") 
                .Where("companyID = @companyID") 
                .AddParameters(new Dictionary<string, object> { { "companyID", userReservationsModel.CompanyID } }) 
                .Build();

            try
            {
                result = await _reservationManagementRepository.ExecuteReadReservationTables(command);

                Console.WriteLine(result.HasError);
                Console.WriteLine(result.ErrorMessage);

                if (result.ValuesRead.Rows.Count > 0)
                {
                    
                    bool isValid =  IsWithinHours(result, userReservationsModel.ReservationStartTime, userReservationsModel.ReservationEndTime);

                    if (isValid == false)
                    {
                        result.ErrorMessage = $"The reservation is not within the company hours.";
                        result.HasError = true;
                    }
                    else
                    {
                        result.HasError = false;
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
        public bool IsWithinHours(Response result, DateTime proposedStart, DateTime proposedEnd){
            
            TimeSpan proposedStartTime = proposedStart.TimeOfDay;
            TimeSpan proposedEndTime = proposedEnd.TimeOfDay;
                
            TimeSpan openingHours = (TimeSpan)(result.ValuesRead.Rows[0]["openingHours"]); 
            TimeSpan closingHours = (TimeSpan)(result.ValuesRead.Rows[0]["closingHours"]);
            
            
            if (proposedStartTime >= openingHours && proposedEndTime <= closingHours)
            {
                Console.WriteLine("This is True");
                return true;
            }

                    
            
            return false;
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
                result = await _reservationManagementRepository.ExecuteReadReservationTables(command);

               
                if (result.ValuesRead.Rows.Count > 0)
                {
                    bool isValid =  IsValidMaxDuration(userReservationsModel, result);
                    
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

        public bool IsValidMaxDuration(UserReservationsModel userReservationsModel, Response result){
        

            var userReservationDuration = userReservationsModel.ReservationEndTime - userReservationsModel.ReservationStartTime;

            if (result.ValuesRead.Rows.Count > 0)
            {
                
                int spaceTimeLimitInMinutes = ((int)(result.ValuesRead.Rows[0]["timeLimit"])*60); 
                
                if (userReservationDuration.TotalMinutes >= spaceTimeLimitInMinutes)
                {
                    
                    return false;
                }

                else
                {
                    return true;
                }
                
                
            }
                
            return false;
        }

        public bool IsValidMinDuration(UserReservationsModel userReservationsModel){
        

            var userReservationDuration = userReservationsModel.ReservationEndTime - userReservationsModel.ReservationStartTime;

                
            if (userReservationDuration.TotalMinutes >= 60)
            {
                    
                return true;
            }
            
                
            return false;
        }

        public Response ValidateReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime){
            

            Response result = new Response();
            
            var isValid =  IsValidReservationLeadTime(userReservationsModel, maxLeadTime, unitOfTime);

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

        public bool  IsValidReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime){
            var currentDateTime = DateTime.UtcNow; 
            var maxLeadDateTime = currentDateTime;

            
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


            if (userReservationsModel.ReservationEndTime <= maxLeadDateTime)
            {
                return true;
                
            }
            
            
            return false;

        }

    }
}

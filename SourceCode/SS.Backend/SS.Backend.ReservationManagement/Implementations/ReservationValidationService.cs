
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using System.Data;

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
            SqlCommand command = commandBuilder.BeginStoredProcedure("CheckConflictingReservationsPROD")
                                        .AddParameters(reservationParameters)
                                        .Build();
            try{
    
                result = await _reservationManagementRepository.ExecuteReadReservationTables(command);

            }
            catch (Exception ex)
            {
                result.HasError = true;
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
                .SelectColumns("openingHours, closingHours, daysOpen")
                .From("dbo.companyProfile") 
                .Where("companyID = @companyID") 
                .AddParameters(new Dictionary<string, object> { { "companyID", userReservationsModel.CompanyID } }) 
                .Build();

            try
            {
                result = await _reservationManagementRepository.ExecuteReadReservationTables(command);


                if (result.ValuesRead.Rows.Count > 0)
                {
                    string openDays = result.ValuesRead.Rows[0]["daysOpen"].ToString();

                    bool isOpenDay = openDays.Contains(userReservationsModel.ReservationStartTime.ToString("dddd"), StringComparison.OrdinalIgnoreCase);

                    if (!isOpenDay)
                    {
                        result.ErrorMessage = "The reservation day is not within business days.";
                        result.HasError = true;
                        return result;
                    }
                    
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

        public bool IsValidMinDuration(UserReservationsModel userReservationsModel, TimeSpan minDuration)
        {
            var userReservationDuration = userReservationsModel.ReservationEndTime - userReservationsModel.ReservationStartTime;

            if (userReservationDuration >= minDuration)
            {
                return true;
            }

            return false;
        }

        public Response ValidateReservationLeadTime(UserReservationsModel userReservationsModel, TimeSpan maxLeadTime){
            

            Response result = new Response();
            
            var isValid =  IsValidReservationLeadTime(userReservationsModel, maxLeadTime);

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

        public Response checkReservationStatus(UserReservationsModel reservation)
        {
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

        public bool IsValidReservationLeadTime(UserReservationsModel userReservationsModel, TimeSpan maxLeadTime)
        {
            var currentDateTime = DateTime.UtcNow;
            var maxLeadDateTime = currentDateTime + maxLeadTime;

            return userReservationsModel.ReservationEndTime <= maxLeadDateTime;
        }

       public async Task<Response> ValidateReservationAsync(UserReservationsModel userReservationsModel, ReservationValidationFlags validationFlags, IReservationRequirements requirements)
        {
            var response = new Response();
            List<string> failedValidations = new List<string>(); 
            
            if (validationFlags.HasFlag(ReservationValidationFlags.CheckBusinessHours))
            {
                var businessHoursResponse = await ValidateWithinHoursAsync(userReservationsModel);
                if (businessHoursResponse.HasError)
                {
                    failedValidations.Add("CheckBusinessHours");
                }
            }

            if (validationFlags.HasFlag(ReservationValidationFlags.MinReservationDuration))
            {
                if (!IsValidMinDuration(userReservationsModel, requirements.MinDuration))
                {
                    failedValidations.Add($"MinReservationDuration (minimum {requirements.MinDuration.TotalMinutes} minutes)");
                }
            }

            if (validationFlags.HasFlag(ReservationValidationFlags.MaxDurationPerSeat))
            {
                var maxDurationResponse = await ValidateReservationDurationAsync(userReservationsModel);
                if (maxDurationResponse.HasError)
                {
                    failedValidations.Add("MaxDurationPerSeat");
                }
            }

            if (validationFlags.HasFlag(ReservationValidationFlags.ReservationLeadTime))
            {
                if (!IsValidReservationLeadTime(userReservationsModel, requirements.MaxAdvanceReservationTime))
                {
                    failedValidations.Add($"ReservationLeadTime (more than {requirements.MaxAdvanceReservationTime.TotalDays} days in advance)");
                }
            }

            if (validationFlags.HasFlag(ReservationValidationFlags.ReservationStatusIsActive))
            {
            
                var statusResponse = checkReservationStatus(userReservationsModel);
                if (statusResponse.HasError)
                {
                    failedValidations.Add("ReservationStatusIsActive");
                    response.ErrorMessage += statusResponse.ErrorMessage + " ";
                }
            }

            if (validationFlags.HasFlag(ReservationValidationFlags.NoConflictingReservations))
            {
                var conflictingReservationsResponse = await ValidateNoConflictingReservationsAsync(userReservationsModel);
                if (conflictingReservationsResponse.HasError)
                {
                    failedValidations.Add("NoConflictingReservations");
                }
            }

            if (failedValidations.Any())
            {
                response.HasError = true;
                response.ErrorMessage += $"Validation failed for {string.Join(", ", failedValidations)}.";
                return response;
            }

            
            response.HasError = false;
            response.ErrorMessage += "All requested validations passed.";
            return response;
        }

        // public Response CheckReservationFormatIsValid(UserReservationsModel userReservationsModel){

        //     Response response = new Response();

        //     response.HasError = false;
        //     response.ErrorMessage = "Reservation Format is Valid.";

        //     if (userReservationsModel == null)
        //     {
        //         return new Response
        //         {
        //             HasError = true,
        //             ErrorMessage = "The reservation model cannot be null."
        //         };
        //     }
            
        //     if (string.IsNullOrWhiteSpace(userReservationsModel.UserHash) || string.IsNullOrWhiteSpace(userReservationsModel.SpaceID))
        //     {
        //         return new Response
        //         {
        //             HasError = true,
        //             ErrorMessage = "Required fields are missing or in invalid format.";
        //         };
    
        //     }

        //     if (userReservationsModel.CompanyID <= 0 || userReservationsModel.FloorPlanID <= 0)
        //     {
        //         return new Response
        //         {
        //             HasError = true,
        //             ErrorMessage = "CompanyID and FloorPlanID must be positive numbers."
        //         };
        //     }

            
        //     if (userReservationsModel.ReservationStartTime < DateTime.UtcNow || userReservationsModel.ReservationEndTime <= userReservationsModel.ReservationStartTime)
        //     {
        //         return new Response
        //         {
        //             HasError = true,
        //             ErrorMessage = "Invalid date values. Dates cannot be in the past, and the end date must be after the start date."
        //         };
    
        //     }
        //     return response;
        // }

    }
}

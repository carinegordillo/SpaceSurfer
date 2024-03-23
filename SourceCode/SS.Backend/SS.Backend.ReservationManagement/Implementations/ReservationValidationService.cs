
using SS.Backend.SharedNamespace;
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

        public async Task<bool> HasConflictingReservations(Response result){
            

            if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0 )
            {
                //conflicts
                return true;
            }
            else
            {
                return false;
            }
    
            return true;
        }
        


        /** check if the reservtaion is within company hours**/
        public async Task<bool> IsWithinHours(Response result, TimeSpan proposedStart, TimeSpan proposedEnd){
            

                    
                TimeSpan openingHours = (TimeSpan)(result.ValuesRead.Rows[0]["openingHours"]); 
                TimeSpan closingHours = (TimeSpan)(result.ValuesRead.Rows[0]["closingHours"]);
                
                
                if (proposedStart >= openingHours && proposedEnd <= closingHours)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                    
                
            return false;
        }

        public async Task<bool> IsValidDuration(UserReservationsModel userReservationsModel, Response result){
        

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

        public async Task<bool> ValidMinimumDuration(UserReservationsModel userReservationsModel){
        

            var userReservationDuration = userReservationsModel.ReservationEndTime - userReservationsModel.ReservationStartTime;

                
            if (userReservationDuration.TotalMinutes >= 60)
            {
                    
                return true;
            }
            else
            {
                return false;
            }
                
            return false;
        }

        public async Task<bool>  IsValidReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime){
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

            var reservationDateTime = userReservationsModel.ReservationDate.Date + userReservationsModel.ReservationStartTime;

            if (reservationDateTime <= maxLeadDateTime)
            {
                return true;
                
            }
            else
            {
                return false;
            }
            
            return false;

        }

    }
}

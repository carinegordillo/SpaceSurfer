
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

        public bool HasConflictingReservations(Response result){
            

            if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0 )
            {
                
                return true;
            }
           
    
            return false;
        }
        


        /** check if the reservtaion is within company hours**/
        public bool IsWithinHours(Response result, DateTime proposedStart, DateTime proposedEnd){
            
                TimeSpan proposedStartTime = proposedStart.TimeOfDay;
                TimeSpan proposedEndTime = proposedEnd.TimeOfDay;
                    
                TimeSpan openingHours = (TimeSpan)(result.ValuesRead.Rows[0]["openingHours"]); 
                TimeSpan closingHours = (TimeSpan)(result.ValuesRead.Rows[0]["closingHours"]);
                
                
                if (proposedStartTime >= openingHours && proposedEndTime <= closingHours)
                {
                    return true;
                }
                    
                
            return false;
        }

        public bool IsValidDuration(UserReservationsModel userReservationsModel, Response result){
        

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

        public  bool IsValidMinimumDuration(UserReservationsModel userReservationsModel){
        

            var userReservationDuration = userReservationsModel.ReservationEndTime - userReservationsModel.ReservationStartTime;

                
            if (userReservationDuration.TotalMinutes >= 60)
            {
                    
                return true;
            }
            
                
            return false;
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

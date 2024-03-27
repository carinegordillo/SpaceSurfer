
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationCreatorService
    {
        public  Task<Response> CreateReservationWithAutoIDAsync(string tableName, UserReservationsModel userReservationsModel);
        public  Task<Response> CreateReservationWithManualIDAsync(string tableName, UserReservationsModel userReservationsModel);
        public  Task<Response> CheckConflictingReservationsAsync(int floorPlanID, string spaceID, TimeSpan proposedStart, TimeSpan proposedEnd);
        public  Task<Response> ValidateWithinHoursAsync(int companyID, TimeSpan proposedStart, TimeSpan proposedEnd);
        public  Task<Response> ValidateReservationDurationAsync(UserReservationsModel userReservationsModel);
        public  Response ValidateReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime);
    }
}
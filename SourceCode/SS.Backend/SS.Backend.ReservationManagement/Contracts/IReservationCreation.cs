
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationCreation
    {
        public  Task<Response> CreateReservationWithAutoID(string tableName, UserReservationsModel userReservationsModel);
        public  Task<Response> CreateReservationWithManualID(string tableName, UserReservationsModel userReservationsModel);
        public  Task<Response> CheckConflictingReservations(int floorPlanID, string spaceID, TimeSpan proposedStart, TimeSpan proposedEnd);
        public  Task<Response> ValidateWithinHours(int companyID, TimeSpan proposedStart, TimeSpan proposedEnd);
        public  Task<Response> ValidateReservationDuration(UserReservationsModel userReservationsModel);
        public  Task<Response> validateReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime);
    }
}
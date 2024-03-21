
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationCreationService
{
    public interface IReservationCreation
    {
        public  Task<Response> CreateReservation(string tableName, UserReservationsModel userReservationsModel);
        public  Task<Response> CheckConflictingReservations(int floorPlanID, string spaceID, TimeSpan proposedStart, TimeSpan proposedEnd);
        public  Task<Response> ValidateWithinHours(int companyID, TimeSpan proposedStart, TimeSpan proposedEnd);
    }
}
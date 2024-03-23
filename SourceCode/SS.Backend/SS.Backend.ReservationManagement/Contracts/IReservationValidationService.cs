
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationValidationService
    {
        public Task<bool> HasConflictingReservations(Response result);
        public  Task<bool> IsWithinHours(Response result, TimeSpan proposedStart, TimeSpan proposedEnd);
        public  Task<bool> IsValidDuration(UserReservationsModel userReservationsModel, Response result);
        public  Task<bool> ValidMinimumDuration(UserReservationsModel userReservationsModel);
        public  Task<bool>  IsValidReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime);
    }
}
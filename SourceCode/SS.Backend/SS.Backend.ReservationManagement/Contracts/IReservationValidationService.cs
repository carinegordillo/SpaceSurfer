
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationValidationService
    {
        public bool HasConflictingReservations(Response result);
        public bool IsWithinHours(Response result, TimeSpan proposedStart, TimeSpan proposedEnd);
        public bool IsValidDuration(UserReservationsModel userReservationsModel, Response result);
        public bool ValidMinimumDuration(UserReservationsModel userReservationsModel);
        public  bool  IsValidReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime);
    }
}

using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationValidationService
    {
        public bool HasConflictingReservations(Response result);
        public bool IsWithinHours(Response result, DateTime proposedStart, DateTime proposedEnd);
        public bool IsValidDuration(UserReservationsModel userReservationsModel, Response result);
        public bool IsValidMinimumDuration(UserReservationsModel userReservationsModel);
        public  bool  IsValidReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime);
    }
}
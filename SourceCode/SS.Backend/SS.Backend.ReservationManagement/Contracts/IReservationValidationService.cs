
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationValidationService
    {
        public Task<Response> ValidateNoConflictingReservationsAsync(UserReservationsModel userReservationsModel);
        public  Task<Response> ValidateWithinHoursAsync(UserReservationsModel userReservationsModel);
        public bool IsWithinHours(Response result, DateTime proposedStart, DateTime proposedEnd);
        public  Task<Response> ValidateReservationDurationAsync(UserReservationsModel userReservationsModel);
        public bool IsValidMaxDuration(UserReservationsModel userReservationsModel, Response result);
        public bool IsValidMinDuration(UserReservationsModel userReservationsModel);
        public  Response ValidateReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime);
        public  bool  IsValidReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime);

    } 
}
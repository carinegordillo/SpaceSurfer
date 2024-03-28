
using SS.Backend.SharedNamespace;

namespace SS.Backend.ReservationManagement
{
    public interface IReservationCreatorService
    {
        public  Task<Response> CreateReservationWithAutoIDAsync(string tableName, UserReservationsModel userReservationsModel);
        public  Task<Response> CreateReservationWithManualIDAsync(string tableName, UserReservationsModel userReservationsModel);
        public  Task<Response> CheckConflictingReservationsAsync(UserReservationsModel userReservationsModel);
        public  Task<Response> ValidateWithinHoursAsync(int companyID, UserReservationsModel userReservationsModel);
        public  Task<Response> ValidateReservationDurationAsync(UserReservationsModel userReservationsModel);
        public  Response ValidateReservationLeadTime(UserReservationsModel userReservationsModel, int maxLeadTime, TimeUnit unitOfTime);
    }
}
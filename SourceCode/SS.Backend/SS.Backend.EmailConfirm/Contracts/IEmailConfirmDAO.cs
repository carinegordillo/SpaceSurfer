using SS.Backend.SharedNamespace;

namespace SS.Backend.EmailConfirm
{
    public interface IEmailConfirmDAO
    {
        public Task<Response> GetReservationInfo(int reservationID);
        public Task<Response> GetConfirmInfo(int reservationID);
        public Task<Response> InsertConfirmStatus(int reservationID);
    }
}
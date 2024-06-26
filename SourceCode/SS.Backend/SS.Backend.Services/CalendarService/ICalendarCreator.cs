using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.CalendarService
{
    public interface ICalendarCreator
    {
        public Task<string> CreateCalendar(ReservationInfo reservationInfo);
    }
}
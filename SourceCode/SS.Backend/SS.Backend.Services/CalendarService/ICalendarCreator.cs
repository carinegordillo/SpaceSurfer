using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.CalendarService
{
    public interface ICalendarCreator
    {
        public void CreateCalendar(ReservationInfo reservationInfo);
    }
}
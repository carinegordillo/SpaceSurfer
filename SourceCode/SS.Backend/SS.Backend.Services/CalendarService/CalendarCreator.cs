//using SS.Backend.Services.LogginSerivces;
using SS.Backend.SharedNamespace;
using System;
using System.IO;
using System.Text;

namespace SS.Backend.Services.CalendarService
{
    public class CalendarCreator : ICalendarCreator
    {
        public void CreateCalendar(ReservationInfo reservationInfo)
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine("BEGIN: VCALENDAR");
            str.AppendLine("VERSION:2.0");
            str.AppendLine("PRODID: -//SpaceSurfer//Reservation Confirmation//EN");
            str.AppendLine("BEGIN:VEVENT");
            str.AppendLine($"UID:{Guid.NewGuid()}@gmail.com");
            str.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
            str.AppendLine($"DTSTART:{reservationInfo.start:yyyyMMddTHHmmssZ}");
            str.AppendLine($"DTEND:{reservationInfo.end:yyyyMMddTHHmmssZ}");
            str.AppendLine($"SUMMARY:{reservationInfo.eventName}");
            str.AppendLine($"DESCRIPTION:{reservationInfo.description}");
            str.AppendLine($"LOCATION:{reservationInfo.location}");
            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");

            File.WriteAllText(reservationInfo.filePath, str.ToString(), Encoding.UTF8);
        }

    }
}
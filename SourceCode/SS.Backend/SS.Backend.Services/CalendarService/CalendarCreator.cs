//using SS.Backend.Services.LogginSerivces;
using SS.Backend.SharedNamespace;
using System;
using System.IO;
using System.Text;

namespace SS.Backend.Services.CalendarService
{
    public class CalendarCreator : ICalendarCreator
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<string> CreateCalendar(ReservationInfo reservationInfo)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine("BEGIN:VCALENDAR");
            str.AppendLine("VERSION:2.0");
            str.AppendLine("PRODID:-//SpaceSurfer//Reservation Confirmation//EN");
            str.AppendLine("BEGIN:VEVENT");
            str.AppendLine($"UID:{Guid.NewGuid()}@gmail.com");
            str.AppendLine($"DTSTAMP:{reservationInfo.dateTime:yyyyMMddTHHmmssZ}");
            str.AppendLine($"DTSTART:{reservationInfo.start:yyyyMMddTHHmmssZ}");
            str.AppendLine($"DTEND:{reservationInfo.end:yyyyMMddTHHmmssZ}");
            str.AppendLine($"SUMMARY:{reservationInfo.eventName}");
            str.AppendLine($"DESCRIPTION:{reservationInfo.description}");
            str.AppendLine($"LOCATION:{reservationInfo.location}");
            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");

            Directory.CreateDirectory(Path.GetDirectoryName(reservationInfo.filePath));

            // using (var writer = new StreamWriter(reservationInfo.filePath, false, Encoding.UTF8))
            // {
            //     await writer.WriteAsync(str.ToString());
            // }

            await File.WriteAllTextAsync(reservationInfo.filePath, str.ToString());

            return reservationInfo.filePath;
        }
    }

}
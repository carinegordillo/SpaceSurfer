using SS.Backend.SharedNamespace;
using SS.Backend.SharedNamespace.GenOTP;
using SS.Backend.DataAccess;
using SS.Backend.Services.LogginSerivces;
using SS.Backend.Services.CalendarCreator;
using SS.Backend.Security;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace SS.Backend.EmailConfirm
{
    public class EmailConfirm : IEmailConfirm
    {
        private readonly IEmailConfirmDAO _emailDAO;

        public EmailConfirm(IEmailConfirmDAO emailDAO)
        {
            _emailDAO = emailDAO;
        }

        public async Task<(string ics, string otp, Response res)> CreateConfirmation(int reservationID)
        {
            var response = new Response();
            //DataRow reservationInput = reservationInfo.ValuesRead.Rows[0];
            var calendarFilePath = "SSReservation.ics";
            string otp = string.Empty;
            string icsFile = string.Empty;

            var infoResponse = await _emailDAO.GetReservationInfo(reservationID);
            if (!infoResponse.HasError && infoResponse.ValuesRead != null && infoResponse.Rows.Count > 0)
            {
                DataRow row = infoResponse.ValuesRead.Rows[0];

                var getOtp = new GenOTP();
                string otp = genOtp.generateOTP();


                var insertResponse = await _emailDAO.InsertConfirmationInfo(reservationID, otp);
                if (!insertResponse.HasError)
                {
                    //extract reservation info
                    var address = response.ValuesRead.Columns.Contains("address") ? row["address"].ToString() : null;
                    var spaceID = response.ValuesRead.Columns.Contains("spaceID") ? row["spaceID"].ToString() : null;
                    var floorID = response.ValuesRead.Columns.Contains("floorID") ? row["floorID"].ToString() : null;
                    //extract and handle reservation date and time 
                    var date = row.Table.Columns.Contains("reservationDate") ? Convert.ToDateTime(row["reservationDate"]) : (DateTime?)null;
                    var startTime = row.Table.Columns.Contains("reservationStartTime") ? (DateTime?)DateTime.Parse(date?.ToShortDateString() + " " + row["reservationStartTime"].ToString()) : null;
                    var endTime = row.Table.Columns.Contains("reservationEndTime") ? (DateTime?)DateTime.Parse(date?.ToShortDateString() + " " + row["reservationEndTime"].ToString()) : null;

                    /*
                    if (address == null) response.ErrorMessage = "The 'address' data was not found.";
                    if (spaceID == null) response.ErrorMessage = "The 'spaceID' data was not found.";
                    if (floorID == null) response.ErrorMessage = "The 'floorID' data was not found.";
                    if (date == null) response.ErrorMessage = "The 'reservationDate' data was not found.";
                    if (startTime == null) response.ErrorMessage = "The 'reservationStartTime' data was not found.";
                    if (endTime == null) response.ErrorMessage = "The 'reservationEndTime' data was not found.";
                    */

                    var reservationInfo = new ReservationInfo
                    {
                        filePath = "SSReservation.ics",
                        eventName = "SpaceSurfer Reservation",
                        dateTime = date,
                        start = startTime,
                        end = endTime,
                        description = $"ReservationID: {reservationID}, FloorID: {floorID}, SpaceID: {spaceID}",
                        location = address
                    };

                    // save these to database
                    var calendarCreator = new CalendarCreator();
                    icsFile = await calendarCreator.CreateCalendar(reservationInfo);

                }
                else
                {
                    response = insertResponse;
                }
            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = "Failed to retrieve reservation info or no data found.";
            }

            return (icsFile, otp, response);
        }
    }
}
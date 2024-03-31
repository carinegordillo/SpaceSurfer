using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
//using SS.Backend.Services.LoggingSerivces;
using SS.Backend.Services.CalendarService;
//using SS.Backend.Security;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;
using System.IO;

namespace SS.Backend.EmailConfirm
{
    public class EmailConfirmService : IEmailConfirmService
    {
        private readonly IEmailConfirmDAO _emailDAO;

        public EmailConfirmService(IEmailConfirmDAO emailDAO)
        {
            _emailDAO = emailDAO;
        }

        public async Task<(string ics, string otp, Response res)> CreateConfirmation(int reservationID)
        {
            var response = new Response();
            //DataRow reservationInput = reservationInfo.ValuesRead.Rows[0];
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var calendarFilePath = Path.Combine(projectRootDirectory, "CalendarFiles", "SSReservation.ics");
            string otp = string.Empty;
            string icsFile = string.Empty;

            var infoResponse = await _emailDAO.GetReservationInfo(reservationID);
            if (!infoResponse.HasError && infoResponse.ValuesRead != null && infoResponse.ValuesRead.Rows.Count > 0)
            {
                DataRow row = infoResponse.ValuesRead.Rows[0];

                var getOtp = new GenOTP();
                otp = getOtp.generateOTP();

                //extract reservation info
                int resID = infoResponse.ValuesRead.Columns.Contains("reservationID") && row["reservationID"] != DBNull.Value
                            ? Convert.ToInt32(row["reservationID"])
                            : -1; // or any other default value you choose

                var address = infoResponse.ValuesRead.Columns.Contains("CompanyAddress") ? row["CompanyAddress"].ToString() : null;
                var spaceID = infoResponse.ValuesRead.Columns.Contains("spaceID") ? row["spaceID"].ToString() : null;
                var companyName = infoResponse.ValuesRead.Columns.Contains("CompanyName") ? row["CompanyName"].ToString() : null;
                //extract and handle reservation date and time 
                var date = row.Table.Columns.Contains("reservationDate") ? Convert.ToDateTime(row["reservationDate"]) : (DateTime?)null;
                var startTime = row.Table.Columns.Contains("reservationStartTime") ? (DateTime?)DateTime.Parse(date?.ToShortDateString() + " " + row["reservationStartTime"].ToString()) : null;
                var endTime = row.Table.Columns.Contains("reservationEndTime") ? (DateTime?)DateTime.Parse(date?.ToShortDateString() + " " + row["reservationEndTime"].ToString()) : null;

                
                if (address == null) response.ErrorMessage = "The 'address' data was not found.";
                if (spaceID == null) response.ErrorMessage = "The 'spaceID' data was not found.";
                if (resID == null) response.ErrorMessage = "The 'reservationID' data was not found.";
                if (companyName == null) response.ErrorMessage = "The 'CompanyName' data was not found.";
                if (date == null) response.ErrorMessage = "The 'reservationDate' data was not found.";
                if (startTime == null) response.ErrorMessage = "The 'reservationStartTime' data was not found.";
                if (endTime == null) response.ErrorMessage = "The 'reservationEndTime' data was not found.";
                

                //calendar ics creator
                var reservationInfo = new ReservationInfo
                {
                    filePath = calendarFilePath,
                    eventName = "SpaceSurfer Reservation",
                    dateTime = date,
                    start = startTime,
                    end = endTime,
                    description = $"Reservation at: {companyName} \nReservationID: {reservationID} \nSpaceID: {spaceID}",
                    location = address
                };
                var calendarCreator = new CalendarCreator();
                icsFile = await calendarCreator.CreateCalendar(reservationInfo);
                byte[] fileBytes = await File.ReadAllBytesAsync(calendarFilePath);
                
                // insert to db table
                var insertResponse = await _emailDAO.InsertConfirmationInfo(resID, otp, fileBytes);
                if (insertResponse.HasError || insertResponse.ValuesRead == null || insertResponse.ValuesRead.Rows.Count == 0)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Failed to insert confirmation info into database.";
                }

            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = "An error occured during reservation confirmation creation. Please try again later.";
            }

            return (icsFile, otp, response);
        }

        public async Task<(string ics, string otp, Response res)> ResendConfirmation(int reservationID)
        {
            var response = new Response();
            //DataRow reservationInput = reservationInfo.ValuesRead.Rows[0];
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var calendarFilePath = Path.Combine(projectRootDirectory, "CalendarFiles", "SSReservation.ics");
            string otp = string.Empty;
            string icsFile = string.Empty;
            
            var statusResponse = await _emailDAO.GetConfirmInfo(reservationID);
            if (!statusResponse.HasError && statusResponse.ValuesRead != null && statusResponse.ValuesRead.Rows.Count > 0)
            {
                DataRow statusRow = statusResponse.ValuesRead.Rows[0];
                var reservationOtp = statusResponse.ValuesRead.Columns.Contains("reservationOTP") ? statusRow["reservationOTP"].ToString() : null;
                var confirmStatus = statusResponse.ValuesRead.Columns.Contains("confirmStatus") ? statusRow["confirmStatus"].ToString() : null;
                if (confirmStatus == "yes") 
                {
                    response.ErrorMessage = "Unable to send confirmation Email. Reservation is already confirmed.";
                    return (icsFile, otp, response);
                }

                var infoResponse = await _emailDAO.GetReservationInfo(reservationID);
                if (!infoResponse.HasError && infoResponse.ValuesRead != null && infoResponse.ValuesRead.Rows.Count > 0)
                {
                    DataRow row = infoResponse.ValuesRead.Rows[0];

                    var getOtp = new GenOTP();
                    otp = getOtp.generateOTP();

                    //extract reservation info
                    var companyName = infoResponse.ValuesRead.Columns.Contains("companyName") ? row["companyName"].ToString() : null;
                    var address = infoResponse.ValuesRead.Columns.Contains("address") ? row["address"].ToString() : null;
                    var spaceID = infoResponse.ValuesRead.Columns.Contains("spaceID") ? row["spaceID"].ToString() : null;
                    //extract and handle reservation date and time 
                    var date = row.Table.Columns.Contains("reservationDate") ? Convert.ToDateTime(row["reservationDate"]) : (DateTime?)null;
                    var startTime = row.Table.Columns.Contains("reservationStartTime") ? (DateTime?)DateTime.Parse(date?.ToShortDateString() + " " + row["reservationStartTime"].ToString()) : null;
                    var endTime = row.Table.Columns.Contains("reservationEndTime") ? (DateTime?)DateTime.Parse(date?.ToShortDateString() + " " + row["reservationEndTime"].ToString()) : null;

                    
                    if (address == null) response.ErrorMessage = "The 'address' data was not found.";
                    if (spaceID == null) response.ErrorMessage = "The 'spaceID' data was not found.";
                    if (companyName == null) response.ErrorMessage = "The 'companyName' data was not found.";
                    if (date == null) response.ErrorMessage = "The 'reservationDate' data was not found.";
                    if (startTime == null) response.ErrorMessage = "The 'reservationStartTime' data was not found.";
                    if (endTime == null) response.ErrorMessage = "The 'reservationEndTime' data was not found.";
                    

                    //calendar ics creator
                    var reservationInfo = new ReservationInfo
                    {
                        filePath = calendarFilePath,
                        eventName = "SpaceSurfer Reservation",
                        dateTime = date,
                        start = startTime,
                        end = endTime,
                        description = $"Reservation at: {companyName} \nReservationID: {reservationID} \nSpaceID: {spaceID}",
                        location = address
                    };
                    var calendarCreator = new CalendarCreator();
                    icsFile = await calendarCreator.CreateCalendar(reservationInfo);
                    byte[] fileBytes = await File.ReadAllBytesAsync(calendarFilePath);
                    
                    // if resending, update otp
                    if (reservationOtp != otp)
                    {
                        await _emailDAO.UpdateOtp(reservationID, otp);
                    }

                }
                else
                {
                    response.HasError = true;
                    response.ErrorMessage = infoResponse.ErrorMessage;
                }
            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = "Failed to check confirmation status.";
            }

            return (icsFile, otp, response);
        }

        public async Task<Response> ConfirmReservation(int reservationID, string otp)
        {
            var response = await _emailDAO.GetConfirmInfo(reservationID);
            if (!response.HasError && response.ValuesRead != null && response.ValuesRead.Rows.Count > 0)
            {
                DataRow row = response.ValuesRead.Rows[0];
                var reservationOtp = response.ValuesRead.Columns.Contains("reservationOTP") ? row["reservationOTP"].ToString() : null;
                if (otp == reservationOtp)
                {
                    var updateResponse = await _emailDAO.UpdateConfirmStatus(reservationID);
                    if (updateResponse.HasError)
                    {
                        response.HasError = true;
                        response.ErrorMessage = "Failed to update confirmation status.";
                    }
                }
                else
                {
                    response.HasError = true;
                    response.ErrorMessage = "OTP does not match. Please try again.";
                }
            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = "Failed to retrieve reservation confirmation info. Please try again later.";
            }
            
            return response;
        }
    }
}
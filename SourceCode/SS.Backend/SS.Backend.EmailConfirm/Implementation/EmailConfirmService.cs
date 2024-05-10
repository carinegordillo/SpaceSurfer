using SS.Backend.SharedNamespace;
using SS.Backend.Services.CalendarService;
using System.Data;
using System;
using System.IO;
using System.Text;
using SS.Backend.Services.LoggingService;
using System.Diagnostics.CodeAnalysis;


namespace SS.Backend.EmailConfirm
{
    public class EmailConfirmService : IEmailConfirmService
    {
        private readonly IEmailConfirmDAO _emailDAO;
        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();
        private LogEntry logEntry;

        public EmailConfirmService(IEmailConfirmDAO emailDAO, ILogger logger)
        {
            _emailDAO = emailDAO;
            _logger = logger;
            logEntry = logBuilder.Build();
        }

        public async Task<(string ics, string otp, string body, Response res)> CreateConfirmation(int reservationID)
        {
            var response = new Response();
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../"));
            var icsFilePath = Path.Combine(projectRootDirectory, "CalendarFiles", "SSReservation.ics");
            Response infoResponse = await _emailDAO.GetReservationInfo(reservationID);
            string otp;
            string htmlBody;
            if (!infoResponse.HasError && infoResponse.ValuesRead != null && infoResponse.ValuesRead.Rows.Count > 0)
            {
                Console.WriteLine(infoResponse.ErrorMessage);
                DataRow row = infoResponse.ValuesRead.Rows[0];
                StringBuilder rowAsString = new StringBuilder();

                foreach (DataColumn column in row.Table.Columns)
                {
                    rowAsString.Append(row[column].ToString());
                    rowAsString.Append(" ");
                }

                Console.WriteLine(rowAsString.ToString());

                var resID = Convert.ToInt32(row["ReservationID"]);

                if (resID == reservationID)
                {
                    var getOtp = new GenOTP();
                    otp = getOtp.generateOTP();
        
                    var location = infoResponse.ValuesRead.Rows[0]["CompanyAddress"].ToString();
                    Console.WriteLine($"reservation ID: {reservationID}");
                    var spaceID = infoResponse.ValuesRead.Rows[0]["spaceID"].ToString();
                    var companyName = infoResponse.ValuesRead.Rows[0]["CompanyName"].ToString();
                    //extract and handle reservation date and time 
                    DateTime? start = null;
                    if (row.Table.Columns.Contains("reservationStartTime") && row["reservationStartTime"] != DBNull.Value)
                    {
                        start = DateTime.Parse(row["reservationStartTime"].ToString());
                    }
                    DateTime? end = null;
                    if (row.Table.Columns.Contains("reservationEndTime") && row["reservationEndTime"] != DBNull.Value)
                    {
                        end = DateTime.Parse(row["reservationEndTime"].ToString());
                    }
                    DateTime? date = start.Value;

                    if (location == null) response.ErrorMessage = "The 'address' data was not found.";
                    if (spaceID == null) response.ErrorMessage = "The 'spaceID' data was not found.";
                    //if (resID == null) response.ErrorMessage = "The 'reservationID' data was not found.";
                    if (companyName == null) response.ErrorMessage = "The 'CompanyName' data was not found.";
                    if (date == null) response.ErrorMessage = "The 'reservationDate' data was not found.";
                    if (start == null) response.ErrorMessage = "The 'reservationStartTime' data was not found.";
                    if (end == null) response.ErrorMessage = "The 'reservationEndTime' data was not found.";


                    //calendar ics creator
#pragma warning disable CS8601 // Possible null reference assignment.
                    var reservationInfo = new ReservationInfo
                    {
                        filePath = icsFilePath,
                        eventName = "SpaceSurfer Reservation",
                        dateTime = date,
                        start = start,
                        end = end,
                        description = $"Reservation at: {companyName} ReservationID: {reservationID} SpaceID: {spaceID}",
                        location = location
                    };
#pragma warning restore CS8601 // Possible null reference assignment.
                    var calendarCreator = new CalendarCreator();
                    icsFilePath = await calendarCreator.CreateCalendar(reservationInfo);
                    Console.WriteLine(icsFilePath);
                    byte[]? fileBytes = await File.ReadAllBytesAsync(icsFilePath);
                    if (string.IsNullOrEmpty(icsFilePath))
                    {
                        // Handle the case where ICS file generation failed
                        response.HasError = true;
                        response.ErrorMessage += "Failed to generate the ICS file.";
                    }
                    // insert to db table
                    response = await _emailDAO.InsertConfirmationInfo(reservationID, otp, fileBytes);
                    if (response.HasError)
                    {
                        response.HasError = true;
                        response.ErrorMessage += "Error inserting confirmation info into database.";
                    }

                    // create the email body with reservation details
                    htmlBody = @"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <title>Reservation Confirmation</title>
                    </head>
                    <body>
                        <p>Hello!</p>
                        <p>Thank you for reserving a space at SpaceSurfer! Here are the details of your reservation:</p>
                        <ul>
                            <li>Reservation ID: <strong>{reservationID}</strong></li>
                            <li>Reservation at: <strong>{companyName}</strong></li>
                            <li>Location: <strong>{address}</strong></li>
                            <li>SpaceID: <strong>{spaceID}</strong></li>
                            <li>Date: <strong>{date}</strong></li>
                            <li>Start Time: <strong>{startTime}</strong></li>
                            <li>End Time: <strong>{endTime}</strong></li>
                        </ul>
                        <p>To confirm your reservation, head over to SpaceSurfer --&gt; Space Booking Center --&gt; Active Reservations, and confirm your Reservation with this code:</p>
                        <p><strong>{otp}</strong></p>
                        <p>Best,<br>PixelPals</p>
                    </body>
                    </html>";

                    string? dateString = reservationInfo.dateTime?.ToString("MMMM d, yyyy");
                    string? startTimeString = reservationInfo.start?.ToString("h:mm tt");
                    string? endTimeString = reservationInfo.end?.ToString("h:mm tt"); 

                    htmlBody = htmlBody.Replace("{companyName}", companyName)
                                        .Replace("{reservationID}", Convert.ToString(reservationID))
                                        .Replace("{address}", reservationInfo.location)
                                        .Replace("{spaceID}", spaceID)
                                        .Replace("{date}", dateString)
                                        .Replace("{startTime}", startTimeString)
                                        .Replace("{endTime}", endTimeString)
                                        .Replace("{otp}", otp);
                }
                else
                {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                    return (null, null, null, new Response { HasError = true, ErrorMessage = "ReservationID not found in database." });
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                }
            }
            else
            {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return (null, null, null, new Response { HasError = true, ErrorMessage = "Failed to retrieve reservation info." });
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                              // response.HasError = true;
                              // response.ErrorMessage += "An error occured during reservation confirmation creation. Please try again later.";
            }

            return (icsFilePath, otp, htmlBody,response);
        }

        public async Task<(string ics, string otp, string body, Response res)> ResendConfirmation(int reservationID)
        {
            var response = new Response();
            //DataRow reservationInput = reservationInfo.ValuesRead.Rows[0];
            var reservationinfo = new ReservationInfo();
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../"));
            reservationinfo.filePath = Path.Combine(projectRootDirectory, "CalendarFiles", "SSReservation.ics");
            string newOtp = string.Empty;
            string icsFile = string.Empty;
            string htmlBody = string.Empty;
            
            // check confirmation status
            var statusResponse = await _emailDAO.GetConfirmInfo(reservationID);
            if (!statusResponse.HasError && statusResponse.ValuesRead != null && statusResponse.ValuesRead.Rows.Count > 0)
            {
                Console.WriteLine(statusResponse.ErrorMessage);
                DataRow statusRow = statusResponse.ValuesRead.Rows[0];
                StringBuilder rowAsString = new StringBuilder();

                foreach (DataColumn column in statusRow.Table.Columns)
                {
                    rowAsString.Append(statusRow[column].ToString());
                    rowAsString.Append(" "); // Add a space or another delimiter if needed
                }

                // Now you can print or use rowAsString.ToString()
                Console.WriteLine(rowAsString.ToString());
                //DataRow statusRow = statusResponse.ValuesRead.Rows[0];
                var reservationOtp = statusResponse.ValuesRead.Rows[0]["reservationOTP"].ToString();
                string? confirmStatus = statusResponse.ValuesRead.Rows[0]["confirmStatus"].ToString();
                Console.WriteLine($"Confirmation Status: {confirmStatus}");
                if (reservationOtp == null) response.ErrorMessage += "The 'reservationOtp' data was not found.";
                if (confirmStatus == null) response.ErrorMessage += "The 'confirmStatus' data was not found.";

                if (confirmStatus.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase)) 
                {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                    return (null, null, null, new Response { HasError = true, ErrorMessage = "Unable to send confirmation Email. Reservation is already confirmed." });
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                }
                else
                {
                //get reservation data
                    var infoResponse = await _emailDAO.GetReservationInfo(reservationID);
                    if (!infoResponse.HasError && infoResponse.ValuesRead != null && infoResponse.ValuesRead.Rows.Count > 0)
                    {
                        DataRow row = infoResponse.ValuesRead.Rows[0];

                        var resID = Convert.ToInt32(row["ReservationID"]);

                        if (resID == reservationID)
                        {
                            var getOtp = new GenOTP();
                            newOtp = getOtp.generateOTP();

#pragma warning disable CS8601 // Possible null reference assignment.
                            reservationinfo.location = infoResponse.ValuesRead.Columns.Contains("CompanyAddress") ? row["CompanyAddress"].ToString() : null;
#pragma warning restore CS8601 // Possible null reference assignment.
                            var spaceID = infoResponse.ValuesRead.Columns.Contains("spaceID") ? row["spaceID"].ToString() : null;
                            var companyName = infoResponse.ValuesRead.Columns.Contains("CompanyName") ? row["CompanyName"].ToString() : null;
                            //extract and handle reservation date and time 
                            reservationinfo.start = null;
                            if (row.Table.Columns.Contains("reservationStartTime") && row["reservationStartTime"] != DBNull.Value)
                            {
                                reservationinfo.start = DateTime.Parse(row["reservationStartTime"].ToString());
                            }
                            reservationinfo.end  = null;
                            if (row.Table.Columns.Contains("reservationEndTime") && row["reservationEndTime"] != DBNull.Value)
                            {
                                reservationinfo.end = DateTime.Parse(row["reservationEndTime"].ToString());
                            }
                            reservationinfo.dateTime = reservationinfo.start.Value;

                            if (reservationinfo.location == null) response.ErrorMessage = "The 'address' data was not found.";
                            if (spaceID == null) response.ErrorMessage = "The 'spaceID' data was not found.";
                            //if (resID == null) response.ErrorMessage = "The 'reservationID' data was not found.";
                            if (companyName == null) response.ErrorMessage = "The 'CompanyName' data was not found.";
                            if (reservationinfo.dateTime == null) response.ErrorMessage = "The 'reservationDate' data was not found.";
                            if (reservationinfo.start == null) response.ErrorMessage = "The 'reservationStartTime' data was not found.";
                            if (reservationinfo.end == null) response.ErrorMessage = "The 'reservationEndTime' data was not found.";
                            
                            reservationinfo.eventName = "SpaceSurfer Reservation";
                            reservationinfo.description = $"Reservation at: {companyName} ReservationID: {reservationID} SpaceID: {spaceID}";
                            
                            //calendar ics creator
                            var calendarCreator = new CalendarCreator();
                            icsFile = await calendarCreator.CreateCalendar(reservationinfo);
                            byte[]? fileBytes = await File.ReadAllBytesAsync(icsFile);
                            // if resending, update otp
                            if (reservationOtp != newOtp)
                            {
                                var otpResponse = await _emailDAO.UpdateOtp(reservationID, newOtp);
                                if (!otpResponse.HasError)
                                {
                                    response.HasError = false;
                                    response.ErrorMessage += $"Successfully updated reservation otp. {otpResponse.HasError}";
                                }
                            }
                            else
                            {
                                response.ErrorMessage += "Failed to generate new reservation otp.";
                            }
                            
                            // create the email body with reservation details
                            htmlBody = @"
                            <!DOCTYPE html>
                            <html lang='en'>
                            <head>
                                <meta charset='UTF-8'>
                                <title>Reservation Confirmation</title>
                            </head>
                            <body>
                                <p>Hello!</p>
                                <p>Thank you for reserving a space at SpaceSurfer! Here are the details of your reservation:</p>
                                <ul>
                                    <li>Reservation ID: <strong>{reservationID}</strong></li>
                                    <li>Reservation at: <strong>{companyName}</strong></li>
                                    <li>Location: <strong>{address}</strong></li>
                                    <li>SpaceID: <strong>{spaceID}</strong></li>
                                    <li>Date: <strong>{date}</strong></li>
                                    <li>Start Time: <strong>{startTime}</strong></li>
                                    <li>End Time: <strong>{endTime}</strong></li>
                                </ul>
                                <p>To confirm your reservation, head over to SpaceSurfer --&gt; Space Booking Center --&gt; Active Reservations, and confirm your Reservation with this code:</p>
                                <p><strong>{otp}</strong></p>
                                <p>Best,<br>PixelPals</p>
                            </body>
                            </html>";

                            string? dateString = reservationinfo.dateTime?.ToString("MMMM d, yyyy");
                            string? startTimeString = reservationinfo.start?.ToString("h:mm tt");
                            string? endTimeString = reservationinfo.end?.ToString("h:mm tt"); 

                            htmlBody = htmlBody.Replace("{companyName}", companyName)
                                                .Replace("{reservationID}", Convert.ToString(reservationID))
                                                .Replace("{address}", reservationinfo.location)
                                                .Replace("{spaceID}", spaceID)
                                                .Replace("{date}", dateString)
                                                .Replace("{startTime}", startTimeString)
                                                .Replace("{endTime}", endTimeString)
                                                .Replace("{otp}", newOtp);
                        }
                        else
                        {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                            return (null, null, null, new Response { HasError = true, ErrorMessage = "ReservationID not found in database." });
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                        }
                    }
                    else
                    {
                        response.HasError = true;
                        response.ErrorMessage = infoResponse.ErrorMessage + "Failed to get reservation data.";
                    }
                }
            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = "Failed to check confirmation status.";
            }

            return (icsFile, newOtp, htmlBody, response);
        }

        public async Task<Response> ConfirmReservation(int reservationID, string otp)
        {
            var response = await _emailDAO.GetConfirmInfo(reservationID);
            string? username = null;
            
            if (!response.HasError && response.ValuesRead != null && response.ValuesRead.Rows.Count > 0)
            {
                var reservation = await _emailDAO.GetReservationInfo(reservationID);
                username = reservation.ValuesRead.Rows[0]["userHash"].ToString();
                DataRow statusRow = response.ValuesRead.Rows[0];
                var reservationOtp = statusRow["reservationOTP"].ToString();
                var confirmStatus = statusRow["confirmStatus"].ToString();

                if (string.IsNullOrEmpty(reservationOtp) || string.IsNullOrEmpty(confirmStatus))
                {
                    return new Response
                    {
                        HasError = true,
                        ErrorMessage = "The reservation data is incomplete."
                    };
                }

                if (confirmStatus.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase)) 
                {
                    response.HasError = true;
                    response.ErrorMessage += "Unable to confirm. Reservation is already confirmed.";
                    return response;
                }
                else
                {
                    var resID = Convert.ToInt32(statusRow["ReservationID"]);

                    if (resID == reservationID)
                    {
                        if (otp == reservationOtp)
                        {
                            var updateResponse = await _emailDAO.UpdateConfirmStatus(reservationID);
                            if (updateResponse.HasError)
                            {
                                response.HasError = true;
                                response.ErrorMessage = "Failed to update confirmation status.";
                                return response;
                            }
                        }
                        else
                        {
                            response.HasError = true;
                            response.ErrorMessage = "OTP does not match. Please try again.";
                            return response;
                        }
                    }
                    else
                    {
                        response.HasError = true;
                        response.ErrorMessage = "ReservationID not found in database.";
                        return response;
                    }
                }
            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = "Failed to retrieve reservation confirmation info. Please try again later.";
                return response;
            }
            response.HasError = false;
            response.ErrorMessage = "Reservation confirmed successfully.";

            //logging
            if (response.HasError == false)
            {
                logEntry = logBuilder.Info().DataStore().Description($"Reservation {reservationID} confirmed successfully.").User(username).Build();
            }
            else
            {
                logEntry = logBuilder.Error().DataStore().Description($"Failed to confirm reservation {reservationID}.").User(username).Build();  
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            return response;
        }
    }
}
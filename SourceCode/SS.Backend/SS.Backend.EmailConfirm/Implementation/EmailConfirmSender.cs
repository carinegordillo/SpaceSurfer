using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.Services.EmailService;
using System.Net.Mail;
using MailKit.Security;
using SS.Backend.Services.LoggingService;



namespace SS.Backend.EmailConfirm
{
    public class EmailConfirmSender : IEmailConfirmSender
    {
        private readonly IEmailConfirmService _emailConfirm;
        private readonly IEmailConfirmDAO _emailDao;
        private readonly ILogger _logger;

        public EmailConfirmSender(IEmailConfirmService emailConfirm, IEmailConfirmDAO emailDao, ILogger logger)
        {
            _emailConfirm = emailConfirm;
            _emailDao = emailDao;
            _logger = logger;
        }

        public async Task<Response> SendConfirmation (UserReservationsModel reservation)
        {
            int reservationID = (int)reservation.ReservationID;
            Console.WriteLine(reservationID);
            var logResponse = new Response();
            Response emailResponse = await _emailDao.GetUsername(reservation.UserHash);
            string? targetEmail = emailResponse.ValuesRead.Rows[0]["username"].ToString();
            (string icsFile, string otp, string body, Response result) = await _emailConfirm.CreateConfirmation(reservationID);

            if (string.IsNullOrEmpty(body))
            {
                result.HasError = true;
                result.ErrorMessage = "Failed to create email confirmation. Body is null";
            }
            if (string.IsNullOrEmpty(icsFile))
            {
                result.HasError = true;
                result.ErrorMessage = "Failed to create email confirmation. Ics is null";
            }
            if (string.IsNullOrEmpty(otp))
            {
                result.HasError = true;
                result.ErrorMessage = "Failed to create email confirmation. Otp is null";
            }
            if (result.HasError)
            {
                result.HasError = true;
                result.ErrorMessage += "Failed to create email confirmation.";
            }
            try
            {
                await MailSender.SendConfirmEmail(targetEmail, icsFile, body);
            }
            catch (SmtpException ex)
            {
                result.ErrorMessage = ex.Message;
            }
            catch (IOException ex)
            {
                result.ErrorMessage = ex.Message;
            }
            catch (AuthenticationException ex)
            {
                result.ErrorMessage = ex.Message;
            }
            catch (Exception ex) // Catch any other unexpected exceptions
            {
                result.ErrorMessage = ex.Message;
            }

            //logging
            if (result.HasError == false)
            {
                LogEntry entry = new LogEntry
                {
                    timestamp = DateTime.UtcNow,
                    level = "Info",
                    username = reservation.UserHash,
                    category = "Data Store",
                    description = "Confirmation email sent successfully."
                };
                await _logger.SaveData(entry);
            }
            else
            {
                LogEntry entry = new LogEntry
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = reservation.UserHash,
                    category = "Data Store",
                    description = "Confirmation email failed."
                };
                await _logger.SaveData(entry);
            }
            return result;
        }

        public async Task<Response> ResendEmail (UserReservationsModel reservation)
        {

            int reservationID = (int)reservation.ReservationID;
            //string targetEmail = reservation.UserHash;
            Response emailResponse = await _emailDao.GetUsername(reservation.UserHash);
            string? targetEmail = emailResponse.ValuesRead.Rows[0]["username"].ToString();
            (string icsFile, string otp, string body, Response result) = await _emailConfirm.ResendConfirmation(reservationID);
            if (string.IsNullOrEmpty(body))
            {
                result.HasError = true;
                result.ErrorMessage = "Failed to create email confirmation. Body is null";
            }
            if (string.IsNullOrEmpty(icsFile))
            {
                result.HasError = true;
                result.ErrorMessage = "Failed to create email confirmation. Ics is null";
            }
            if (string.IsNullOrEmpty(otp))
            {
                result.HasError = true;
                result.ErrorMessage = "Failed to create email confirmation. Otp is null";
            }
            if (result.HasError)
            {
                result.HasError = true;
                result.ErrorMessage = "Failed to create email confirmation.";
            }
            try
            {
                await MailSender.SendConfirmEmail(targetEmail, icsFile, body);
            }
            catch (SmtpException ex)
            {
                result.ErrorMessage = ex.Message;
            }
            catch (IOException ex)
            {
                result.ErrorMessage = ex.Message;
            }
            catch (AuthenticationException ex)
            {
                result.ErrorMessage = ex.Message;
            }
            catch (Exception ex) // Catch any other unexpected exceptions
            {
                result.ErrorMessage = ex.Message;
            }

            //logging
            if (result.HasError == false)
            {
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Info",
                    username = reservation.UserHash,
                    category = "Data Store",
                    description = "Confirmation email resent successfully."
                };
                await _logger.SaveData(entry);
            }
            else
            {
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = reservation.UserHash,
                    category = "Data Store",
                    description = "Resending confirmation email failed."
                };
                await _logger.SaveData(entry);
            }
            return result;
        }
    }
}
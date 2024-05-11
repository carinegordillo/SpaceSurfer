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
        private LogEntryBuilder logBuilder = new LogEntryBuilder();
        private LogEntry logEntry;

        public EmailConfirmSender(IEmailConfirmService emailConfirm, IEmailConfirmDAO emailDao, ILogger logger)
        {
            _emailConfirm = emailConfirm;
            _emailDao = emailDao;
            _logger = logger;
            logEntry = logBuilder.Build();
        }

        public async Task<Response> SendConfirmation (UserReservationsModel reservation)
        {
            int reservationID = (int)reservation.ReservationID;
            Console.WriteLine(reservationID);
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
                logEntry = logBuilder.Info().DataStore().Description($"Confirmation email sent successfully.").User(reservation.UserHash).Build();
            }
            else
            {
                logEntry = logBuilder.Error().DataStore().Description($"Confirmation email failed to send.").User(reservation.UserHash).Build();
                
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
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
                logEntry = logBuilder.Info().DataStore().Description($"Confirmation email resent successfully.").User(reservation.UserHash).Build();
            }
            else
            {
                logEntry = logBuilder.Error().DataStore().Description($"Confirmation email failed to resend.").User(reservation.UserHash).Build();
                
            }
            if (logEntry != null && _logger != null)
            {
                _logger.SaveData(logEntry);
            }
            return result;
        }
    }
}
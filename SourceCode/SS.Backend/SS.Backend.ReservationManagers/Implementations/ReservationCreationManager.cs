using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.EmailConfirm;
using SS.Backend.Services.EmailService;
using System.Net.Mail;
using MailKit.Security;

namespace SS.Backend.ReservationManagers{

    public class ReservationCreationManager : IReservationCreationManager
    {
        private readonly string SS_RESERVATIONS_TABLE = "dbo.reservations";
        private readonly IReservationCreatorService _reservationCreatorService;
        private readonly IReservationValidationService _reservationValidationService;
        private readonly IEmailConfirmDAO _emailDao;
        private readonly IEmailConfirmService _emailService;
        //private readonly IEmailConfirmSender _emailSender;

        private readonly IReservationRequirements _reservationRequirements = new SpaceSurferReservationRequirements();
        
    

        public ReservationCreationManager(IReservationCreatorService reservationCreatorService, 
                                            IReservationValidationService reservationValidationService, 
                                            //IEmailConfirmSender emailSender,
                                            IEmailConfirmService emailService,
                                            IEmailConfirmDAO emailDao)
        {
            _reservationCreatorService = reservationCreatorService;
            _reservationValidationService = reservationValidationService;
            _emailService = emailService;
            //_emailSender = emailSender;
            _emailDao = emailDao;
            
        }

        public async Task<Response> CreateSpaceSurferSpaceReservationAsync(UserReservationsModel userReservationsModel, string? tableNameOverride = null)
        {
            Response response = new Response();
            Response reservationCreationResponse = new Response();
            Response emailResponse = new Response();

            string tableName = tableNameOverride ?? SS_RESERVATIONS_TABLE;
                
            ReservationValidationFlags flags = ReservationValidationFlags.CheckBusinessHours | ReservationValidationFlags.MaxDurationPerSeat | ReservationValidationFlags.ReservationLeadTime | ReservationValidationFlags.NoConflictingReservations | ReservationValidationFlags.CheckReservationFormatIsValid;
        
            Response validationResponse = await _reservationValidationService.ValidateReservationAsync(userReservationsModel, flags, _reservationRequirements);
    
            if (validationResponse.HasError)
            {
                response.ErrorMessage += "Reservation did not pass validation checks: " + validationResponse.ErrorMessage;
                response.HasError = true;
            }

            else
            {
                try
                {
                    reservationCreationResponse =  await _reservationCreatorService.CreateReservationWithAutoIDAsync(tableName, userReservationsModel);
                    //emailResponse = await _emailSender.SendConfirmation(userReservationsModel);
                    if (reservationCreationResponse.HasError)
                    {
                        response.ErrorMessage = "Could not create Reservation.";
                        response.HasError = true;
                    }
                    else
                    {
                        response.ErrorMessage = "Reservation created successfully!";
                        response.HasError = false;
                    }
                    // if(emailResponse.HasError)
                    // {
                    //     response.ErrorMessage += "Could not send confirmation email.";
                    //     Console.WriteLine("Could not send confirmation email.");
                    // }
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.ErrorMessage = ex.Message;
                }

            }
            return response;
        }

        public async Task<Response> SendConfirmation (UserReservationsModel reservation)
        {
            int reservationID = (int)reservation.ReservationID;
            Console.WriteLine(reservationID);
            var logResponse = new Response();
            Response emailResponse = await _emailDao.GetUsername(reservation.UserHash);
            string targetEmail = emailResponse.ValuesRead.Rows[0]["username"].ToString();
            (string icsFile, string otp, string body, Response result) = await _emailService.CreateConfirmation(reservationID);

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
                //await _logger.SaveData(entry);
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
                //await _logger.SaveData(entry);
            }
            return result;
        }


    }
}

    
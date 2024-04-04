// using MailKit.Security;
// using Microsoft.AspNetCore.Mvc;
// using SS.Backend.Services.EmailService;
// using System.Net.Mail;
// using SS.Backend.EmailConfirm;

// namespace SecurityAPI.Controllers
// {
//     [ApiController]
//     [Route("api/mail")]
//     public class MailController : Controller
//     {
//         private readonly EmailConfirmService _emailConfirm;

//         [HttpPost]
//         public async Task<IActionResult> Send()
//         {
//             (string ics, string otp, var result) = await _emailConfirm.CreateConfirmation(6);
//             string targetEmail = "4sarahsantos@gmail.com";
//             string subject = "Testing Email Confirmation";
//             string msg = $"Hello Sarah,\n\nThis is a test email sent from SpaceSurfers! \nReservation: {ics} \nConfirmation Otp: {otp} \n\nBest,\nPixelPals";

//             try
//             {
//                 await MailSender.SendEmail(targetEmail, subject, msg);
//                 Console.WriteLine("Successfully sent email.");
//                 return Ok("Success");
//             }
//             catch (SmtpException ex)
//             {
//                 Console.WriteLine("SMTP error: " + ex.Message);
//                 return StatusCode(500, "SMTP error: " + ex.Message);
//             }
//             catch (IOException ex)
//             {
//                 Console.WriteLine("IO error: " + ex.Message);
//                 return StatusCode(500, "IO error: " + ex.Message);
//             }
//             catch (AuthenticationException ex)
//             {
//                 Console.WriteLine("Authentication error: " + ex.Message);
//                 return StatusCode(500, "Authentication error: " + ex.Message);
//             }
//             catch (Exception ex) // Catch any other unexpected exceptions
//             {
//                 Console.WriteLine("Error sending email: " + ex.Message);
//                 return StatusCode(500, "Error sending email: " + ex.Message);
//             }
//         }
//     }
// }

using System.Net.Mail;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SS.Backend.Services.EmailService;
using SS.Backend.EmailConfirm;
using SS.Backend.SharedNamespace;

// Assuming MailSender is using MailKit for sending emails
using MailKit.Net.Smtp;
using System.IO;
using SecurityAPI.Models;
using SS.Backend.ReservationManagement;

namespace SecurityAPI.Controllers
{
    [ApiController]
    [Route("api/mail")]
    public class MailController : ControllerBase
    {
        private readonly ILogger<MailController> _logger;
        private readonly IEmailConfirmSender _emailConfirm;

        public MailController(IEmailConfirmSender emailConfirm, ILogger<MailController> logger)
        {
            _emailConfirm = emailConfirm ?? throw new ArgumentNullException(nameof(emailConfirm));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> Send()
        {
            // if (!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
            // }
            // string targetEmail = request.TargetEmail;

            // if (string.IsNullOrWhiteSpace(targetEmail))
            // {
            //     _logger.LogWarning("Target email address is required.");
            //     return BadRequest("Target email address is required.");
            // }

            try
            {
                int reservationID = 3;
                

                // (string ics, string body, Response result) = await _emailConfirm.CreateConfirmation(reservationID);
                // //(ics, otp, result) = await _emailConfirm.ResendConfirmation(reservationID);

                // if (string.IsNullOrEmpty(body))
                // {
                //     _logger.LogError("Failed to create email confirmation. body is null");
                //     return StatusCode(500, "Failed to create email confirmation. body is null");
                // }
                // if (string.IsNullOrEmpty(ics))
                // {
                //     _logger.LogError("Failed to create email confirmation. Ics is null");
                //     return StatusCode(500, "Failed to create email confirmation. Ics is null");
                // }
                // if (string.IsNullOrEmpty(otp))
                // {
                //     _logger.LogError("Failed to create email confirmation. Otp is null");
                //     return StatusCode(500, "Failed to create email confirmation. Otp is null");
                // }
                // if (string.IsNullOrEmpty(ics) || string.IsNullOrEmpty(otp) || result.HasError)
                // {
                //     _logger.LogError("Failed to create email confirmation.");
                //     return StatusCode(500, "Failed to create email confirmation.");
                // }

                string subject = "Testing Email Confirmation";
                UserReservationsModel reservation = new UserReservationsModel
                {
                    ReservationID = 3, // Assuming it's not set yet if it's a new reservation
                    CompanyID = 6, // Example company ID
                    FloorPlanID = 4, // Example floor plan ID
                    SpaceID = "SPACE101", // Identifier for the specific space being reserved
                    ReservationStartTime = new DateTime(2024, 4, 1, 11, 0, 0), // May 21, 2024, 14:00
                    ReservationEndTime = new DateTime(2024, 4, 1, 14, 0, 0), // May 21, 2024, 16:00
                    Status = ReservationStatus.Active, // Assuming the reservation is currently active
                    UserHash = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU="
                };
                //string msg = $"Hello,\n\nThis is a test email sent from SpaceSurfers! \nReservation: {ics} \nConfirmation Otp: {otp} \n\nBest,\nPixelPals";
                Response response = await _emailConfirm.SendConfirmation(reservation);
                if (response.HasError)
                {
                    return StatusCode(500, $"Failed to send email confirmation {response.ErrorMessage}");
                }
                _logger.LogInformation("Successfully sent email to {Email}.");
                return Ok("Success");
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError(ex, "SMTP command error while sending email.");
                return StatusCode(500, "SMTP command error: " + ex.Message);
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError(ex, "Authentication error while sending email.");
                return StatusCode(500, "Authentication error: " + ex.Message);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "IO error while sending email.");
                return StatusCode(500, "IO error: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> CreateEmail([FromBody]int reservationID)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     string otp = string.Empty;
        //     string icsFile = string.Empty;
        //     string htmlBody = string.Empty;
        //     Response result = new Response();
        //     (icsFile, otp, htmlBody, result) = await _emailConfirm.CreateConfirmation(reservationID);
        //     //result = await _emailDAO.InsertConfirmationInfo(reservationID, otp, fileBytes);
        //     if (string.IsNullOrEmpty(htmlBody))
        //     {
        //         _logger.LogError("Failed to create email confirmation. body is null");
        //         return StatusCode(500, "Failed to create email confirmation. body is null");
        //     }
        //     if (string.IsNullOrEmpty(icsFile))
        //     {
        //         _logger.LogError("Failed to create email confirmation. Ics is null");
        //         return StatusCode(500, "Failed to create email confirmation. Ics is null");
        //     }
        //     if (string.IsNullOrEmpty(otp))
        //     {
        //         _logger.LogError("Failed to create email confirmation. Otp is null");
        //         return StatusCode(500, "Failed to create email confirmation. Otp is null");
        //     }
        //     if (result.HasError)
        //     {
        //         _logger.LogError("Failed to create email confirmation.");
        //         return StatusCode(500, "Failed to create email confirmation.");
        //     }
        //     return Ok(new{icsFile, htmlBody, result});
    
        // }
    }
}


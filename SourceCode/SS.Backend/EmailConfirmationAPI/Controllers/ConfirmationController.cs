using MailKit.Net.Smtp;
using System.IO;
using SS.Backend.ReservationManagement;
using SS.Backend.EmailConfirm;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Data;
using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using SS.Backend.SpaceManager;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.EmailConfirm;
using System.Text.Json;

namespace EmailConfirmationAPI.Controllers
{
    [ApiController]
    [Route("api/v1/reservationConfirmation")]
    public class ConfirmationController : ControllerBase
    {
        private readonly IEmailConfirmDAO _emailDao;
        private readonly IEmailConfirmService _emailService;
        private readonly IEmailConfirmSender _emailSender;

        public ConfirmationController(IEmailConfirmDAO emailDao, IEmailConfirmService emailService, IEmailConfirmSender emailSender)
        {
            _emailDao = emailDao;
            _emailService = emailService;
            _emailSender = emailSender;

        }

        [HttpPost("SendConfirmation")]
        public async Task<IActionResult> SendConfirmation(int ReservationID)
        {
            UserReservationsModel reservation = null;
            Response resResponse = new Response();
            try
            {
                (reservation, resResponse) = await _emailDao.GetUserReservationByID(ReservationID);
                Console.WriteLine(resResponse.ErrorMessage);
                if (reservation == null || resResponse.HasError)
                {
                    return StatusCode(500, "Failed to retrieve reservation data: " + resResponse.ErrorMessage);
                }
                
                Response response = await _emailSender.SendConfirmation(reservation);
                if (response.HasError)
                {
                    return StatusCode(500, $"Failed to send email confirmation: {response.ErrorMessage}");
                }
               
                return Ok("Success");
            }
            catch (SmtpCommandException ex)
            {
               
                return StatusCode(500, "SMTP command error: " + ex.Message);
            }
            catch (IOException ex)
            {
              
                return StatusCode(500, "IO error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(resResponse.ErrorMessage);
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }

        [HttpPost("ResendConfirmation")]
        public async Task<IActionResult> ResendConfirmation(int reservationID)
        {
            UserReservationsModel reservation = null;
            Response resResponse = new Response();
            try
            {
                (reservation, resResponse) = await _emailDao.GetUserReservationByID(reservationID);
                Console.WriteLine(resResponse.ErrorMessage);
                if (reservation == null || resResponse.HasError)
                {
                    return StatusCode(500, "Failed to retrieve reservation data: " + resResponse.ErrorMessage);
                }
                
                Response response = await _emailSender.ResendEmail(reservation);
                if (response.HasError)
                {
                    return StatusCode(500, $"Failed to resend email confirmation: {response.ErrorMessage}");
                }
               
                return Ok("Success");
            }
            catch (SmtpCommandException ex)
            {
               
                return StatusCode(500, "SMTP command error: " + ex.Message);
            }
            catch (IOException ex)
            {
              
                return StatusCode(500, "IO error: " + ex.Message);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }

        [HttpPost("ConfirmReservation")]
        public async Task<IActionResult> ConfirmReservaiton([FromQuery] int reservationID, [FromQuery] string otp)
        {
            try
            {
                Response response = await _emailService.ConfirmReservation(reservationID, otp);
                if (response.HasError)
                {
                    return StatusCode(500, $"Failed to confirm reservation: {response.ErrorMessage}");
                }
               
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }
    }
}

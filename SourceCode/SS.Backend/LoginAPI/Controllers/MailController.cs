using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using SS.Backend.Services.EmailService;
using System.Net.Mail;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/mail")]
    public class MailController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Send()
        {
            string targetEmail = "sarahphan915@gmail.com";
            string subject = "Testing";
            string msg = "Hello Sarah,\n\nThis is a test email sent from SpaceSurfers!\n\nBest,\nPixelPals";

            try
            {
                await MailSender.SendEmail(targetEmail, subject, msg);
                Console.WriteLine("Successfully sent email.");
                return Ok("Success");
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("SMTP error: " + ex.Message);
                return StatusCode(500, "SMTP error: " + ex.Message);
            }
            catch (IOException ex)
            {
                Console.WriteLine("IO error: " + ex.Message);
                return StatusCode(500, "IO error: " + ex.Message);
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine("Authentication error: " + ex.Message);
                return StatusCode(500, "Authentication error: " + ex.Message);
            }
            catch (Exception ex) // Catch any other unexpected exceptions
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                return StatusCode(500, "Error sending email: " + ex.Message);
            }
        }
    }
}

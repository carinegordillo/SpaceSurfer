using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;
using SS.Backend.Services.EmailService;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthNController : Controller
    {
        private readonly SSAuthService _authService;
        private readonly IConfiguration _config;

        public AuthNController(SSAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("sendOTP")]
        public async Task<IActionResult> SendOTP([FromBody] AuthenticationRequest request)
        {
            var (otp, response) = await _authService.SendOTP_and_SaveToDB(request);
            Console.WriteLine("initializng login attempt");
            await _authService.initializeLoginAttempt(request);

            if (response.HasError == false)
            {
                string targetEmail = request.UserIdentity;
                string msg = $"Your verification code is: {otp}\r\n\r\nPlease keep this code confidential and do not share it with anyone else. This code is for your personal use only and will help ensure the security of your account.\r\n\r\nIf you did not request this verification code or have any concerns, please contact us immediately at spacesurfers5@gmail.com.\r\n\r\nBest regards,\r\nSpace Surfer Team";
                string subject = "Your Verification Code";
                await MailSender.SendEmail(targetEmail, subject, msg);
            }

            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(new { otp });
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
        {
            var (principal, response) = await _authService.Authenticate(request);

            if (response.HasError)
            {
                Console.WriteLine("checking login attempts");
                bool deactivate = await _authService.checkLoginAttempts(request);
                if (deactivate)
                {
                    Console.WriteLine("sending deactivate email");
                    string targetEmail = request.UserIdentity;
                    string subject = "Account Deactivated";
                    string msg = "We regret to inform you that your account has been temporarily deactivated due to multiple unsuccessful login attempts.\r\n\r\nTo recover your account, please follow the link provided on the login page. If you have any questions or need further assistance, please don't hesitate to contact us at spacesurfers5@gmail.com.\r\n\r\nBest regards,\r\nSpace Surfer Team";
                    await MailSender.SendEmail(targetEmail, subject, msg);
                }

                return BadRequest(response.ErrorMessage);
            }

            if (principal != null && !string.IsNullOrEmpty(principal.UserIdentity))
            {
                var rolesDictionary = principal.Claims;

                rolesDictionary ??= new Dictionary<string, string>();

                var accessToken = _authService.CreateJwt(Request, principal);
                var idToken = _authService.CreateIdToken(principal);

                await _authService.clearLoginAttempts(request);
                return Ok(new { accessToken, idToken });
            }
            else
            {
                return BadRequest("Principal or UserIdentity is null or empty.");
            }
        }

        [HttpPost("verifyCode")]
        public async Task<IActionResult> verifyCode([FromBody] AuthenticationRequest request)
        {

            var response = await _authService.verifyCode(request);

            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }
            else
            {
                return Ok();
            }
        }

        [HttpPost("getRole")]
        public IActionResult GetRole([FromBody] Jwt token)
        {
            if (token != null)
            {
                var claims = token.Payload.Claims;
                if (claims != null)
                {
                    string role = claims["Role"];
                    return Ok(role);
                }
                else
                {
                    return BadRequest("No claims in token.");
                }
            }
            else
            {
                return BadRequest("No token found.");
            }
        }

    }
}

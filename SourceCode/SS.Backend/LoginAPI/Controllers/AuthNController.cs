using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;
using SS.Backend.Services.EmailService;

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

            if (response.HasError == false)
            {
                string targetEmail = request.UserIdentity;
                string subject = "Verfication Code";
                string msg = "Verification code: " + otp;
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
                return BadRequest(response.ErrorMessage);
            }

            if (principal != null && !string.IsNullOrEmpty(principal.UserIdentity))
            {
                var rolesDictionary = principal.Claims;

                // Check if rolesDictionary is null, and provide a default empty dictionary if it is
                rolesDictionary ??= new Dictionary<string, string>();

                var token = await _authService.GenerateAccessToken(principal.UserIdentity, rolesDictionary);

                return Ok(token);
            }
            else
            {
                return BadRequest("Principal or UserIdentity is null or empty.");
            }
        }


        [HttpPost("decodeToken")]
        public IActionResult decodeToken([FromBody] string accessToken)
        {
            List<string> role = _authService.GetRolesFromToken(accessToken);
            string exp_time = _authService.GetExpTimeFromToken(accessToken);
            string subject = _authService.GetSubjectFromToken(accessToken);
            return Ok( new { role, exp_time, subject });
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken(string username, IDictionary<string, string>? roles)
        {
            try
            {
                if (username == null)
                {
                    return BadRequest("Username cannot be null");
                }

                var (newToken, response) = await _authService.RefreshToken(username, roles ?? new Dictionary<string, string>());

                if (response.HasError)
                {
                    return BadRequest(response.ErrorMessage);
                }

                return Ok(newToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

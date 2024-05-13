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
            var ip = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            Console.WriteLine("this is the ip: {0}", ip);
            var (principal, response) = await _authService.Authenticate(request, ip);

            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }

            if (principal != null && !string.IsNullOrEmpty(principal.UserIdentity))
            {
                var rolesDictionary = principal.Claims;

                rolesDictionary ??= new Dictionary<string, string>();

                var accessToken = _authService.CreateJwt(Request, principal);
                var idToken = _authService.CreateIdToken(principal);

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

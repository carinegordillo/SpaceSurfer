using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;

namespace HTMLTesting.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthNController : Controller
    {
        private readonly SSAuthService _authService;

        public AuthNController(SSAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("sendOTP")]
        public async Task<IActionResult> SendOTP([FromBody] AuthenticationRequest request)
        {
            var (otp, response) = await _authService.SendOTP_and_SaveToDB(request);

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

            return Ok(new { principal });
        }
    }
}

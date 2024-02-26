using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;

namespace TestAPI.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly SSAuthService _authService;

        public AuthController(SSAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
        {
            var (token, response) = await _authService.SendOTP_and_SaveToDB(request);
            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }
            return Ok(new { Token = token });
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
        {
            var (principal, response) = await _authService.Authenticate(request);
            if (response.HasError)
            {
                return Unauthorized(response.ErrorMessage);
            }
            return Ok(principal);
        }
    }
}

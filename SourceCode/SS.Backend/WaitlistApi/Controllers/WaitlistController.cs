using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;
using SS.Backend.Services.EmailService;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

namespace WaitlistApi.Controllers
{
    [ApiController]
    [Route("api/waitlist")]
    public class WaitlistController : Controller
    {
        private readonly SSAuthService _authService;
        private readonly IConfiguration _config;

        public WaitlistController(SSAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpGet("test")]
        public async Task<IActionResult> test()
        {
            string accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

                if (claimsJson != null)
                {
                    // Deserialize the JSON string into a dictionary
                    var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                    if (claims.TryGetValue("Role", out var role) && role == "2")
                    {
                        return Ok("Role is valid.");
                    }
                    else
                    {
                        return BadRequest("Unauthorized role.");
                    }
                }
                else
                {
                    return BadRequest("Invalid token.");
                }
            }
            else
            {
                return BadRequest("Unauthorized. Access token is missing or invalid.");
            }
        }


    }
}

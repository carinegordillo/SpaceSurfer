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

        [HttpGet("button")]
        public async Task<IActionResult> button()
        {
            
            string accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

                if (claimsJson != null)
                {
                    var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                    if (claims.TryGetValue("Role", out var role) && role == "2")
                    {
                        bool closeToExpTime = _authService.CheckExpTime(accessToken);
                        if (closeToExpTime)
                        {
                            SSPrincipal principal = new SSPrincipal();
                            principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
                            principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
                            var newToken = _authService.CreateJwt(Request, principal);

                            bool testVar = true;
                            return Ok(new { testVar, newToken });
                        }
                        else
                        {
                            bool testVar = true;
                            return Ok(testVar);
                        }
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

        [HttpGet("checkTokenExp")]
        public async Task<IActionResult> checkTokenExp()
        {

            string accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                bool tokenExpired = _authService.IsTokenExpired(accessToken);
                if (tokenExpired)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                return BadRequest("Unauthorized. Access token is missing or invalid.");
            }
        }

    }
}

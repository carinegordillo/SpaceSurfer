using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;
using SS.Backend.Services.PersonalOverviewService;
using System.Text.Json;



namespace PersonalOverviewAPI.Controllers
{
    [ApiController]
    [Route("api/v1/PersonalOverview")]
    public class PersonalOverviewController : ControllerBase
    {
        private readonly IPersonalOverview _personalOverviewService;
        private readonly SSAuthService _authService;
        private readonly IConfiguration _config;

        public PersonalOverviewController(IPersonalOverview personalOverviewService, SSAuthService authService, IConfiguration config)
        {
            _personalOverviewService = personalOverviewService;
            _authService = authService;
            _config = config;
        }

        [HttpGet("Reservations")]
        public async Task<IActionResult> GetUserReservations([FromQuery(Name = "fromDate")] DateOnly? fromDate = null, [FromQuery(Name = "toDate")] DateOnly? toDate = null)
        {
            string? accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

                if (claimsJson != null)
                {
                    var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                    if (claims.TryGetValue("Role", out var role) && (role == "1" || role == "2" || role == "3" || role == "4" || role == "5"))
                    {
                        try
                        {
                            var user = _authService.ExtractSubjectFromToken(accessToken);
                            var reservations = await _personalOverviewService.GetUserReservationsAsync(user, fromDate, toDate);
                            if (_authService.CheckExpTime(accessToken))
                            {
                                SSPrincipal principal = new SSPrincipal();
                                principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
                                principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
                                var newToken = _authService.CreateJwt(Request, principal);
                                return Ok(new { reservations, newToken });
                            }
                            else
                            {
                                return Ok(reservations);
                            }
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, $"An error occurred while fetching user reservations: {ex.Message}");
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

        [HttpPost("ReservationDeletion")]
        public async Task<IActionResult> DeleteReservations([FromBody] int reservationID)
        {

            Console.WriteLine("1");
            string? accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                var claimsJson = _authService.ExtractClaimsFromToken(accessToken);
                Console.WriteLine("2");
                if (claimsJson != null)
                {
                    var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                    if (claims.TryGetValue("Role", out var role) && (role == "1" || role == "2" || role == "3" || role == "4" || role == "5"))
                    {
                        Console.WriteLine("3");
                        try
                        {
                            Console.WriteLine("4");
                            var user = _authService.ExtractSubjectFromToken(accessToken);
                            var deleteReservation = await _personalOverviewService.DeleteUserReservationsAsync(user, reservationID);
                            Console.WriteLine("5");
                            if (_authService.CheckExpTime(accessToken))
                            {
                                Console.WriteLine("6");
                                SSPrincipal principal = new SSPrincipal();
                                principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
                                principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
                                var newToken = _authService.CreateJwt(Request, principal);
                                return Ok(new { deleteReservation, newToken });
                            }
                            else
                            {
                                return Ok(deleteReservation);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("7");
                            return StatusCode(500, $"An error occurred while fetching user reservations: {ex.Message}");
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

    }
};




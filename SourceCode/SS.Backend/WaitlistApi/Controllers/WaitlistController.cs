using System;
using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;
using SS.Backend.Services.EmailService;
using SS.Backend.Waitlist;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using System.Threading.Tasks;

namespace WaitlistApi.Controllers
{
    [ApiController]
    [Route("api/waitlist")]
    public class WaitlistController : Controller
    {
        private readonly SSAuthService _authService;
        private readonly WaitlistService _waitlistService;

        public WaitlistController(SSAuthService authService, WaitlistService waitlistService)
        {
            _authService = authService;
            _waitlistService = waitlistService;
        }

        // Get list of all reservations user is waitlisted on
        [HttpGet("getWaitlists")]
        public async Task<IActionResult> GetWaitlistedReservations(string userHash)
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

                            try
                            {
                                var waitlistedReservations = await _waitlistService.GetUserWaitlists(userHash);
                                return Ok(new { waitlistedReservations, newToken });
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, ex.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                var waitlistedReservations = await _waitlistService.GetUserWaitlists(userHash);
                                return Ok(waitlistedReservations);
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, ex.Message);
                            }
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

        // Get details for that specific reservation
        [HttpGet("getDetails")]
        public async Task<IActionResult> GetReservationDetails(string userHash, int reservationId)
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

                            try
                            {
                                var resDetails = await _waitlistService.GetReservationDetails(userHash, reservationId);
                                if (resDetails == null)
                                {
                                    return NotFound();
                                }
                                return Ok(new { resDetails, newToken });
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, ex.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                var resDetails = await _waitlistService.GetReservationDetails(userHash, reservationId);
                                if (resDetails == null)
                                {
                                    return NotFound();
                                }
                                return Ok(resDetails);
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, ex.Message);
                            }
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

        // Leave waitlist
        [HttpPost("leaveWaitlist")]
        public async Task<IActionResult> LeaveWaitlist(string userHash, int reservationId)
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

                            try
                            {
                                int leavePos = await _waitlistService.GetWaitlistPosition(userHash, reservationId);
                                await _waitlistService.UpdateWaitlist_WaitlistedUserLeft(reservationId, leavePos);
                                bool success = true;
                                return Ok(new { success, newToken });
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, ex.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                int leavePos = await _waitlistService.GetWaitlistPosition(userHash, reservationId);
                                await _waitlistService.UpdateWaitlist_WaitlistedUserLeft(reservationId, leavePos);
                                bool success = true;
                                return Ok(success);
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, ex.Message);
                            }
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

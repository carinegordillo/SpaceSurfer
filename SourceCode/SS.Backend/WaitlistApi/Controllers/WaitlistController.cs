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

        [HttpGet("getFloorplan")]
        public async Task<IActionResult> GetCompanyFloorPlans(int cid, int fid)
        {
            Console.WriteLine("Test");
            string? accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

                if (claimsJson != null)
                {
                    var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                    if (claims.TryGetValue("Role", out var role) && role == "1" || role == "2" || role == "3" || role == "4" || role == "5")
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
                                var floor = await _waitlistService.GetCompanyFloorsAsync(cid, fid);

                                return Ok(new { floor, newToken });
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, "Internal server error: " + ex.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                var floor = await _waitlistService.GetCompanyFloorsAsync(cid, fid);

                                return Ok(floor);
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, "Internal server error: " + ex.Message);
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

        // Get list of all reservations user is waitlisted on
        [HttpPost("getWaitlists")]
        public async Task<IActionResult> GetWaitlistedReservations([FromBody] string userHash)
        {
            string accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

                if (claimsJson != null)
                {
                    var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                    if (claims.TryGetValue("Role", out var role) && (role == "1" || role == "2" || role == "3" || role == "4" || role == "5"))
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

                    if (claims.TryGetValue("Role", out var role) && (role == "1" || role == "2" || role == "3" || role == "4" || role == "5"))
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
        public async Task<IActionResult> LeaveWaitlist([FromBody] LeaveRequestModel requestModel)
        {
            string accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

                if (claimsJson != null)
                {
                    var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                    if (claims.TryGetValue("Role", out var role) && (role == "1" || role == "2" || role == "3" || role == "4" || role == "5"))
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
                                int leavePos = await _waitlistService.GetWaitlistPosition(requestModel.UserHash, requestModel.ReservationId);
                                await _waitlistService.UpdateWaitlist_WaitlistedUserLeft(requestModel.ReservationId, leavePos);
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
                                int leavePos = await _waitlistService.GetWaitlistPosition(requestModel.UserHash, requestModel.ReservationId);
                                await _waitlistService.UpdateWaitlist_WaitlistedUserLeft(requestModel.ReservationId, leavePos);
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

        // Get resId
        [HttpPost("getResId")]
        public async Task<IActionResult> getReservationID([FromBody] ReservationRequestModel requestModel)
        {
            string accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

                if (claimsJson != null)
                {
                    var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                    if (claims.TryGetValue("Role", out var role) && (role == "1" || role == "2" || role == "3" || role == "4" || role == "5"))
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
                                var resId = await _waitlistService.GetReservationID_NoFloor(requestModel.CompanyName, requestModel.SpaceId, requestModel.Start, requestModel.End);
                                return Ok(new { resId, newToken });
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
                                var resId = await _waitlistService.GetReservationID_NoFloor(requestModel.CompanyName, requestModel.SpaceId, requestModel.Start, requestModel.End);
                                return Ok(resId);
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


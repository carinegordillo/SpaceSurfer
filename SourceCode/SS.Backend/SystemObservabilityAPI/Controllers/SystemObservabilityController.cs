using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;
using SS.Backend.Services.PersonalOverviewService;
using SS.Backend.SystemObservability;
using System.Text.Json;

namespace SystemObservabilityAPI.Controllers
{
    [ApiController]
    [Route("api/v1/SystemObservability")]
    public class SystemObservabilityController : ControllerBase
    {

        private readonly IViewDurationService _viewDurationService;
        private readonly ILoginCountService _loginCountService;
        private readonly ICompanyReservationCountService _companyReservationCountService;
        //private readonly SSAuthService _authService;
        private readonly IConfiguration _config;


        public SystemObservabilityController(IViewDurationService viewDurationService, ILoginCountService loginCountService, 
            ICompanyReservationCountService companyReservationCountService, IConfiguration config)
        {
            _viewDurationService = viewDurationService;
            _loginCountService = loginCountService;
           // _authService = authService;
            _config = config;
        }

        [HttpGet("Information")]
        public async Task<IActionResult> GetAllInformation([FromQuery(Name = "timeSpan")] string timeSpan)
        {
            //string? accessToken = HttpContext.Request.Headers["Authorization"];
            //if (accessToken != null && accessToken.StartsWith("Bearer "))
            //{
            //    accessToken = accessToken.Substring("Bearer ".Length).Trim();
            //    var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

            //    if (claimsJson != null)
            //    {
            //        var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

            //        if (claims.TryGetValue("Role", out var role) && (role == "1" ))
            //        {
            //            try
            //            {
            //                var user = _authService.ExtractSubjectFromToken(accessToken);
                            var user = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=";

                            var viewsDurationCount = await _viewDurationService.GetTop3ViewDuration(user,timeSpan);
                            var loginsCount = await _loginCountService.GetLoginCount(user,timeSpan);
                            var topCompanyReservationCount = await _companyReservationCountService.GetTop3CompaniesWithMostReservations(user,timeSpan);
                  


            //                if (_authService.CheckExpTime(accessToken))
            //                {
            //                    SSPrincipal principal = new SSPrincipal();
            //                    principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
            //                    principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
            //                    var newToken = _authService.CreateJwt(Request, principal);
            //                    return Ok(new { views, newToken });
            //                }
            //                else
            //                {
                                return Ok(new { viewsDurationCount, loginsCount, topCompanyReservationCount });
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                return StatusCode(500, $"An error occurred while fetching user reservations: {ex.Message}");
            //            }
            //        }
            //        else
            //        {
            //            return BadRequest("Unauthorized role.");
            //        }
            //    }
            //    else
            //    {
            //        return BadRequest("Invalid token.");
            //    }
            //}
            //else
            //{
            //    return BadRequest("Unauthorized. Access token is missing or invalid.");
            //}
        }

        [HttpPost("ViewDurationInsertion")]
        public async Task<IActionResult> InsertViewDuration([FromQuery(Name = "viewName")] string viewName, [FromQuery(Name = "durationInSeconds")] int durationInSeconds)
        {

        //    string? accessToken = HttpContext.Request.Headers["Authorization"];
        //    if (accessToken != null && accessToken.StartsWith("Bearer "))
        //    {
        //        accessToken = accessToken.Substring("Bearer ".Length).Trim();
        //        var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

        //        if (claimsJson != null)
        //        {
        //            var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

        //            if (claims.TryGetValue("Role", out var role) && (role == "1" || role == "2" || role == "3" || role == "4" || role == "5"))
        //            {

        //                try
        //                {

        //                    var user = _authService.ExtractSubjectFromToken(accessToken);
                           var user = "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=";
                           var viewDurationInsertion = await _viewDurationService.InsertViewDuration(user, viewName, durationInSeconds);

        //                    if (_authService.CheckExpTime(accessToken))
        //                    {

        //                        SSPrincipal principal = new SSPrincipal();
        //                        principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
        //                        principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
        //                        var newToken = _authService.CreateJwt(Request, principal);
        //                        return Ok(new { viewDurationInsertion, newToken });
        //                    }
        //                    else
        //                    {
                                return Ok(viewDurationInsertion);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {

        //                    return StatusCode(500, $"An error occurred while fetching user reservations: {ex.Message}");
        //                }
        //            }
        //            else
        //            {
        //                return BadRequest("Unauthorized role.");
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest("Invalid token.");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("Unauthorized. Access token is missing or invalid.");
        //    }
        }
    }
}

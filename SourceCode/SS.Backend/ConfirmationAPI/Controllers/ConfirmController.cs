using MailKit.Net.Smtp;
using System.IO;
using SS.Backend.ReservationManagement;
using SS.Backend.EmailConfirm;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Data;
using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagers;
using SS.Backend.SpaceManager;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConfirmationAPI.Controllers
{
    [ApiController]
    [Route("api/v1/ConfirmedReservations")]
    public class ConfirmController : ControllerBase
    {
        private readonly IEmailConfirmDAO _emailDao;
        private readonly IEmailConfirmService _emailService;
        private readonly IEmailConfirmSender _emailSender;
        private readonly SSAuthService _authService;
        private readonly IConfirmationDeletion _deleteConfirm;
        private readonly IEmailConfirmList _confirmList;

        public ConfirmController(IEmailConfirmDAO emailDao, 
                                        IEmailConfirmService emailService, 
                                        IEmailConfirmSender emailSender,
                                        SSAuthService authService,
                                        IConfirmationDeletion deleteConfirm,
                                        IEmailConfirmList confirmList)
        {
            _emailDao = emailDao;
            _emailService = emailService;
            _emailSender = emailSender;
            _authService = authService;
            _deleteConfirm = deleteConfirm;
            _confirmList = confirmList;
        }

        [HttpGet("ListConfirmations")]
        public async Task<IActionResult> ListConfirmations([FromBody] string hashedUsername)
        {
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
                            Response response = new Response();
                            try
                            {
                                var confirmations = await _confirmList.ListConfirmations(hashedUsername);
                                var count = confirmations.Count();
                                Console.WriteLine(count);
                            
                                if (confirmations == null)
                                {
                                    return StatusCode(500, "Failed to retrieve confirmed reservations data. ");
                                }
                            
                                return Ok(new { confirmations, newToken });
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine(resResponse.ErrorMessage);
                                return StatusCode(500, "Error listing confirmations: " + ex.Message);
                            }
                        }
                        else
                        {
                            Response response = new Response();
                            try
                            {
                                var confirmations = await _confirmList.ListConfirmations(hashedUsername);
                                var count = confirmations.Count();
                                Console.WriteLine(count);
                            
                                if (confirmations == null)
                                {
                                    return StatusCode(500, "Failed to retrieve confirmed reservations data. ");
                                }
                            
                                return Ok(new {confirmations});
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine(resResponse.ErrorMessage);
                                return StatusCode(500, "Error listing confirmations: " + ex.Message);
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

       
            // Response response = new Response();
            // try
            // {
            //     var confirmations = await _confirmList.ListConfirmations(hashedUsername);
            //     var count = confirmations.Count();
            //     Console.WriteLine(count);
            
            //     if (confirmations == null)
            //     {
            //         return StatusCode(500, "Failed to retrieve confirmed reservations data. ");
            //     }
               
            //     return Ok(confirmations);
            // }
            // catch (Exception ex)
            // {
            //     //Console.WriteLine(resResponse.ErrorMessage);
            //     return StatusCode(500, "Error listing confirmations: " + ex.Message);
            // }
        }

        [HttpPost("CancelConfirmation")]
        public async Task<IActionResult> CancelConfirmation([FromQuery] string hashedUsername, [FromQuery] int reservationID)
        {
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
                            Response response = new Response();
                            try
                            {
                                response = await _deleteConfirm.CancelConfirmedReservation(hashedUsername, reservationID);
                                Console.WriteLine(response.ErrorMessage);
                                if (response.HasError)
                                {
                                    return StatusCode(500, "Failed to cancel confirmed reservation data: " + response.ErrorMessage);
                                }
                            
                                return Ok(new { response, newToken });
                            }
                            catch (Exception ex)
                            {
                            
                                return StatusCode(500, "Error canceling confirmed reservation: " + ex.Message);
                            }
                        }
                        else
                        {
                            Response response = new Response();
                            try
                            {
                                response = await _deleteConfirm.CancelConfirmedReservation(hashedUsername, reservationID);
                                Console.WriteLine(response.ErrorMessage);
                                if (response.HasError)
                                {
                                    return StatusCode(500, "Failed to cancel confirmed reservation data: " + response.ErrorMessage);
                                }
                            
                                return Ok(new {response});
                            }
                            catch (Exception ex)
                            {
                            
                                return StatusCode(500, "Error canceling confirmed reservation: " + ex.Message);
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


            // Response response = new Response();
            // try
            // {
            //     response = await _deleteConfirm.CancelConfirmedReservation(hashedUsername, reservationID);
            //     Console.WriteLine(response.ErrorMessage);
            //     if (response.HasError)
            //     {
            //         return StatusCode(500, "Failed to cancel confirmed reservation data: " + response.ErrorMessage);
            //     }
               
            //     return Ok("Success");
            // }
            // catch (Exception ex)
            // {
               
            //     return StatusCode(500, "Error canceling confirmed reservation: " + ex.Message);
            // }
        }

        [HttpDelete("DeleteConfirmation/{reservationID}")]
        public async Task<IActionResult> DeleteConfirmation([FromBody] int reservationID)
        {
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
                            Response response = new Response();
                            try
                            {
                                response = await _deleteConfirm.DeleteConfirmedReservation(reservationID);
                                Console.WriteLine(response.ErrorMessage);
                                if (response.HasError)
                                {
                                    return StatusCode(500, "Failed to delete confirmed reservation data: " + response.ErrorMessage);
                                }
                            
                                return Ok(new {response, newToken});
                            }
                            catch (Exception ex)
                            {
                            
                                return StatusCode(500, "Error deleting confirmed reservation: " + ex.Message);
                            }
                        }
                        else
                        {
                            Response response = new Response();
                            try
                            {
                                response = await _deleteConfirm.DeleteConfirmedReservation(reservationID);
                                Console.WriteLine(response.ErrorMessage);
                                if (response.HasError)
                                {
                                    return StatusCode(500, "Failed to delete confirmed reservation data: " + response.ErrorMessage);
                                }
                            
                                return Ok(new {response});
                            }
                            catch (Exception ex)
                            {
                            
                                return StatusCode(500, "Error deleting confirmed reservation: " + ex.Message);
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


            // Response response = new Response();
            // try
            // {
            //     response = await _deleteConfirm.DeleteConfirmedReservation(reservationID);
            //     Console.WriteLine(response.ErrorMessage);
            //     if (response.HasError)
            //     {
            //         return StatusCode(500, "Failed to delete confirmed reservation data: " + response.ErrorMessage);
            //     }
               
            //     return Ok("Success");
            // }
            // catch (Exception ex)
            // {
               
            //     return StatusCode(500, "Error deleting confirmed reservation: " + ex.Message);
            // }
        }

        [HttpGet("checkTokenExp")]
        public IActionResult checkTokenExp()
        {

            string? accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                bool tokenExpired = _authService.IsTokenExpired(accessToken);
                if (tokenExpired)
                {
                    Console.WriteLine("Token is expired.");
                    return Ok(true);
                }
                else
                {
                    Console.WriteLine("Token is not expired.");
                    return Ok(false);
                }
            }
            else
            {
                Console.WriteLine("Token is missing or invalid.");
                return BadRequest("Unauthorized. Access token is missing or invalid.");
            }
        }
    }
}

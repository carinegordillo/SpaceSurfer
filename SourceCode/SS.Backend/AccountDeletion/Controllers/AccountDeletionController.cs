// AccountDeletionController.cs
using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;
using SS.Backend.Services.DeletingService;
using System.Text.Json;

namespace AccountDeletionAPI.Controllers
{
    // Define the route and indicating the class is a controller
    [Route("api/AccountDeletion")]
    [ApiController]
    public class AccountDeletionController : ControllerBase
    {

        private readonly IAccountDeletion _accountDeletion;
        private readonly SSAuthService _authService;
        private readonly IConfiguration _config;

        // Constructing injection for account deletion
        public AccountDeletionController(IAccountDeletion accountDeletionService, SSAuthService authService, IConfiguration config)
        {
            _accountDeletion = accountDeletionService;
            _authService = authService;
            _config = config;
        }

        // Defining the Delete endpoint for account deletion
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteAccountDeletion([FromBody] DeletionRequest request)
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
                            var deleteReservation = await _accountDeletion.DeleteAccount(user);

                            if (_authService.CheckExpTime(accessToken))
                            {

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
}

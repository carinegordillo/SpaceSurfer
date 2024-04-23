// using Microsoft.AspNetCore.Mvc;
// using SS.Backend.Security;
// using System.Data;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization;

// namespace SecurityAPI.Controllers;

// [ApiController]
// [Route("/api/securityAuth")]

// public class SecurityController : ControllerBase
// {
//     private readonly SSAuthService _authService;
//     public SecurityController(SSAuthService authService)
//     {
//         _authService = authService;
//     }

//     [HttpPost("postSendOTP")]
//     public async Task<IActionResult> PostSendOTP([FromBody] AuthenticationRequest request)
//     {
//         var (otp, response) = await _authService.SendOTP_and_SaveToDB(request);

//         if (response.HasError)
//         {
//             return BadRequest(response.ErrorMessage);
//         }

//         return Ok(new { otp });
//     }
    
//     [HttpPost("postAuthenticate")]
//     //[Route("/authN")]
//     public async Task<IActionResult> PostAuthenticate([FromBody] AuthenticationRequest authRequest)
//     {
//         var (principal, response) = await _authService.Authenticate(authRequest);

//         if (response == null)
//         {
//             return BadRequest("Authentication response is null.");
//         }

//         if (response.HasError)
//         {
//             return BadRequest(response.ErrorMessage);
//         }

//         // Assuming your principal object contains necessary information like username and roles
//         // You might need to adjust this based on your actual implementation
//         var username = principal.UserIdentity;
//         var roleClaims = principal.Claims.Where(c => c.Key == "Role").ToList();
//         var rolesDictionary = roleClaims.ToDictionary(roleClaim => roleClaim.Value, roleClaim => roleClaim.Value);

//         // Generate Access Token after successful authentication
//         var accessToken = _authService.GenerateAccessToken(username, rolesDictionary);

       
//         return Ok(new {accessToken});
//     }
    

//     [HttpGet("getAuthorize")]
//     [Authorize]
//     //[PostAuthorize(Policy = "RequireClaimPolicy")]
//     public async Task<IActionResult> GetAuthorize(/*[FromQuery] SSPrincipal currentPrincipal, [FromQuery] IDictionary<string, string> requiredClaims*/)
//     {
//         var currentPrincipal = _authService.MapToSSPrincipal(User);
//         // example claims needed for some function
//         var requiredClaims = new Dictionary<string, string> {{ "Role", "Admin" }};
//         // Perform authorization check
//         var isAuthorized = await _authService.IsAuthorize(currentPrincipal, requiredClaims);

//         // Return appropriate HTTP response
//         if (isAuthorized)
//         {
//             return Ok("User is authorized.");
//         }
//         else
//         {
//             return Forbid("User is not authorized.");
//         }
        
        
//     }

//     /*
//     [HttpGet("getUserData")]
//     public IActionResult GetUserData()
//     {
//         // Check if user is authenticated
//         if (HttpContext.User.Identity?.IsAuthenticated == true)
//         {
//             // Directly use ClaimsPrincipal for standard operations
//             var userName = HttpContext.User.Identity.Name;

//             // Custom authorization check (if needed) could be performed here, but typically handled by attributes
//             return Ok($"Hello, {userName}");
//         }
//         else
//         {
//             return Unauthorized("User is not authenticated.");
//         }
//     }
//     */

// }


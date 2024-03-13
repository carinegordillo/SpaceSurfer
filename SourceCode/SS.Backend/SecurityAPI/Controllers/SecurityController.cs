using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;
using System.Data;
using System.Threading.Tasks;

namespace SecurityAPI.Controllers;

[ApiController]
[Route("api/securityAuth")]

public class SecurityController : ControllerBase
{
    private readonly SSAuthService _authService;
    public SecurityController(SSAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("postSendOTP")]
    public async Task<IActionResult> PostSendOTP([FromBody] AuthenticationRequest request)
    {
        var (otp, response) = await _authService.SendOTP_and_SaveToDB(request);

        if (response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(new { otp });
    }
    
    [HttpPost("postAuthenticate")]
    //[Route("/authN")]
    public async Task<IActionResult> PostAuthenticate([FromBody] AuthenticationRequest authRequest)
    {
        var (principal, response) = await _authService.Authenticate(authRequest);

        if (response == null)
        {
            return BadRequest("Authentication response is null.");
        }

        if (response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(principal);
        /*
        if (result.HasError)
        {
            return BadRequest(result.ErrorMessage);
        }
        else if (principal != null)
        {
            return Ok(principal);//return Ok(new { principal });
        }
        else
        {
            return Unauthorized();
        }
        */
    }
    

    [HttpPost("postAuthorize")]
    //[PostAuthorize(Policy = "RequireClaimPolicy")]
    public async Task<IActionResult> PostAuthorize([FromQuery] SSPrincipal currentPrincipal, [FromQuery] IDictionary<string, string> requiredClaims)
    {
        
        // Perform authorization check
        bool isAuthorized = await _authService.IsAuthorize(currentPrincipal, requiredClaims);

        // Return appropriate HTTP response
        if (isAuthorized)
        {
            return Ok("User is authorized.");
        }
        else
        {
            return Forbid("User is not authorized.");
        }
        
        
    }

}


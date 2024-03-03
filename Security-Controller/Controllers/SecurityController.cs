/*
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SS.Backend.DataAccess;
using SS.Backend.Security;

namespace Security_Controller.Controllers;

[ApiController]
[Route("[controller]")]
public class SecurityController : Controller
{
    private readonly SSAuthService _authService;
    public SecurityController(SSAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("postAuthN")]
    [Route("/authN")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
        {
            var (principal, response) = await _authService.Authenticate(request);

            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(new { principal });
        }

    [HttpGet(Name = "getAuthZ")]
    [Route("/authZ")]
    public async Task<IActionResult> AuthZ([FromRouteAttribute] SSPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
    {
        var verify = await _authService.IsAuthorize(currentPrincipal, requiredClaims);

        return Ok(new { verify} );
    }

    
    [HttpPost(Name = "postSendOTP")]
    [Route("/otp")]
    public async Task<IActionResult> SendOTP([FromBody] AuthenticationRequest authRequest)
    {   
        var (otp, response) = await _authService.SendOTP_and_SaveToDB(authRequest);

        if(response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }
        
        return Ok(new { otp });
    }
    
    
}
*/

using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;

namespace Security_Controller.Controllers;

[ApiController]
[Route("[controller]")]
public class SecurityController : Controller
{
    private readonly SSAuthService _authService;
    public SecurityController(SSAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("sendOTP")]
    public async Task<IActionResult> SendOTP([FromBody] AuthenticationRequest request)
    {
        var (otp, response) = await _authService.SendOTP_and_SaveToDB(request);

        if (response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(new { otp });
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
    {
        var (principal, response) = await _authService.Authenticate(request);

        if (response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(new { principal });
    }

    [HttpPost(Name = "postAuthZ")]
    [Route("/authZ")]
    public async Task<IActionResult> AuthZ([FromQueryAttribute] SSPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
    {
        bool verify = await _authService.IsAuthorize(currentPrincipal, requiredClaims);

        if (!verify)
        {
            // If not authorized, return 401 Unauthorized
            return Unauthorized();
        }
        return Ok(new { verify} );
    }
}



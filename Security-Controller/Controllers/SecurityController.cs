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
    
    [HttpPost(Name = "postSendOTP")]
    public async Task<IActionResult> SendOTP([FromBody] AuthenticationRequest authRequest)
    {   
        var (otp, response) = await _authService.SendOTP_and_SaveToDB(authRequest);

        if(response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }
        
        return Ok(new { otp });
    }

    [HttpPost(Name = "postAuthN")]
    public async Task<IActionResult> AuthN ([FromBody] AuthenticationRequest authRequest)
    {
        var (principal, response) = await _authService.Authenticate(authRequest);

        if(response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(new { principal });
    }

    [HttpPost(Name = "postAuthZ")]
    public async Task<IActionResult> AuthZ([FromRouteAttribute] SSPrincipal currentPrincipal, IDictionary<string, string> requiredClaims)
    {
        var verify = await _authService.IsAuthorize(currentPrincipal, requiredClaims);

        return Ok(new { verify} );
    }

    /* 
    [HttpGet(Name = "getAuthN")]
    public async Task<ActionResult<List<AuthenticationRequest>>> Authenticate(AuthenticationRequest authRequest)
    {
        var response = await _authService.Authenticate(authRequest);
        
        if (response.HasError)
        {
            Console.WriteLine(response.ErrorMessage);
            return StatusCode(500, response.ErrorMessage);
        }

        var requests = response.ValuesRead.Select(row => new AuthenticationRequest
        {
            RequestId = (int)row[0],
            UserHash = (string)row[1],
            RequestDate = (DateTime)row[2],
            Status = (string)row[3],
            RequestType = (string)row[4],
            ResolveDate = row[5] != DBNull.Value ? DateTime.Parse((string)row[5]) : (DateTime?)null,
            AdditionalInformation = row[6] != DBNull.Value ? (string)row[6] : null
        }).ToList();

        return Ok(requests);
    }
    */
    
}

using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;
using System.Data;

namespace SecurityAPI.Controllers;

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

}


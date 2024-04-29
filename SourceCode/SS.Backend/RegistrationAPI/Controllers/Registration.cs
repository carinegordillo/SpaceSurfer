using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.Services;
using SS.Backend.UserManagement;
using SS.Backend.SharedNamespace;

using System.Threading.Tasks;
using System.Web;
using SS.Backend.Security;
using System.Text.Json;
using System.Collections.Generic;

namespace TaskManagerHubAPI.Controllers;

[ApiController]
[Route("api/registration")]
public class RegistrationController : ControllerBase
{

    private readonly IAccountCreation _accountCreation;
    private readonly SSAuthService _authService;
    private readonly IConfiguration _config;
    
    public RegistrationController (IAccountCreation AccountCreation, SSAuthService authService, IConfiguration config){
        _accountCreation = AccountCreation;
        _authService = authService;
        _config = config;
    }
    
    [HttpPost]
    [Route("postAccount")]
    public async Task<IActionResult> PostCreateAccount([FromBody] AccountCreationRequest request){
        try
        {
            var response = await _accountCreation.CreateUserAccount(request.UserInfo, request.CompanyInfo);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    public class VerifyAccountRequest
    {
        public string Username { get; set; }
    }
    [HttpPost]
    [Route("verifyAccount")]
    public async Task<IActionResult> VerifyAccount([FromBody] VerifyAccountRequest request)
    {
        try
        {
            var response = await _accountCreation.VerifyAccount(request.Username);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}
        
 

using Microsoft.AspNetCore.Mvc;

using System.Data;
using SS.Backend.UserManagement;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using System.Text.Json;

using SS.Backend.SharedNamespace;
using SS.Backend.Services.LoggingService;

namespace adminAccountRecoveryAPI.Controllers;
[ApiController]
[Route("api/adminAccountRecovery")]
public class RecoverRequestController : ControllerBase
{
    private readonly IAccountRecovery _accountRecovery;
    private IAccountDisabler _accountDisabler;
    private readonly SSAuthService _authService;
    private readonly IConfiguration _config;

    public RecoverRequestController (IAccountRecovery AccountRecovery, IAccountDisabler AccountDisabler,SSAuthService authService, IConfiguration config){
        _accountRecovery = AccountRecovery;
        _accountDisabler = AccountDisabler;
        _authService = authService;
        _config = config;
    }



    [Route("getAllRequests")]
    [HttpGet]
    public async Task<ActionResult<List<UserRequestModel>>> GetAllRequests(){
       string? accessToken = HttpContext.Request.Headers["Authorization"];
        if (accessToken != null && accessToken.StartsWith("Bearer "))
        {
              
            accessToken = accessToken.Substring("Bearer ".Length).Trim();
            var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

            if (claimsJson != null)
            {
                var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                if (claims.TryGetValue("Role", out var role) && role == "1" )
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
                            List<UserRequestModel> requestList = new List<UserRequestModel>();
                            requestList = await _accountRecovery.ReadUserRequests();
                            return Ok(new { requestList, newToken });
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
                            List<UserRequestModel> requestList = new List<UserRequestModel>();
                            requestList = await _accountRecovery.ReadUserRequests();
                            return Ok(requestList);
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

    [Route("acceptRequests")]
    [HttpPost]
    public async Task<IActionResult> AcceptRequests([FromBody] List<string> userHashes)
    {
        string? accessToken = HttpContext.Request.Headers["Authorization"];
        if (accessToken != null && accessToken.StartsWith("Bearer "))
        {
              
            accessToken = accessToken.Substring("Bearer ".Length).Trim();
            var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

            if (claimsJson != null)
            {
                var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                if (claims.TryGetValue("Role", out var role) && role == "1" )
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
                            var results = new List<object>();

                            foreach (var requestId in userHashes)
                            {
                                var response = await _accountRecovery.RecoverAccount(requestId, true);
                                if (response.HasError)
                                {
                                    Console.WriteLine($"Error processing request {requestId}: {response.ErrorMessage}");
                                    results.Add(new { RequestId = requestId, Success = false, Message = response.ErrorMessage });
                                }
                                else
                                {
                                    results.Add(new { RequestId = requestId, Success = true, Message = "Request accepted successfully" });
                                }
                            }
                            return Ok(new { results, newToken });
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
                            // Initialize a list to hold the result for each requestId
                            var results = new List<object>();

                            foreach (var requestId in userHashes)
                            {
                                var response = await _accountRecovery.RecoverAccount(requestId, true);
                                if (response.HasError)
                                {
                                    Console.WriteLine($"Error processing request {requestId}: {response.ErrorMessage}");
                                    results.Add(new { RequestId = requestId, Success = false, Message = response.ErrorMessage });
                                }
                                else
                                {
                                    results.Add(new { RequestId = requestId, Success = true, Message = "Request accepted successfully" });
                                }
                            }
                            // Return the list of results as JSON 
                            return Ok(results);
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

    [Route("denyRequests")]
    [HttpPost]
    public async Task<IActionResult> DenyRequests([FromBody] List<string> userHashes)
    {
        string? accessToken = HttpContext.Request.Headers["Authorization"];
        if (accessToken != null && accessToken.StartsWith("Bearer "))
        {
              
            accessToken = accessToken.Substring("Bearer ".Length).Trim();
            var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

            if (claimsJson != null)
            {
                var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                if (claims.TryGetValue("Role", out var role) && role == "1" )
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
                            var results = new List<object>();
                            foreach (var requestId in userHashes)
                            {
                                var response = await _accountRecovery.RecoverAccount(requestId, false);
                                if (response.HasError)
                                {
                                    Console.WriteLine($"Error processing request {requestId}: {response.ErrorMessage}");
                                    results.Add(new { RequestId = requestId, Success = false, Message = response.ErrorMessage });
                                }
                                else
                                {
                                    results.Add(new { RequestId = requestId, Success = true, Message = "Request accepted successfully" });
                                }
                            }
                            return Ok(new { results, newToken });
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
                            // Initialize a list to hold the result for each requestId
                            var results = new List<object>();

                            foreach (var requestId in userHashes)
                            {
                                var response = await _accountRecovery.RecoverAccount(requestId, false);
                                if (response.HasError)
                                {
                                    Console.WriteLine($"Error processing request {requestId}: {response.ErrorMessage}");
                                    results.Add(new { RequestId = requestId, Success = false, Message = response.ErrorMessage });
                                }
                                else
                                {
                                    results.Add(new { RequestId = requestId, Success = true, Message = "Request accepted successfully" });
                                }
                            }
                            // Return the list of results as JSON 
                            return Ok(results);
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

    [Route("disableAccount")]
    [HttpPost]
    public async Task<IActionResult> DisableUserAccount([FromBody] string userName)
    {
        string? accessToken = HttpContext.Request.Headers["Authorization"];
        if (accessToken != null && accessToken.StartsWith("Bearer "))
        {
              
            accessToken = accessToken.Substring("Bearer ".Length).Trim();
            var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

            if (claimsJson != null)
            {
                var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                if (claims.TryGetValue("Role", out var role) && role == "1")
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
                            if (string.IsNullOrEmpty(userName))
                            {
                                return BadRequest("User name must be provided.");
                            }

                            var response = await _accountDisabler.DisableAccount(userName);
                            Console.WriteLine(response.HasError);
                            if (response.HasError)
                            {
                                  
                                return Ok(new { success = false, message = "Failed to Disable account." });
                            }
                            else
                            {
                                Console.WriteLine($"Disable request for user: {userName} was successful");
                                return Ok(new { success = true, message = "Account Successfully disabled." ,newToken});
                            }
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
                            if (string.IsNullOrEmpty(userName))
                            {
                                return BadRequest("User name must be provided.");
                            }

                            var response = await _accountDisabler.DisableAccount(userName);
                            Console.WriteLine(response.HasError);
                            if (response.HasError)
                            {
                                  
                                return Ok(new { success = false, message = "Failed to Disable account." });
                            }
                            else
                            {
                                Console.WriteLine($"Disable request for user: {userName} was successful");
                                return Ok(new { success = true, message = "Account Successfully disabled." });
                            }
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

    [Route("deleteRequestByUserHash")]
    [HttpPost]
    public async Task<IActionResult> DisableUserAccount([FromBody] List<string> userHashes)
    {
        string? accessToken = HttpContext.Request.Headers["Authorization"];
        if (accessToken != null && accessToken.StartsWith("Bearer "))
        {
              
            accessToken = accessToken.Substring("Bearer ".Length).Trim();
            var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

            if (claimsJson != null)
            {
                var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                if (claims.TryGetValue("Role", out var role) && role == "1" )
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
                            var results = new List<object>();

                            foreach (var userHash in userHashes)
                            {
                                var response = await _accountRecovery. deleteUserRequestByuserHash(userHash);
                                if (response.HasError)
                                {
                                    Console.WriteLine($"Error processing request {userHash}: {response.ErrorMessage}");
                                    results.Add(new { RequestId = userHash, Success = false, Message = response.ErrorMessage });
                                }
                                else
                                {
                                    results.Add(new { RequestId = userHash, Success = true, Message = "Request accepted successfully" });
                                }
                            }
                            return Ok(new { results, newToken });
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
                            // Initialize a list to hold the result for each requestId
                            var results = new List<object>();

                            foreach (var userHash in userHashes)
                            {
                                var response = await _accountRecovery. deleteUserRequestByuserHash(userHash);
                                if (response.HasError)
                                {
                                    Console.WriteLine($"Error processing request {userHash}: {response.ErrorMessage}");
                                    results.Add(new { RequestId = userHash, Success = false, Message = response.ErrorMessage });
                                }
                                else
                                {
                                    results.Add(new { RequestId = userHash, Success = true, Message = "Request accepted successfully" });
                                }
                            }
                            return Ok(results);
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






    

}



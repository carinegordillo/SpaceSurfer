using Microsoft.AspNetCore.Mvc;
using SS.Backend.UserManagement;
using System.Data;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Text.Json;

namespace userProfileAPI.Controllers;

[ApiController]
[Route("api/profile")]
public class userProfileController : ControllerBase
{
    private readonly IProfileModifier _profileModifier;
    private readonly SSAuthService _authService;
    private readonly IConfiguration _config;
    public userProfileController (IProfileModifier ProfileModifier,SSAuthService authService, IConfiguration config){
        _profileModifier = ProfileModifier;
        _authService = authService;
        _config = config;
    }


    [Route("getUserProfile")]
    [HttpGet]
    public async Task<ActionResult<List<UserProfileModel>>> getProfile([FromQuery] string email){
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
                        try
                        {
                              
                            var response = await _profileModifier.getUserProfile(email);

                            if (response.HasError)
                            {
                                Console.WriteLine(response.ErrorMessage);
                                return StatusCode(500, response.ErrorMessage);
                            }

                            List<UserProfileModel> requestList = new List<UserProfileModel>();

                            if (response.ValuesRead != null)
                            {
                                foreach (DataRow row in response.ValuesRead.Rows)
                                {
                    #pragma warning disable CS8601 // Possible null reference assignment.
                                    var userRequest = new UserProfileModel
                                    {
                                        FirstName = Convert.ToString(row["firstName"]),
                                        LastName = Convert.ToString(row["lastName"]),
                                        BackupEmail = Convert.ToString(row["backupEmail"]),
                                        AppRole = Convert.ToInt32(row["appRole"]),
                                    };
                    #pragma warning restore CS8601 // Possible null reference assignment.
            
                                    requestList.Add(userRequest);
                                }
                            }
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

                              
                                var response = await _profileModifier.getUserProfile(email);

                                if (response.HasError)
                                {
                                    Console.WriteLine(response.ErrorMessage);
                                    return StatusCode(500, response.ErrorMessage);
                                }

                                List<UserProfileModel> requestList = new List<UserProfileModel>();

                                if (response.ValuesRead != null)
                                {
                                    foreach (DataRow row in response.ValuesRead.Rows)
                                    {
                        #pragma warning disable CS8601 // Possible null reference assignment.
                                        var userRequest = new UserProfileModel
                                        {
                                            FirstName = Convert.ToString(row["firstName"]),
                                            LastName = Convert.ToString(row["lastName"]),
                                            BackupEmail = Convert.ToString(row["backupEmail"]),
                                            AppRole = Convert.ToInt32(row["appRole"]),
                                        };
                        #pragma warning restore CS8601 // Possible null reference assignment.

                                        requestList.Add(userRequest);
                                    }
                                }

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

    [HttpPost("updateUserProfile")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] EditableUserProfile userProfile)
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
                        try
                        {
                            var response = await _profileModifier.ModifyProfile(userProfile);
            
                                if (response == null)
                                {
                                      
                                    return StatusCode(500, "Internal server error: response is null");
                                }

                                Console.WriteLine($"Response: HasError={response.HasError}, ErrorMessage={response.ErrorMessage}");

                            return Ok(new { response, newToken });
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
                                var response = await _profileModifier.ModifyProfile(userProfile);
            
                                if (response == null)
                                {
                                      
                                    return StatusCode(500, "Internal server error: response is null");
                                }

                                Console.WriteLine($"Response: HasError={response.HasError}, ErrorMessage={response.ErrorMessage}");

                                return Ok(response);
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

    

        /*
    [Route("sendProfileModificationRequest")]
    [HttpPost]
    public async Task<IActionResult> sendRequest ([FromForm] string email, [FromForm] string additionalInformation)
    {

        
        var response = await _profileModifier.createRecoveryRequest(email, additionalInformation);
        if (response.HasError)
        {
            // Log the error or handle it as necessary
            Console.WriteLine($"Error processing recovery request for user: {email}: {response.ErrorMessage}");
            response.ErrorMessage = "Failed to initiate recovery request.";
        }
        else
        {
            // Log the success or handle it as necessary
            Console.WriteLine($"Recovery request for user: {email} was successful");
            response.ErrorMessage = "Recovery request initiated.";

        }
        
        // Return the list of results as JSON 
        return Ok(response);
    }*/

}
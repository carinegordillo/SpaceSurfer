using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.Services;
using SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;
using SS.Backend.Security;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace demoAPI.Controllers;

[ApiController]
[Route("api/SpaceManager")]
public class DemoController : ControllerBase
{

    private readonly ISpaceCreation _spaceCreation;
    private readonly ISpaceModification _spaceModification;
    private readonly SSAuthService _authService;

    public DemoController(ISpaceCreation SpaceCreation, ISpaceModification spaceModification, SSAuthService authService)
    {
        _spaceCreation = SpaceCreation;
        _spaceModification = spaceModification;
        _authService = authService;
    }

    // }  
    [HttpPost]
    [Route("postSpace")]
    public async Task<IActionResult> PostCreateSpace([FromBody] CompanyFloor data, string userHash)
    {
        var response = new Response();
        string message = "";
        //string? accessToken = HttpContext.Request.Headers["Authorization"];
        //if (accessToken != null && accessToken.StartsWith("Bearer "))
        //{
        //    accessToken = accessToken.Substring("Bearer ".Length).Trim();
        //    var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

        //    if (claimsJson != null)
        //    {
        //        var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

        //        if (claims.TryGetValue("Role", out var role) && (role == "1" || role == "2" || role == "3" || role == "4" || role == "5"))
        //        {
        //            bool closeToExpTime = _authService.CheckExpTime(accessToken);
        //            if (closeToExpTime)
        //            {
        //                SSPrincipal principal = new SSPrincipal();
        //                principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
        //                principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
        //                var newToken = _authService.CreateJwt(Request, principal);

        //                try
        //                {
        //                    if (response.HasError)
        //                    {
        //                        return BadRequest(response.ErrorMessage);
        //                    }
        //                    message = "SPACE created successfully!";
        //                    return Ok(new { message , response , newToken});
        //                    response = await _spaceCreation.CreateSpace(userHash, data);
        //                }
        //                catch (Exception ex)
        //                {
        //                    return StatusCode(500, ex.Message);
        //                }
        //            }
        //            else
        //            {
        try
                        {
                            if (response.HasError)
                            {
                                return BadRequest(response.ErrorMessage);
                            }
                            message = "SPACE created successfully!";
                            return Ok(new { message, response });
                            response = await _spaceCreation.CreateSpace(userHash, data);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex.Message);
                        }

        //            }
        //        }
        //        else
        //        {
        //            return BadRequest("Unauthorized role.");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid token.");
        //    }
        //}
        //else
        //{
        //    return BadRequest("Unauthorized. Access token is missing or invalid.");
        //}
    }

    [HttpPost]
    [Route("modifyTimeLimits")]
    public async Task<IActionResult> ModifyTimeLimits([FromBody] Dictionary<string, int> data, string userHash)
    {
        var response = new Response();
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
                    bool closeToExpTime = _authService.CheckExpTime(accessToken);
                    if (closeToExpTime)
                    {
                        SSPrincipal principal = new SSPrincipal();
                        principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
                        principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
                        var newToken = _authService.CreateJwt(Request, principal);

                        try
                        {
                            List<string> messages = new List<string>();

                            foreach (var entry in data)
                            {
                                var spaceID = entry.Key;
                                var newTimeLimit = entry.Value;

                                response = await _spaceModification.ModifyTimeLimit(userHash, spaceID, newTimeLimit);
                                if (response.HasError)
                                {
                                    return BadRequest($"Error modifying time limit for space ID {spaceID}: {response.ErrorMessage}");
                                }
                                else
                                {
                                    messages.Add($"Timelimit for space ID {spaceID} modified successfully!");
                                }
                            }
                            return Ok(new { messages , newToken });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            List<string> messages = new List<string>();

                            foreach (var entry in data)
                            {
                                var spaceID = entry.Key;
                                var newTimeLimit = entry.Value;

                                response = await _spaceModification.ModifyTimeLimit(userHash, spaceID, newTimeLimit);
                                if (response.HasError)
                                {
                                    return BadRequest($"Error modifying time limit for space ID {spaceID}: {response.ErrorMessage}");
                                }
                                else
                                {
                                    messages.Add($"Timelimit for space ID {spaceID} modified successfully!");
                                }
                            }
                            return Ok(new { messages });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex.Message);
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


    //edit floor plan image 
    public class FloorPlanUpdateRequest
    {
        public required string FloorPlanName { get; set; }
        public required byte[] NewFloorPlanImage { get; set; }
    }

    [HttpPost]
    [Route("modifyFloorPlan")]
    public async Task<IActionResult> ModifyFloorImage([FromBody] FloorPlanUpdateRequest data, string userHash)
    {
        var response = new Response();
        string message = "";
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
                    bool closeToExpTime = _authService.CheckExpTime(accessToken);
                    if (closeToExpTime)
                    {
                        SSPrincipal principal = new SSPrincipal();
                        principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
                        principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
                        var newToken = _authService.CreateJwt(Request, principal);

                        try
                        {
                            response = await _spaceModification.ModifyFloorImage(userHash, data.FloorPlanName, data.NewFloorPlanImage);
                            if (response.HasError)
                            {
                                return BadRequest($"Error modifying floor plan image for floor plan {data.FloorPlanName}: {response.ErrorMessage}");
                            }
                            message = "Floor Plan replaced successfully!";
                            return Ok(new { message, response , newToken });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            response = await _spaceModification.ModifyFloorImage(userHash, data.FloorPlanName, data.NewFloorPlanImage);
                            if (response.HasError)
                            {
                                return BadRequest($"Error modifying floor plan image for floor plan {data.FloorPlanName}: {response.ErrorMessage}");
                            }
                            message = "Floor Plan replaced successfully!";
                            return Ok(new { message, response });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex.Message);
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
    //delete space 

    public class SpaceIdModel
    {
        public required string SpaceID { get; set; }
    }

    [HttpPost]
    [Route("deleteSpace")]
    public async Task<IActionResult> DeleteSpaceID([FromBody] SpaceIdModel data, string userHash)
    {
        var response = new Response();
        string message = "";
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
                    bool closeToExpTime = _authService.CheckExpTime(accessToken);
                    if (closeToExpTime)
                    {
                        SSPrincipal principal = new SSPrincipal();
                        principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
                        principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
                        var newToken = _authService.CreateJwt(Request, principal);

                        try
                        {
                            response = await _spaceModification.DeleteSpace(userHash, data.SpaceID);
                            if (response.HasError)
                            {
                                return BadRequest($"Error Deleting Space for spaceID {data.SpaceID}: {response.ErrorMessage}");
                            }
                            message = "SpaceID deleted successfully!";
                            return Ok(new { message, response , newToken });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            response = await _spaceModification.DeleteSpace(userHash, data.SpaceID);
                            if (response.HasError)
                            {
                                return BadRequest($"Error Deleting Space for spaceID {data.SpaceID}: {response.ErrorMessage}");
                            }
                            message = "SpaceID deleted successfully!";
                            return Ok(new { message, response });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex.Message);
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

    public class FloorPlanModel
    {
        public required string FloorPlanName { get; set; }
    }

    [HttpPost]
    [Route("deleteFloor")]
    public async Task<IActionResult> DeleteFloor([FromBody] FloorPlanModel data, string userHash)
    {
        var response = new Response();
        string message = "";
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
                    bool closeToExpTime = _authService.CheckExpTime(accessToken);
                    if (closeToExpTime)
                    {
                        SSPrincipal principal = new SSPrincipal();
                        principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
                        principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
                        var newToken = _authService.CreateJwt(Request, principal);

                        try
                        {
                            response = await _spaceModification.DeleteFloor(userHash, data.FloorPlanName);
                            if (response.HasError)
                            {
                                return BadRequest($"Error Deleting Space for spaceID {data.FloorPlanName}: {response.ErrorMessage}");
                            }
                        message = "SpaceID deleted successfully!";
                            return Ok(new { message, response , newToken });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            response = await _spaceModification.DeleteFloor(userHash, data.FloorPlanName);
                            if (response.HasError)
                            {
                                return BadRequest($"Error Deleting Space for spaceID {data.FloorPlanName}: {response.ErrorMessage}");
                            }
                            message = "SpaceID deleted successfully!";
                            return Ok(new { message, response });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex.Message);
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
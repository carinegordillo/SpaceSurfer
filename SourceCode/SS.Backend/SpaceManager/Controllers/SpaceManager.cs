using Microsoft.AspNetCore.Mvc;
using System.Data;
// using SS.Backend.Services;
using SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;
using SS.Backend.Security;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace demoAPI.Controllers;

[ApiController]
[Route("api/SpaceManager")]
public class DemoController : ControllerBase
{

    private readonly ISpaceCreation _spaceCreation;
    private readonly ISpaceModification _spaceModification;
    private readonly SSAuthService _authService;

    public DemoController (ISpaceCreation SpaceCreation, ISpaceModification spaceModification, SSAuthService authService){
        _spaceCreation = SpaceCreation;
        _spaceModification = spaceModification;
        _authService = authService;
    }



    // [HttpPost]
    // [Route("postSpace")]
    // // public async Task<ActionResult<List<UserInfo>>> PostCreateAccount([FromBody] UserInfo userInfo){
    // public async Task<IActionResult> PostCreateSpace([FromBody] CompanyFloor companyFloor, string accessToken){
    //     // string accessToken = "123";
    //     // List<string> info = _authService.GetRolesFromToken(accessToken);
      
    //     string userHash = _authService.GetSubjectFromToken(accessToken);
        
    //     // string dummyHash = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";
        
    //     var response = await _spaceCreation.CreateSpace(userHash, companyFloor);
    //     if (response.HasError)
    //     {
    //         return BadRequest(response.ErrorMessage);
    //     }
    //     return Ok(new { message = "SPACE created successfully!" + response});
    //     // return Ok(response);
    // }  
    [HttpPost]
    [Route("postSpace")]
    public async Task<IActionResult> PostCreateSpace([FromBody] CompanyFloor companyFloor){
        // Extract the bearer token from the Authorization header
        var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
        if(string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Authorization header is missing or not valid.");
        }

        // Remove the 'Bearer ' prefix to get the actual token
        var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();

        // Now you can use the token as before
        string userHash = _authService.GetSubjectFromToken(accessToken);
        
        var response = await _spaceCreation.CreateSpace(userHash, companyFloor);
        if (response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }
        return Ok(new { message = "SPACE created successfully!" + response});
    }

    [HttpPost]
    [Route("modifyTimeLimits")]
    public async Task<IActionResult> ModifyTimeLimits([FromBody] Dictionary<string, int> spaceTimeLimits)
    {
    
        var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
        if(string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Authorization header is missing or not valid.");
        }
        var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
        string userHash = _authService.GetSubjectFromToken(accessToken);

        List<string> messages = new List<string>();

        foreach (var entry in spaceTimeLimits)
        {
            var spaceID = entry.Key;
            var newTimeLimit = entry.Value;
            
            var response = await _spaceModification.ModifyTimeLimit(userHash, spaceID, newTimeLimit);
            if (response.HasError)
            {
                return BadRequest($"Error modifying time limit for space ID {spaceID}: {response.ErrorMessage}");
            }
            else
            {
                messages.Add($"Timelimit for space ID {spaceID} modified successfully!");
            }
        }
        return Ok(new { messages = messages });
    }


    //edit floor plan image 
    public class FloorPlanUpdateRequest
    {
        public required string FloorPlanName { get; set; }
        public required byte[] NewFloorPlanImage { get; set; }
    }

    [HttpPost]
    [Route("modifyFloorPlan")]
    public async Task<IActionResult> ModifyFloorImage([FromBody] FloorPlanUpdateRequest request)
    {
        // Extract the bearer token from the Authorization header
        var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
        if(string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Authorization header is missing or not valid.");
        }
        
        // Remove the 'Bearer ' prefix to get the actual token
        var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
        // Extract user hash or other relevant data from the token
        string userHash = _authService.GetSubjectFromToken(accessToken);
        
        // Proceed with modifying the floor plan using the extracted user hash
        var response = await _spaceModification.ModifyFloorImage(userHash, request.FloorPlanName, request.NewFloorPlanImage);
        if (response.HasError)
        {
            return BadRequest($"Error modifying floor plan image for floor plan {request.FloorPlanName}: {response.ErrorMessage}");
        }
        return Ok(new { message = "Floor Plan replaced successfully!" + response});
    }
    //delete space 

    public class SpaceIdModel
    {
        public required string SpaceID { get; set; }
    }

    [HttpPost]
    [Route("deleteSpace")]
    public async Task<IActionResult> DeleteSpaceID([FromBody] SpaceIdModel model)
    {
        // Assuming dummyCompanyID is fetched or defined elsewhere
        var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
        if(string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Authorization header is missing or not valid.");
        }
        
        // Remove the 'Bearer ' prefix to get the actual token
        var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
        // Extract user hash or other relevant data from the token
        string userHash = _authService.GetSubjectFromToken(accessToken);
        
        var response = await _spaceModification.DeleteSpace(userHash, model.SpaceID);
        if (response.HasError)
        {
            return BadRequest($"Error Deleting Space for spaceID {model.SpaceID}: {response.ErrorMessage}");
        }
        
        return Ok(new { message = "SpaceID deleted successfully!" + response});
    }

    public class FloorPlanModel
    {
        public required string FloorPlanName { get; set; }
    }

    [HttpPost]
    [Route("deleteFloor")]
    public async Task<IActionResult> DeleteFloor([FromBody] FloorPlanModel model)
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
        if(string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Authorization header is missing or not valid.");
        }
        
        // Remove the 'Bearer ' prefix to get the actual token
        var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
        // Extract user hash or other relevant data from the token
        string userHash = _authService.GetSubjectFromToken(accessToken);
        
        var response = await _spaceModification.DeleteFloor(userHash, model.FloorPlanName);
        if (response.HasError)
        {
            return BadRequest($"Error Deleting Space for spaceID {model.FloorPlanName}: {response.ErrorMessage}");
        }
        
        return Ok(new { message = "SpaceID deleted successfully!" + response});
    }
}
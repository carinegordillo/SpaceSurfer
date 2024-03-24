using Microsoft.AspNetCore.Mvc;
using System.Data;
// using SS.Backend.Services;
using SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;
using SS.Backend.Security;
using Microsoft.IdentityModel.JsonWebTokens;

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


    [HttpPost]
    [Route("postSpace")]
    // public async Task<ActionResult<List<UserInfo>>> PostCreateAccount([FromBody] UserInfo userInfo){
    public async Task<IActionResult> PostCreateSpace([FromBody] CompanyFloor companyFloor){
        string accessToken = "123";
        List<string> info = _authService.GetRolesFromToken(accessToken);
        
        string dummyHash = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";
        
        var response = await _spaceCreation.CreateSpace(dummyHash, companyFloor);
        if (response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }
        return Ok(new { message = "SPACE created successfully!" + response});
        // return Ok(response);
    }  

    [HttpPost]
    [Route("modifyTimeLimits")]
    public async Task<IActionResult> ModifyTimeLimits([FromBody] Dictionary<string, int> spaceTimeLimits)
    {
        // Assuming dummyCompanyID is fetched or defined elsewhere
        string dummyHash = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";
        List<string> messages = new List<string>();

        foreach (var entry in spaceTimeLimits)
        {
            var spaceID = entry.Key;
            var newTimeLimit = entry.Value;
            
            var response = await _spaceModification.ModifyTimeLimit(dummyHash, spaceID, newTimeLimit);
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
        // Assuming dummyCompanyID is fetched or defined elsewhere
        string dummyHash = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";
        
        var response = await _spaceModification.ModifyFloorImage(dummyHash, request.FloorPlanName, request.NewFloorPlanImage);
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
        string dummyHash = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";
        
        var response = await _spaceModification.DeleteSpace(dummyHash, model.SpaceID);
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
        // Assuming dummyCompanyID is fetched or defined elsewhere
        string dummyHash = "/5WhbnBQfb39sAFdKIfsqr8Rt0D6fSi6CoCC+7qbeeI=      ";
        
        var response = await _spaceModification.DeleteFloor(dummyHash, model.FloorPlanName);
        if (response.HasError)
        {
            return BadRequest($"Error Deleting Space for spaceID {model.FloorPlanName}: {response.ErrorMessage}");
        }
        
        return Ok(new { message = "SpaceID deleted successfully!" + response});
    }
}
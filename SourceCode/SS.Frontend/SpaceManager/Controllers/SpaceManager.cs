using Microsoft.AspNetCore.Mvc;
using System.Data;
// using SS.Backend.Services;
using SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;

namespace demoAPI.Controllers;

[ApiController]
[Route("api/SpaceManager")]
public class DemoController : ControllerBase
{

    private readonly ISpaceCreation _spaceCreation;
    private readonly ISpaceModification _spaceModification;
    public DemoController (ISpaceCreation SpaceCreation, ISpaceModification spaceModification){
        _spaceCreation = SpaceCreation;
        _spaceModification = spaceModification;
    }
    
    // [Route("createSpace")]
    // [HttpGet]
    // public async Task<ActionResult<List<UserInfo>>> GetAllRequests(){

    //     var response = await _spaceCreation.ReadUserTable("dbo.companyFloor");
    //     List<CompanyFloor> spaceList = new List<CompanyFloor>();

    //     try{

    //         if (response.HasError)
    //         {
    //             Console.WriteLine(response.ErrorMessage);
    //             return StatusCode(500, response.ErrorMessage);
    //         }
            
    //             if (response.ValuesRead != null)
    //             {
    //                 foreach (DataRow row in response.ValuesRead.Rows)
    //                 {
    //                     spaceList.Add(new CompanyFloor
    //                     {
    //                         FloorPlanName = row["floorPlanName"].ToString(),
    //                         FloorPlanImage = row["floorPlanImage"], 
    //                         FloorSpaces = row["FloorSpaces"]
    //                     });
    //                 }
    //             }

    //             return Ok(spaceList);

    //         foreach (var spaceInfo in spaceList){
    //             Console.WriteLine($"Name: {spaceInfo.FloorPlanName}, Spaces: {spaceInfo.FloorSpaces}");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, $"An error occurred: {ex.Message}");
    //     }

    //     return Ok(spaceList);
    // }

    [HttpPost]
    [Route("postSpace")]
    // public async Task<ActionResult<List<UserInfo>>> PostCreateAccount([FromBody] UserInfo userInfo){
    public async Task<IActionResult> PostCreateSpace([FromBody] CompanyFloor companyFloor){
        
        string dummyHash = "12345";
        
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
        string dummyHash = "12345";
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
        public string FloorPlanName { get; set; }
        public byte[] NewFloorPlanImage { get; set; }
    }

    [HttpPost]
    [Route("modifyFloorPlan")]
    public async Task<IActionResult> ModifyFloorImage([FromBody] FloorPlanUpdateRequest request)
    {
        // Assuming dummyCompanyID is fetched or defined elsewhere
        string dummyHash = "12345";
        
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
        public string SpaceID { get; set; }
    }

    [HttpPost]
    [Route("deleteSpace")]
    public async Task<IActionResult> DeleteSpaceID([FromBody] SpaceIdModel model)
    {
        // Assuming dummyCompanyID is fetched or defined elsewhere
        string dummyHash = "12345";
        
        var response = await _spaceModification.DeleteSpace(dummyHash, model.SpaceID);
        if (response.HasError)
        {
            return BadRequest($"Error Deleting Space for spaceID {model.SpaceID}: {response.ErrorMessage}");
        }
        
        return Ok(new { message = "SpaceID deleted successfully!" + response});
    }

    public class FloorPlanModel
    {
        public string FloorPlanName { get; set; }
    }

    [HttpPost]
    [Route("deleteFloor")]
    public async Task<IActionResult> DeleteFloor([FromBody] FloorPlanModel model)
    {
        // Assuming dummyCompanyID is fetched or defined elsewhere
        string dummyHash = "12345";
        
        var response = await _spaceModification.DeleteFloor(dummyHash, model.FloorPlanName);
        if (response.HasError)
        {
            return BadRequest($"Error Deleting Space for spaceID {model.FloorPlanName}: {response.ErrorMessage}");
        }
        
        return Ok(new { message = "SpaceID deleted successfully!" + response});
    }
}
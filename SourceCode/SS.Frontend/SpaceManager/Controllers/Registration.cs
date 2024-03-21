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
    public DemoController (ISpaceCreation SpaceCreation){
        _spaceCreation = SpaceCreation;
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
        
        CompanyInfo dummyCompanyInfo = new CompanyInfo
        {
            companyName = "Dummy Company Name",
            address = "123 Dummy Street, Dummville",
            openingHours = "09:00",
            closingHours = "17:00",
            daysOpen = "Monday,Tuesday,Wednesday,Thursday,Friday"
        };
        
        var response = await _spaceCreation.CreateSpace(dummyCompanyInfo, companyFloor);
        if (response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }
        return Ok(new { message = "Account created successfully!" + response});
        // return Ok(response);
    }  
}
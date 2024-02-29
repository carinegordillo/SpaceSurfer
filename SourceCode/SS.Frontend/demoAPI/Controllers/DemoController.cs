using Microsoft.AspNetCore.Mvc;
using SS.Backend.UserManagement;

namespace demoAPI.Controllers;

[ApiController]
[Route("api/employees")]
public class DemoController : ControllerBase
{

    private readonly IAccountRecovery _accountRecovery;
    public DemoController (IAccountRecovery AccountRecoveryNoInj){
        _accountRecovery = AccountRecoveryNoInj;
    }
    
    [Route("getAllDummyRequests")]
    [HttpGet]
    public async Task<ActionResult<List<Employee>>> GetAllRequests(){

        var response = await _accountRecovery.ReadDummyTable();

        if (response.HasError)
        {
            Console.WriteLine(response.ErrorMessage);
            return StatusCode(500, response.ErrorMessage);
        }

        var requests = response.ValuesRead.Select(row => new Employee
        {
            Name = row[0].ToString(), 
            Position = row[1].ToString()
        }).ToList();

        foreach (var employee in requests){
            Console.WriteLine($"Name: {employee.Name}, Position: {employee.Position}");
        }

        return Ok(requests);
    }


    [HttpPost]
    [Route("postDummyRequest")]
    public async Task<ActionResult<List<Employee>>> PostDummyRequest([FromForm] string name, [FromForm] string position){
        var response = await _accountRecovery.sendDummyRequest(name, position);
        return Ok(response);
    }
   
}

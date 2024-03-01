using Microsoft.AspNetCore.Mvc;
using System.Data;
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
        List<Employee> employeeList = new List<Employee>();

        try{

            if (response.HasError)
            {
                Console.WriteLine(response.ErrorMessage);
                return StatusCode(500, response.ErrorMessage);
            }

            
                if (response.ValuesRead != null)
                {
                    foreach (DataRow row in response.ValuesRead.Rows)
                    {
                        employeeList.Add(new Employee
                        {
                            Name = row["Name"].ToString(),
                            Position = row["Position"].ToString(),
                        });
                    }
                }

                return Ok(employeeList);

            foreach (var employee in employeeList){
                Console.WriteLine($"Name: {employee.Name}, Position: {employee.Position}");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

        return Ok(employeeList);
    }


    [HttpPost]
    [Route("postDummyRequest")]
    public async Task<ActionResult<List<Employee>>> PostDummyRequest([FromForm] string employeeName, [FromForm] string position){
        var response = await _accountRecovery.sendDummyRequest(employeeName, position);
        return Ok(response);
    }
   
}

using Microsoft.AspNetCore.Mvc;
using  AccountManagement.Models;
using SS.Backend.UserManagement;

namespace AccountManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class RecoverRequestController : ControllerBase
{
    private readonly IAccountStatusModifier _accountStatusModifier;
    public RecoverRequestController (IAccountStatusModifier accountStatusModifier){
        _accountStatusModifier = accountStatusModifier;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<RequestsModel>>> GetAllRequests(){

        var response = await _accountStatusModifier.ReadRecoveryRequests();

        if (response.HasError)
        {
            Console.WriteLine(response.ErrorMessage);
            return StatusCode(500, response.ErrorMessage);
        }
        // add a whole bunch of logging statements

        var requests = response.ValuesRead.Select(row => new RequestsModel
        {
            userID = (string)row[0],
            accountStatus = (string)row[1]
        }).ToList();

        return Ok(requests);
    }
   
}

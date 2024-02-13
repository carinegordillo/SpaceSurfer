using Microsoft.AspNetCore.Mvc;
using SS.Backend.UserManagement;

namespace AccountManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class RecoverRequestController : ControllerBase
{

    private readonly IAccountRecovery _accountRecovery;
    public RecoverRequestController (IAccountRecovery AccountRecoveryNoInj){
        _accountRecovery = AccountRecoveryNoInj;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<UserRequestModel>>> GetAllRequests(){

        var response = await _accountRecovery.ReadUserRequests();

        if (response.HasError)
        {
            Console.WriteLine(response.ErrorMessage);
            return StatusCode(500, response.ErrorMessage);
        }

        var requests = response.ValuesRead.Select(row => new UserRequestModel
        {
            RequestId = (int)row[0],
    UserHash = (string)row[1],
    RequestDate = (DateTime)row[2],
    Status = (string)row[3],
    RequestType = (string)row[4],
    ResolveDate = row[5] != DBNull.Value ? DateTime.Parse((string)row[5]) : (DateTime?)null,
    AdditionalInformation = row[6] != DBNull.Value ? (string)row[6] : null
        }).ToList();

        return Ok(requests);
    }
   
}

using Microsoft.AspNetCore.Mvc;

using  AccountManagement.Models;

namespace AccountManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class RecoverRequestController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RequestsModel>>> GetAllRequests(){
        var requests = new List<RequestsModel>{
            new RequestsModel{
                userID = "123214",
                accountStatus = "enabled"
            }
        };
        return Ok(requests);
    }
   
}

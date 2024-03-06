using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.UserManagement;
using SS.Backend.DataAccess;

namespace AccountManagement.Controllers;

[ApiController]
[Route("api/requestRecovery")]
public class RecoverRequestController : ControllerBase
{
    private readonly IAccountRecovery _accountRecovery;
    public RecoverRequestController (IAccountRecovery AccountRecovery){
        _accountRecovery = AccountRecovery;
    }


    [Route("sendRecoveryRequest")]
    [HttpPost]
    public async Task<IActionResult> sendRecoveryRequest ([FromForm] string email, [FromForm] string additionalInformation)
    {

        
        var response = await _accountRecovery.createRecoveryRequest(email, additionalInformation);
        if (response.HasError)
        {
            // Log the error or handle it as necessary
            Console.WriteLine($"Error processing recovery request for user: {email}: {response.ErrorMessage}");
            response.ErrorMessage = "Failed to initiate recovery request.";
        }
        else
        {
            // Log the success or handle it as necessary
            Console.WriteLine($"Recovery request for user: {email} was successful");
            response.ErrorMessage = "Recovery request initiated.";

        }
        
        // Return the list of results as JSON 
        return Ok(response);
    }
}

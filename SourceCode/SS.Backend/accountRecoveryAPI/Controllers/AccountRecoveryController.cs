using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.UserManagement;
using SS.Backend.DataAccess;

using SS.Backend.SharedNamespace;
using SS.Backend.Services.LoggingService;


namespace AccountManagement.Controllers;

[ApiController]
[Route("api/requestRecovery")]
public class RecoverRequestController : ControllerBase
{
    private readonly IAccountRecovery _accountRecovery;
    private IAccountDisabler _accountDisabler;

    public RecoverRequestController (IAccountRecovery AccountRecovery, IAccountDisabler AccountDisabler){
        _accountRecovery = AccountRecovery;
        _accountDisabler = AccountDisabler;
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

    [Route("getUserAccountDetails")]
    [HttpGet]
    public async Task<ActionResult> getUserAccountDetails([FromQuery] string email)
    {
        try
        {
            var userAccountDetails = await _accountRecovery.ReadUserAccount(email);

            if (userAccountDetails == null)
            {
                return NotFound("User not found.");
            }

            return Ok(userAccountDetails);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }





    

}

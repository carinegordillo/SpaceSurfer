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

    [Route("getAllRequests")]
    [HttpGet]
    public async Task<ActionResult<List<UserRequestModel>>> GetAllRequests(){

        var response = await _accountRecovery.ReadUserRequests();

        if (response.HasError)
        {
            Console.WriteLine(response.ErrorMessage);
            return StatusCode(500, response.ErrorMessage);
        }

        List<UserRequestModel> requestList = new List<UserRequestModel>();

        if (response.ValuesRead != null)
        {
            foreach (DataRow row in response.ValuesRead.Rows)
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                var userRequest = new UserRequestModel
                {
                    RequestId = Convert.ToInt32(row["request_id"]),
                    UserHash = Convert.ToString(row["userHash"]),
                    RequestDate = Convert.ToDateTime(row["requestDate"]),
                    Status = Convert.ToString(row["status"]),
                    RequestType = Convert.ToString(row["requestType"]),
                    ResolveDate = row["resolveDate"] != DBNull.Value ? Convert.ToDateTime(row["resolveDate"]) : (DateTime?)null,
                    AdditionalInformation = row["additionalInformation"] != DBNull.Value ? Convert.ToString(row["additionalInformation"]) : null
                };
#pragma warning restore CS8601 // Possible null reference assignment.
                requestList.Add(userRequest);
            }
        }

        return Ok(requestList);
    }

    [Route("acceptRequests")]
    [HttpPost]
    public async Task<IActionResult> AcceptRequests([FromBody] List<string> userHashes)
    {
        // Initialize a list to hold the result for each requestId
        var results = new List<object>();

        foreach (var requestId in userHashes)
        {
            var response = await _accountRecovery.RecoverAccount(requestId, true);
            if (response.HasError)
            {
                Console.WriteLine($"Error processing request {requestId}: {response.ErrorMessage}");
                results.Add(new { RequestId = requestId, Success = false, Message = response.ErrorMessage });
            }
            else
            {
                results.Add(new { RequestId = requestId, Success = true, Message = "Request accepted successfully" });
            }
        }
        // Return the list of results as JSON 
        return Ok(results);
    }

    [Route("denyRequests")]
    [HttpPost]
    public async Task<IActionResult> DenyRequests([FromBody] List<string> userHashes)
    {
        // Initialize a list to hold the result for each requestId
        var results = new List<object>();

        foreach (var requestId in userHashes)
        {
            var response = await _accountRecovery.RecoverAccount(requestId, false);
            if (response.HasError)
            {
                Console.WriteLine($"Error processing request {requestId}: {response.ErrorMessage}");
                results.Add(new { RequestId = requestId, Success = false, Message = response.ErrorMessage });
            }
            else
            {
                results.Add(new { RequestId = requestId, Success = true, Message = "Request accepted successfully" });
            }
        }
        // Return the list of results as JSON 
        return Ok(results);
    }

    [Route("disableAccount")]
    [HttpPost]
    public async Task<IActionResult> DisableUserAccount([FromBody] string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return BadRequest("User name must be provided.");
        }

        var response = await _accountDisabler.DisableAccount(userName);
        Console.WriteLine(response.HasError);
        if (response.HasError)
        {
            Console.WriteLine("Error disabling account");
            return Ok(new { success = false, message = "Failed to Disable account." });
        }
        else
        {
            Console.WriteLine($"Disable request for user: {userName} was successful");
            return Ok(new { success = true, message = "Account Successfully disabled." });
        }
    }

    [Route("deleteRequestByUserHash")]
    [HttpPost]
    public async Task<IActionResult> DisableUserAccount([FromBody] List<string> userHashes)
    {
        var results = new List<object>();

        foreach (var userHash in userHashes)
        {
            var response = await _accountRecovery. deleteUserRequestByuserHash(userHash);
            if (response.HasError)
            {
                Console.WriteLine($"Error processing request {userHash}: {response.ErrorMessage}");
                results.Add(new { RequestId = userHash, Success = false, Message = response.ErrorMessage });
            }
            else
            {
                results.Add(new { RequestId = userHash, Success = true, Message = "Request accepted successfully" });
            }
        }
        return Ok(results);
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

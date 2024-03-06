using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.UserManagement;

namespace userProfileAPI.Controllers;

[ApiController]
[Route("api/profile")]
public class userProfileController : ControllerBase
{
    private readonly IProfileModifier _accountRecovery;
    public userProfileController (IProfileModifier ProfileModifier){
        _accountRecovery = ProfileModifier;
    }


    [Route("getUserProfile")]
    [HttpGet]
    public async Task<ActionResult<List<UserProfileModel>>> getProfile([FromQuery] string email){

        var response = await _accountRecovery.getUserProfile(email);

        if (response.HasError)
        {
            Console.WriteLine(response.ErrorMessage);
            return StatusCode(500, response.ErrorMessage);
        }

        List<UserProfileModel> requestList = new List<UserProfileModel>();

        if (response.ValuesRead != null)
        {
            foreach (DataRow row in response.ValuesRead.Rows)
            {
                var userRequest = new UserProfileModel
                {
                    FirstName = Convert.ToString(row["firstName"]),
                    LastName = Convert.ToString(row["lastName"]),
                    BackupEmail = Convert.ToString(row["backupEmail"]),
                    AppRole = Convert.ToInt32(row["appRole"]),
                };
                requestList.Add(userRequest);
            }
        }

        return Ok(requestList);
    }

        /*
    [Route("sendProfileModificationRequest")]
    [HttpPost]
    public async Task<IActionResult> sendRequest ([FromForm] string email, [FromForm] string additionalInformation)
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
    }*/

}
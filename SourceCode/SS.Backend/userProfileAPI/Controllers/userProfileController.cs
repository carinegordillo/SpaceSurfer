using Microsoft.AspNetCore.Mvc;
using SS.Backend.UserManagement;
using System.Data;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Text;

namespace userProfileAPI.Controllers;

[ApiController]
[Route("api/profile")]
public class userProfileController : ControllerBase
{
    private readonly IProfileModifier _profileModifier;
    public userProfileController (IProfileModifier ProfileModifier){
        _profileModifier = ProfileModifier;
    }


    [Route("getUserProfile")]
    [HttpGet]
    public async Task<ActionResult<List<UserProfileModel>>> getProfile([FromQuery] string email){

        var response = await _profileModifier.getUserProfile(email);

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
#pragma warning disable CS8601 // Possible null reference assignment.
                var userRequest = new UserProfileModel
                {
                    FirstName = Convert.ToString(row["firstName"]),
                    LastName = Convert.ToString(row["lastName"]),
                    BackupEmail = Convert.ToString(row["backupEmail"]),
                    AppRole = Convert.ToInt32(row["appRole"]),
                };
#pragma warning restore CS8601 // Possible null reference assignment.
                requestList.Add(userRequest);
            }
        }

        return Ok(requestList);
    }

    [HttpPost("updateUserProfile")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] EditableUserProfile userProfile)
    {
        Console.WriteLine("UpdateUserProfile called");

        try
        {
            var response = await _profileModifier.ModifyProfile(userProfile);
            
            if (response == null)
            {
                Console.WriteLine("ModifyProfile returned null");
                return StatusCode(500, "Internal server error: response is null");
            }

            Console.WriteLine($"Response: HasError={response.HasError}, ErrorMessage={response.ErrorMessage}");

            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    

        /*
    [Route("sendProfileModificationRequest")]
    [HttpPost]
    public async Task<IActionResult> sendRequest ([FromForm] string email, [FromForm] string additionalInformation)
    {

        
        var response = await _profileModifier.createRecoveryRequest(email, additionalInformation);
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
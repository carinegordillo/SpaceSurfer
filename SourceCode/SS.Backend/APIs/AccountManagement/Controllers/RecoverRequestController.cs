using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.UserManagement;

namespace AccountManagement.Controllers;

[ApiController]
[Route("api/requests")]
public class RecoverRequestController : ControllerBase
{

    private readonly IAccountRecovery _accountRecovery;
    public RecoverRequestController (IAccountRecovery AccountRecoveryNoInj){
        _accountRecovery = AccountRecoveryNoInj;
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
                var userRequest = new UserRequestModel
                {
                    RequestId = Convert.ToInt32(row["request_id"]),
                    UserHash = Convert.ToString(row["userHash"]),
                    RequestDate = Convert.ToDateTime(row["requestDate"]),
                    Status = Convert.ToString(row["status"]),
                    RequestType = Convert.ToString(row["requestType"]),
                    // Check if resolveDate is DBNull and only then parse it to DateTime
                    ResolveDate = row["resolveDate"] != DBNull.Value ? Convert.ToDateTime(row["resolveDate"]) : (DateTime?)null,
                    // Check if additionalInformation is DBNull before converting
                    AdditionalInformation = row["additionalInformation"] != DBNull.Value ? Convert.ToString(row["additionalInformation"]) : null
                };
                requestList.Add(userRequest);
            }
        }

        return Ok(requestList);
    }

    [Route("getPendingRequests")]
    [HttpGet]
    public async Task<ActionResult<List<UserRequestModel>>> GetAllPendingRequests(){

        var response = await _accountRecovery.ReadUserPendingRequests();

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
                var userRequest = new UserRequestModel
                {
                    RequestId = Convert.ToInt32(row["request_id"]),
                    UserHash = Convert.ToString(row["userHash"]),
                    RequestDate = Convert.ToDateTime(row["requestDate"]),
                    Status = Convert.ToString(row["status"]),
                    RequestType = Convert.ToString(row["requestType"]),
                    // Check if resolveDate is DBNull and only then parse it to DateTime
                    ResolveDate = row["resolveDate"] != DBNull.Value ? Convert.ToDateTime(row["resolveDate"]) : (DateTime?)null,
                    // Check if additionalInformation is DBNull before converting
                    AdditionalInformation = row["additionalInformation"] != DBNull.Value ? Convert.ToString(row["additionalInformation"]) : null
                };
                requestList.Add(userRequest);
            }
        }

        return Ok(requestList);
    }


    [Route("acceptRequests")]
    [HttpPost]
    public async Task<IActionResult> AcceptRequests([FromBody] List<string> requestIds)
    {
        // Initialize a list to hold the result for each requestId
        var results = new List<object>();

        foreach (var requestId in requestIds)
        {
            var response = await _accountRecovery.RecoverAccount(requestId, true);
            if (response.HasError)
            {
                // Log the error or handle it as necessary
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

   
}

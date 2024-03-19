using Microsoft.AspNetCore.Mvc;
using System.Data;
// using SS.Backend.Services;
using SS.Backend.Services.AccountCreationService;
using SS.Backend.SharedNamespace;

namespace demoAPI.Controllers;

[ApiController]
[Route("api/registration")]
public class DemoController : ControllerBase
{

    private readonly IAccountCreation _accountCreation;
    public DemoController (IAccountCreation AccountCreation){
        _accountCreation = AccountCreation;
    }
    
    [Route("createAccount")]
    [HttpGet]
    public async Task<ActionResult<List<UserInfo>>> GetAllRequests(){

        var response = await _accountCreation.ReadUserTable("dbo.userProfile");
        List<UserInfo> userList = new List<UserInfo>();

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
                        userList.Add(new UserInfo
                        {
                            username = row["hashedUsername"].ToString(),
                            firstname = row["firstName"].ToString(), 
                            lastname = row["lastName"].ToString(), 
                            backupEmail = row["backupEmail"] != DBNull.Value ? Convert.ToString(row["backupEmail"]) : null,
                            // AdditionalInformation = row["additionalInformation"] != DBNull.Value ? Convert.ToString(row["additionalInformation"]) : null
                            role = Convert.ToInt32(row["appRole"]) 
                        });
                    }
                }

                return Ok(userList);

            foreach (var userInfo in userList){
                Console.WriteLine($"Name: {userInfo.firstname}, Position: {userInfo.lastname}");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

        return Ok(userList);
    }

    [HttpPost]
    [Route("postAccount")]
    // public async Task<ActionResult<List<UserInfo>>> PostCreateAccount([FromBody] UserInfo userInfo){
    public async Task<IActionResult> PostCreateAccount([FromBody] AccountCreationRequest request){
        var response = await _accountCreation.CreateUserAccount(request.UserInfo, request.CompanyInfo);
        if (response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }
        return Ok(new { message = "Account created successfully!", response });
    }  
}
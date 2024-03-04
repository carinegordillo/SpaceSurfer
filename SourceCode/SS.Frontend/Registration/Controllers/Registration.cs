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

        // var response = await _accountCreation.ReadUserTable("dbo.userProfile");

        // if (response.HasError)
        // {
        //     Console.WriteLine(response.ErrorMessage);
        //     return StatusCode(500, response.ErrorMessage);
        // }

        // var requests = response.ValuesRead.Select(row => new UserInfo
        // {
        //     username = "dummyusername@hotmail.com",
        //     firstname = row[1].ToString(), 
        //     lastname = "CONTROLLER", 
        //     backupEmail = "CONTROLLER@hotmail.com", 
        //     role = 1
        // }).ToList();



        // foreach (var userInfo in requests){
        //     Console.WriteLine($"Name: {userInfo.firstname}, Position: {userInfo.lastname}");
        // }

        // return Ok(requests);




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
    [Route("postDummyRequest")]
    public async Task<ActionResult<List<UserInfo>>> PostDummyRequest([FromForm] UserInfo userInfo){
        var response = await _accountCreation.CreateUserAccount(userInfo);
        return Ok(response);
    }
   
}


// using Microsoft.AspNetCore.Mvc;
// using SS.Backend.SharedNamespace;
// using SS.Backend.Services.AccountCreationService;

// namespace Registration.Controllers
// {
//     [ApiController]
//     [Route("api/registration")] // Simplified route for clarity
//     public class RegistrationController : ControllerBase
//     {
//         private readonly IAccountCreation _accountCreation;

//         public RegistrationController(IAccountCreation accountCreation)
//         {
//             _accountCreation = accountCreation;
//         }

//         // This endpoint might be for testing or a different functionality
//         // Ensure it has a unique route and method name
//         [HttpGet("getAllDummyRequests")]
//         public async Task<ActionResult<List<UserInfo>>> GetAllDummyRequests()
//         {
//             // Implementation for fetching dummy requests
//             // This is just a placeholder response for demonstration
//             return Ok(new List<UserInfo>());
//         }

//         // Endpoint for creating a new user account
//         [HttpPost("postDummyRequest")]
//         public async Task<ActionResult> CreateNewUserAccount([FromBody] UserInfo userInfo)
//         {
//             // Assuming userInfo is sent as JSON in the request body
//             string pepper = "DA0b";
//             Hashing hashing = new Hashing();
//             UserPepper userPepper = new UserPepper
//             {
//                 hashedUsername = hashing.HashData(userInfo.username, pepper)
//             };

//             var userAccountParameters = new Dictionary<string, object>
//             {
//                 { "username", userInfo.username},
//                 {"birthDate", userInfo.dob}   
//             };

//             var tableData = new Dictionary<string, Dictionary<string, object>>
//             {
//                 { "userAccount", userAccountParameters }
//             };

//             var response = await _accountCreation.CreateUserAccount(userPepper, userInfo, tableData);
            
//             if (response.HasError)
//             {
//                 return BadRequest(response.ErrorMessage);
//             }

//             return Ok(new { Message = "User account created successfully" });
//         }
//     }
// }


    // public async Task<ActionResult> CreateUserAccount([FromBody] UserInfo userInfo)
    // {
    //     // SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
    //     // string pepper = await pepperDao.ReadPepperAsync();
    //     string pepper = "DA0b";
    //     Hashing hashing = new Hashing();
    //     UserPepper userPepper = new UserPepper
    //     {
    //         hashedUsername = hashing.HashData(userInfo.username, pepper)
    //     };

    //     var userAccount_success_parameters = new Dictionary<string, object>
    //     {
    //         { "username", userInfo.username},
    //         {"birthDate", userInfo.dob}   
    //     };

    //     var userProfile_success_parameters = new Dictionary<string, object>
    //     {
    //         {"hashedUsername", userPepper.hashedUsername},
    //         { "FirstName", userInfo.firstname},
    //         { "LastName", userInfo.lastname}, 
    //         {"backupEmail", userInfo.backupEmail},
    //         {"appRole", userInfo.role}, 
    //     };
        
    //     var activeAccount_success_parameters = new Dictionary<string, object>
    //     {
    //         {"hashedUsername", userPepper.hashedUsername},
    //         {"isActive", userInfo.status} 
    //     };

    //     var hashedAccount_success_parameters = new Dictionary<string, object>
    //     {
    //         {"hashedUsername", userPepper.hashedUsername},
    //         {"username", userInfo.username},
    //     };

    //     var tableData = new Dictionary<string, Dictionary<string, object>>
    //     {
    //         { "userAccount", userAccount_success_parameters },
    //         { "userProfile", userProfile_success_parameters },
    //         { "activeAccount", activeAccount_success_parameters}, 
    //         {"userHash", hashedAccount_success_parameters}
    //     };

    //     // Now call CreateUserAccount with all required parameters
    //     var response = await _accountCreation.CreateUserAccount(userPepper, userInfo, tableData);

    //     if (response.HasError)
    //     {
    //         return BadRequest(response.ErrorMessage);
    //     }

    //     return Ok(new { userInfo });
    // }


// using Microsoft.AspNetCore.Mvc;
// using System.Data;
// // using SS.Backend.Services;
// using SS.Backend.Services.AccountCreationService;
// using SS.Backend.SharedNamespace;

// namespace demoAPI.Controllers;

// [ApiController]
// [Route("api/registration")]
// public class DemoController : ControllerBase
// {

//     private readonly IAccountCreation _accountCreation;
//     public DemoController (IAccountCreation AccountCreation){
//         _accountCreation = AccountCreation;
//     }
    
//     [Route("createAccount")]
//     [HttpGet]
//     public async Task<ActionResult<List<UserInfo>>> GetAllRequests(){

//         var response = await _accountCreation.ReadUserTable("dbo.userProfile");
//         List<UserInfo> userList = new List<UserInfo>();

//         try{

//             if (response.HasError)
//             {
//                 Console.WriteLine(response.ErrorMessage);
//                 return StatusCode(500, response.ErrorMessage);
//             }
            
//                 if (response.ValuesRead != null)
//                 {
//                     foreach (DataRow row in response.ValuesRead.Rows)
//                     {
//                         userList.Add(new UserInfo
//                         {
//                             username = row["hashedUsername"].ToString(),
//                             firstname = row["firstName"].ToString(), 
//                             lastname = row["lastName"].ToString(), 
//                             backupEmail = row["backupEmail"] != DBNull.Value ? Convert.ToString(row["backupEmail"]) : null,
//                             // AdditionalInformation = row["additionalInformation"] != DBNull.Value ? Convert.ToString(row["additionalInformation"]) : null
//                             role = Convert.ToInt32(row["appRole"]) 
//                         });
//                     }
//                 }

//                 return Ok(userList);

//         }
//         catch (Exception ex)
//         {
//             return StatusCode(500, $"An error occurred: {ex.Message}");
//         }

//     }

//     [HttpPost]
//     [Route("postAccount")]
//     // public async Task<ActionResult<List<UserInfo>>> PostCreateAccount([FromBody] UserInfo userInfo){
//     public async Task<IActionResult> PostCreateAccount([FromBody] AccountCreationRequest request){
//         Response response = new Response();
//         response.HasError = true;
        
//         if(request.UserInfo != null){
//             response = await _accountCreation.CreateUserAccount(request.UserInfo, request.CompanyInfo);
//             Console.WriteLine("is this ERROR: ", request.UserInfo.firstname);
//         }
//         if (response.HasError)
//             {
//                 // Console.WriteLine("ERROR: ", response.ErrorMessage);
//                 return BadRequest(response.ErrorMessage);
//             }
//         return Ok(new { message = "Account created successfully!", response });
//         }


//     public class VerifyAccountRequest
//     {
//         public string Username { get; set; }
//     }
//     [HttpPost]
//     [Route("verifyAccount")]
//     public async Task<IActionResult> VerifyAccount([FromBody] VerifyAccountRequest request)
//     {
//         if (request == null || string.IsNullOrWhiteSpace(request.Username))
//         {
//             return BadRequest("Invalid request");
//         }

//         var response = await _accountCreation.VerifyAccount(request.Username);
        
//         if (response.HasError)
//         {
//             return BadRequest(response.ErrorMessage);
//         }

//         return Ok(new { message = "Account verified successfully!", response });
//     }
// }
        
 

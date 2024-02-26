using Microsoft.AspNetCore.Mvc;

using SS.Backend.SharedNamespace;
using SS.Backend.Services.AccountCreationService;
namespace Registration.Controllers;

using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Reflection;
using SS.Backend.Services.LoggingService;

[ApiController]
[Route("api/registration")]
public class Registration : ControllerBase
{
    private readonly AccountCreation _accountCreation;

    public Registration(AccountCreation accountCreation)
    {
        _accountCreation = accountCreation;
    }


    [HttpPost("createAccount")]
    public async Task<IActionResult> CreateUserAccount([FromBody] UserInfo userInfo)
    {
        // SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
        // string pepper = await pepperDao.ReadPepperAsync();
        string pepper = "DA0b";
        Hashing hashing = new Hashing();
        // Construct or obtain a UserPepper instance (example shown, customize as necessary)
        UserPepper userPepper = new UserPepper
        {
            hashedUsername = hashing.HashData(userInfo.username, pepper)
        };

        // Construct or obtain tableData (example shown, customize as necessary)
        var userAccount_success_parameters = new Dictionary<string, object>
        {
            { "username", userInfo.username},
            {"birthDate", userInfo.dob}   
        };

        var userProfile_success_parameters = new Dictionary<string, object>
        {
            {"hashedUsername", userPepper.hashedUsername},
            { "FirstName", userInfo.firstname},
            { "LastName", userInfo.lastname}, 
            {"backupEmail", userInfo.backupEmail},
            {"appRole", userInfo.role}, 
        };
        
        var activeAccount_success_parameters = new Dictionary<string, object>
        {
            {"hashedUsername", userPepper.hashedUsername},
            {"isActive", userInfo.status} 
        };

        var hashedAccount_success_parameters = new Dictionary<string, object>
        {
            {"hashedUsername", userPepper.hashedUsername},
            {"username", userInfo.username},
        };

        var tableData = new Dictionary<string, Dictionary<string, object>>
        {
            { "userAccount", userAccount_success_parameters },
            { "userProfile", userProfile_success_parameters },
            { "activeAccount", activeAccount_success_parameters}, 
            {"userHash", hashedAccount_success_parameters}
        };

        // Now call CreateUserAccount with all required parameters
        var response = await _accountCreation.CreateUserAccount(userPepper, userInfo, tableData);

        if (response.HasError)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(new { userInfo });
    }
}

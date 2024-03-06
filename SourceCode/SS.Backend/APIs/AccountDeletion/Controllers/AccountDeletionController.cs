// AccountDeletionController.cs
using Microsoft.AspNetCore.Mvc;
using SS.Backend.Services.DeletingService;

namespace AccountDeletionAPI.Controllers
{
    // Define the route and indicating the class is a controller
    [Route("api/AccountDeletion")]
    [ApiController]
    public class AccountDeletionController : ControllerBase
    {

        private readonly IAccountDeletion _accountDeletion;

        // Constructing injection for account deletion
        public AccountDeletionController(IAccountDeletion accountDeletionService)
        {
            _accountDeletion = accountDeletionService;
        }

        // Defining the POST endpoint for account deletion
        [HttpPost("Delete")]
        public async Task<IActionResult> HttpDeleteAccount([FromBody] DeletionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Call the AccountDeletion service method to delete the account
            var response = await _accountDeletion.DeleteAccount(request.username);

            // Check if there was an error during account deletion
            if (response.HasError)
            {
                // returns a bad request with the error message
                return BadRequest(response.ErrorMessage);
            }

            // Return success response
            return Ok(response);
        }
    }
}

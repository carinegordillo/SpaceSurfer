// AccountDeletionController.cs
using Microsoft.AspNetCore.Mvc;
using SS.Backend.Services.DeletingService;

namespace AccountDeletionAPI.Controllers
{
    [Route("/api/AccountDeletion")]
    [ApiController]
    public class AccountDeletionController : ControllerBase
    {
        private readonly IAccountDeletion _accountDeletionService;

        public AccountDeletionController(IAccountDeletion accountDeletionService)
        {
            _accountDeletionService = accountDeletionService;
        }

        [HttpPost()]
        [Route("Delete")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeletionRequest request)
        {
            // Call the AccountDeletion service method to delete the account
            var response = await _accountDeletionService.DeleteAccount(request.username);

            // Check if there was an error during account deletion
            if (response.HasError)
            {
                return BadRequest(response.ErrorMessage);
            }

            // Return success response
            return Ok(new { message = "Account deleted successfully" });
        }
    }
}

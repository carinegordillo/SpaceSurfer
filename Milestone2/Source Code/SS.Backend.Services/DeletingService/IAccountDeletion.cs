using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService

/// <summary>
///     IAccountDeletion an interface resposible of deleting user records of a database
/// </summary>
///
/// <param username> String value representing a username.</param>
{
    public interface IAccountDeletion
    {
        public Task<Response> DeleteAccount(string username);

    }
}

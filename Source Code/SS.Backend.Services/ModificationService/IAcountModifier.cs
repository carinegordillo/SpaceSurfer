using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.ModificationService
{
    public interface IAccountModifier
    {
        public Task<Response> UpdateDisplayName(string username, string newEmail, int newAge);

    }
}

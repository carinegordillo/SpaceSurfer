using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.UpdatingService
{
    public interface IUpdater
    {
        public Task<Response> UpdateDisplayName(string username, string newEmail, int newAge);

    }
}

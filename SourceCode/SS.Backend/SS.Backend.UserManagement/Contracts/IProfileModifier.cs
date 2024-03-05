using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.UserManagement
{
    public interface IProfileModifier
    {
        public Task<Response> ModifyFirstName(string hashedUsername, string newFirstName);
        public Task<Response> ModifyLastName(string hashedUsername, string newLastName);
        public Task<Response> ModifyBackupEmail(string hashedUsername, string newBackupEmail);
        public Task<Response> getUserProfile(string hashedUsername);

    }
}

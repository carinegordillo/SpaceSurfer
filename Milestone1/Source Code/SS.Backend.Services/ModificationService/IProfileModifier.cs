using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.ModificationService
{
    public interface IProfileModifier
    {
        public Task<Response> ModifyFirstName(string hashedUsername, string newFirstName);
        public Task<Response> ModifyLastName(string hashedUsername, string newLastName);
        public Task<Response> ModifyBackupEmail(string hashedUsername, string newBackupEmail);
        public Task<Response> GenProfileModifier(string whereClause, object whereClauseval, string fieldName, object newValue, string profileTableName);

    }
}

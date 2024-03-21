using SS.Backend.SharedNamespace;

namespace SS.Backend.SpaceManager
{
    public interface ISpaceCreation
    {
        public Task<Response> CreateSpace(CompanyInfo? companyInfo, CompanyFloor? companyFloor);
        public List<Dictionary<string, object>> ListSpace(CompanyFloor? companyFloor);
        public Task<Response> ReadUserTable(string tableName);
    }
}
using SS.Backend.SharedNamespace;

namespace SS.Backend.SystemObservability
{
    public interface ICompanySpaceCountService
    {
        Task<IEnumerable<CompanySpaceCount>> GetTop3CompaniesWithMostSpaces(string username, string timeSpan);
    }
}

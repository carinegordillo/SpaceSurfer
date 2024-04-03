using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;

namespace SS.Backend.SpaceManager
{
    public interface ISpaceReader
    {
         public  Task<IEnumerable<CompanyInfoWithID>> GetCompanyInfoAsync();
        
        public  Task<IEnumerable<CompanyFloorStrImage>> GetCompanyFloorsAsync(int companyId);
        
    }
}

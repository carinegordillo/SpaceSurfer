using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;

namespace SS.Backend.SpaceManager
{
    public interface ISpaceReader
    {
        public  Task<IEnumerable<CompanyInfoWithID>> GetCompanyInfoAsync();
        public  Task<IEnumerable<CompanyFloorStrImage>> GetCompanyFloorsAsync(int companyId);
        public  Task<IEnumerable<CompanyInfoWithID>> GetAvailableCompaniesForUser(int? employeeCompanyID);
        public  Task<List<CompanyInfoWithID>> GetAllFacilities();
        public  Task<List<CompanyInfoWithID>> GetEmployeeCompany(int companyID);
        public  Task<Response> InsertIntoCompanyFloorPlansAsync(int companyID, string floorPlanName, string floorPlanPath);
        
        
        

        
    }
}


using SS.Backend.SharedNamespace;

namespace SS.Backend.SpaceManager
{

    public interface ISpaceManagerDao
    {
        public Task<Response> GeneralModifier(Dictionary<string, object> whereClauses, string fieldName, object newValue, string tableName);
       
        public Task<Response> DeleteField(Dictionary<string, object> conditions, string tableName);
        public  Task<Response> readTableWhere(string whereClause, object whereClauseval, string tableName);
        public Task<Response> GetFloorPlanIdByNameAndCompanyId(string floorPlanName, int companyID);
        

    }
}
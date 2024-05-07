
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
namespace SS.Backend.SpaceManager
{

    public interface ISpaceManagerDao
    {
        public Task<Response> GetCompanyIDByHashedUsername(string hashedUsername);
        
        public Task<Response> InsertIntoMultipleTables(Dictionary<string, Dictionary<string, object>> tableData);
        public Task<Response> GetCompanyFloorIDByName(string floorPlanName, int companyID);
        public  Task<Response> ReadUserTable(string tableName);
        public Task<Response> GeneralModifier(Dictionary<string, object> whereClauses, string fieldName, object newValue, string tableName);
       
        public Task<Response> DeleteField(Dictionary<string, object> conditions, string tableName);
        public  Task<Response> readTableWhere(string whereClause, object whereClauseval, string tableName);
        public Task<Response> GetFloorPlanIdByNameAndCompanyId(string floorPlanName, int companyID);
        public  Task<Response> ExecuteReadCompanyTables(SqlCommand command);
        public  Task<Response> ExecuteWriteCompanyTables(SqlCommand command);
    
        

    }
}
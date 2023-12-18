using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService
{
    public class TableNames : ITableNames
    {

        Credential temp = Credential.CreateSAUser();

        public async Task<Response> RetrieveTableNames()
        {

            var table = "TABLE_NAMES";
            SealedSqlDAO sealedSqlDAO = new SealedSqlDAO(temp);
            var commandBuild = new CustomSqlCommandBuilder();


            var tableNames = commandBuild.BeginSelect().SelectOne("TABLE_NAMES").From("INFORMATION_SCHEMA.TABLES").Where("TABLE_TYPE = BASE_TABLE AND TABLE_CATALOG = 'SS_Server' AND TABLE_NAME <> 'Logs'").Build();
            // Your SQL query to retrieve table names

            return await sealedSqlDAO.ReadSqlResult(tableNames);
        }
    }
}

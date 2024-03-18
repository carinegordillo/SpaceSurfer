using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.DeletingService
{
    /// <summary>
    ///     DatabaseHelper class is responsible of retreiving table names from a database
    /// </summary>
    ///
    public class DatabaseHelper : IDatabaseHelper
    {
        // Initializing temp credential
        Credential temp = Credential.CreateSAUser();

        public async Task<Response> RetrieveTableNames()
        {
            // Opens connection to Data Accss
            SealedSqlDAO sealedSqlDAO = new SealedSqlDAO(temp);

            //initialzing Sql Command builder
            CustomSqlCommandBuilder commandBuild = new CustomSqlCommandBuilder();

            // SQL Query to get the Table Names in the database
            var tableNames = commandBuild.BeginSelect().SelectOne("*").From("INFORMATION_SCHEMA.COLUMNS ").Where("COLUMN_NAME = 'username' AND TABLE_NAME<>'Logs';").Build();

            return await sealedSqlDAO.ReadSqlResult(tableNames);
        }
    }
}

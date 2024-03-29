// DatabaseHelper will no longer be needed unless a separate class is required to seclude the SqlCommands



//using SS.Backend.DataAccess;
//using SS.Backend.SharedNamespace;

//namespace SS.Backend.Services.DeletingService
//{
//    /// <summary>
//    ///     DatabaseHelper class is responsible of retrieving table names from a database
//    /// </summary>
//    ///
//    public class DatabaseHelper : IDatabaseHelper
//    {
//        // Initializing temp credential
//        Credential temp = Credential.CreateSAUser();

//#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
//        public async Task<Response> RetrieveTable(string user)
//#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
//        {
//            // Opens connection to Data Accss
//            // ISqlDAO sqlDAO = new SqlDAO(temp);

//            //initializing Sql Command builder
//            CustomSqlCommandBuilder commandBuild = new CustomSqlCommandBuilder();

//            // SQL Query to get the Table Names in the database
//            //var tableNames = commandBuild.BeginSelect().SelectOne("Username").From("dbo.userAccount ").Where("Username = '@user';").Build();

//            //return await sqlDAO.ReadSqlResult(tableNames);
//#pragma warning disable CS8603 // Possible null reference return.
//            return null;
//#pragma warning restore CS8603 // Possible null reference return.
//        }
//    }
//}

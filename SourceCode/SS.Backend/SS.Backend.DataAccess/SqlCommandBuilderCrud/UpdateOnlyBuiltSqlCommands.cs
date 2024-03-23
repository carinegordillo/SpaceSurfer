
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public class UpdateOnlyBuiltSqlCommands
    {
        private  ICustomSqlCommandBuilder _customSqlCommandBuilder;
        public UpdateOnlyBuiltSqlCommands(ICustomSqlCommandBuilder customSqlCommandBuilder)
        {
            _customSqlCommandBuilder = customSqlCommandBuilder;
        }

        public SqlCommand GenericUpdateOne(string whereClause, object whereClauseval, string fieldName, object newValue, string tableName)
        {

            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            
            var columnValues = new Dictionary<string, object>{
                { fieldName, newValue },{whereClause,whereClauseval}};

            SqlCommand updateCommand = commandBuilder.BeginUpdate(tableName)
                                            .Set(columnValues)
                                            .Where($"{whereClause} = @{whereClause}")
                                            .AddParameters(columnValues)
                                            .Build();

            return updateCommand;
        }

    }
}







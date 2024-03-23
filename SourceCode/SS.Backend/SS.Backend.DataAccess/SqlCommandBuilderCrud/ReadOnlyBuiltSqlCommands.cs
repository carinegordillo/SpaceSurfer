
using Microsoft.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public class ReadOnlyBuiltSqlCommands
    {
        private  ICustomSqlCommandBuilder _customSqlCommandBuilder;
        public ReadOnlyBuiltSqlCommands(ICustomSqlCommandBuilder customSqlCommandBuilder)
        {
            _customSqlCommandBuilder = customSqlCommandBuilder;
        }

        public SqlCommand GenericReadWhereSingular(string whereClause, object whereClauseVal, string tableName)
        {

            var parameters = new Dictionary<string, object>
            {{ whereClause, whereClauseVal }};

            SqlCommand ReadCommand = _customSqlCommandBuilder.BeginSelectAll()
                        .From(tableName)
                        .Where($"{whereClause} = @{whereClause}")
                        .AddParameters(parameters)
                        .Build();
            
            
            return ReadCommand;
        }

        public SqlCommand GenericReadAllFrom(string tableName)
        {

            SqlCommand readAllCommand = _customSqlCommandBuilder.BeginSelectAll()
                                            .From($"{tableName}")
                                            .Build();

           
            return readAllCommand;
        }
    }
}
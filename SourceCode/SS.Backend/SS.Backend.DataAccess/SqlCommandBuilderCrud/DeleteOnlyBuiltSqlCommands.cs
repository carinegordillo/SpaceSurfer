
using Microsoft.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public class DeleteOnlyBuiltSqlCommands
    {
        private  ICustomSqlCommandBuilder _customSqlCommandBuilder;
        public DeleteOnlyBuiltSqlCommands(ICustomSqlCommandBuilder customSqlCommandBuilder)
        {
            _customSqlCommandBuilder = customSqlCommandBuilder;
        }

        public SqlCommand DeleteTableContents(string tableName)
        {

            SqlCommand deleteCommand = _customSqlCommandBuilder.BeginDelete(tableName)
                                            .Build();

            
            
            return deleteCommand;
        }

        public SqlCommand DeleteRowWhere(string whereClause, object whereClauseval, string tableName)
        {

            SqlCommand deleteCommand = _customSqlCommandBuilder.BeginDelete(tableName).Where($"{whereClause} = '{whereClauseval}'").Build();

            
            
            return deleteCommand;
        }

        
    }
}
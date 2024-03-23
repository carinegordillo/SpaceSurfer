
using Microsoft.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public class CreateOnlyBuiltSqlCommands
    {
        private  ICustomSqlCommandBuilder _customSqlCommandBuilder;
        public CreateOnlyBuiltSqlCommands(ICustomSqlCommandBuilder customSqlCommandBuilder)
        {
            _customSqlCommandBuilder = customSqlCommandBuilder;
        }

        public SqlCommand GenericInsert(Dictionary<string, object> columnValues, string tableName)
        {
            SqlCommand InserCommand = _customSqlCommandBuilder.BeginInsert(tableName)
                                                            .Columns(columnValues.Keys)
                                                            .Values(columnValues.Keys)
                                                            .AddParameters(columnValues)
                                                            .Build();

            return InserCommand;
        }


    }
}
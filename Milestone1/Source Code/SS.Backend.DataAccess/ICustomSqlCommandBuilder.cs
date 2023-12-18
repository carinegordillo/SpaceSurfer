using System.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public interface ICustomSqlCommandBuilder
    {
        ICustomSqlCommandBuilder BeginInsert(string tableName);
        ICustomSqlCommandBuilder Columns(IEnumerable<string> columns);
        ICustomSqlCommandBuilder Values(IEnumerable<string> columns);
        ICustomSqlCommandBuilder BeginUpdate(string tableName);
        ICustomSqlCommandBuilder Select();
        ICustomSqlCommandBuilder From(string tableName);
        ICustomSqlCommandBuilder SelectOne(string column);
        ICustomSqlCommandBuilder Set(Dictionary<string, object> columnValues);
        ICustomSqlCommandBuilder Where(string whereClause);
        ICustomSqlCommandBuilder BeginDelete(string tableName);
        ICustomSqlCommandBuilder AddParameters(Dictionary<string, object> parameters);

        SqlCommand Build();

    }
}
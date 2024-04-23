
using Microsoft.Data.SqlClient;
using System.Text;
using System.Data;

namespace SS.Backend.DataAccess;
public class CustomSqlCommandBuilder : ICustomSqlCommandBuilder
{
    private SqlCommand _command;
    private StringBuilder _commandText;

    public CustomSqlCommandBuilder()
    {
        _command = new SqlCommand();
        _commandText = new StringBuilder();
    }

    public ICustomSqlCommandBuilder BeginInsert(string tableName)
    {
        ResetBuilder();
        _commandText.Append($"INSERT INTO {tableName} ");
        return this;
    }

    public ICustomSqlCommandBuilder Columns(IEnumerable<string> columns)
    {
        _commandText.Append($"({string.Join(", ", columns)}) ");
        return this;
    }

    public ICustomSqlCommandBuilder Values(IEnumerable<string> columns)
    {
        var valueParameters = columns.Select(column => $"@{column}");
        _commandText.Append($"VALUES ({string.Join(", ", valueParameters)})");
        return this;
    }

    public ICustomSqlCommandBuilder BeginUpdate(string tableName)
    {
        ResetBuilder();
        _commandText.Append($"UPDATE {tableName} SET ");
        return this;
    }

    public ICustomSqlCommandBuilder BeginSelect()
    {
        _commandText.Clear();
        _commandText.Append("SELECT ");
        return this;
    }

    public ICustomSqlCommandBuilder BeginSelectAll()
    {
        _commandText.Clear();
        _commandText.Append("SELECT *");
        return this;
    }

    public ICustomSqlCommandBuilder SelectColumns(params string[] columns)
    {
        _commandText.Append(string.Join(", ", columns));
        return this;
    }

    public ICustomSqlCommandBuilder From(string tableName)
    {
        _commandText.Append($" FROM {tableName}");
        return this;
    }

    public ICustomSqlCommandBuilder Set(Dictionary<string, object> columnValues)
    {
        var setClauses = columnValues.Select(kv => $"{kv.Key} = @{kv.Key}");
        _commandText.Append(string.Join(", ", setClauses) + " ");
        return this;
    }

    public ICustomSqlCommandBuilder Where(string whereClause)
    {
        _commandText.Append($" WHERE {whereClause}");
        return this;
    }

    public ICustomSqlCommandBuilder WhereMultiple(Dictionary<string, object> conditions, string logicalOperator = "AND")
    {
        var conditionStrings = conditions.Select(kv => $"{kv.Key} = @{kv.Key}");
        _commandText.Append($" WHERE {string.Join($" {logicalOperator} ", conditionStrings)}");
        return this;
    }

    public ICustomSqlCommandBuilder Join(string joinTable, string fromColumn, string toColumn)
    {
        _commandText.Append($" JOIN {joinTable} ON {fromColumn} = {toColumn}");
        return this;
    }

    public ICustomSqlCommandBuilder BeginDelete(string tableName)
    {
        ResetBuilder();
        _commandText.Append($"DELETE FROM {tableName} ");
        return this;
    }

    public ICustomSqlCommandBuilder AddParameters(Dictionary<string, object> parameters)
    {
        foreach (var param in parameters)
        {
            _command.Parameters.AddWithValue($"@{param.Key}", param.Value);
        }
        return this;
    }

    public ICustomSqlCommandBuilder BeginStoredProcedure(string storedProcedureName)
    {
        ResetBuilder(); 
        _command.CommandType = CommandType.StoredProcedure;
        _commandText.Append(storedProcedureName); 
        return this;
    }

    private void ResetBuilder()
    {
        _commandText.Clear();
        _command = new SqlCommand();
    }

    public SqlCommand Build()
    {
        _command.CommandText = _commandText.ToString();
        return _command;
    }
}
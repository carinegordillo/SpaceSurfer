
﻿using Microsoft.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public interface ICustomSqlCommandBuilder
    {
        ICustomSqlCommandBuilder BeginInsert(string tableName);
        ICustomSqlCommandBuilder Columns(IEnumerable<string> columns);
        ICustomSqlCommandBuilder Values(IEnumerable<string> columns);
        ICustomSqlCommandBuilder BeginUpdate(string tableName);
        ICustomSqlCommandBuilder Set(Dictionary<string, object> columnValues);
        ICustomSqlCommandBuilder Where(string whereClause);
        ICustomSqlCommandBuilder WhereMultiple(Dictionary<string, object> conditions, string logicalOperator = "AND");
        ICustomSqlCommandBuilder BeginSelect();
        ICustomSqlCommandBuilder BeginSelectAll();
        ICustomSqlCommandBuilder BeginSelectString(string statement);
        ICustomSqlCommandBuilder SelectColumns(params string[] columns);
        ICustomSqlCommandBuilder From(string tableName);
        ICustomSqlCommandBuilder BeginDelete(string tableName);
        ICustomSqlCommandBuilder Join(string joinTable, string fromColumn, string toColumn);
        ICustomSqlCommandBuilder BeginStoredProcedure(string storedProcedureName);
        ICustomSqlCommandBuilder OrderBy(string tableName);
        ICustomSqlCommandBuilder AddParameters(Dictionary<string, object> parameters);
        SqlCommand Build();

    }
}

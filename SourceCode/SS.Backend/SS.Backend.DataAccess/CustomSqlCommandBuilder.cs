
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

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

    public ICustomSqlCommandBuilder BeginSelectString(string statement)
    {
        _commandText.Clear();
        _commandText.Append($"SELECT {statement}");
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

    public ICustomSqlCommandBuilder InnerJoin(string joinTable, string fromColumn, string toColumn)
    {
        _commandText.Append($" INNER JOIN {joinTable} ON {fromColumn} = {toColumn}");
        return this;
    }

    public ICustomSqlCommandBuilder BeginDelete(string tableName)
    {
        ResetBuilder();
        _commandText.Append($"DELETE {tableName}");
        return this;
    }

    public ICustomSqlCommandBuilder BeginDeleteFrom(string tableName)
    {
        ResetBuilder();
        _commandText.Append($"DELETE FROM {tableName}");
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

    public ICustomSqlCommandBuilder OrderBy(string tableName)
    {
        _commandText.Append($" ORDER BY {tableName} ");
        return this;
    }

    public ICustomSqlCommandBuilder GroupBy(string tableName)
    {
        _commandText.Append($" GROUP BY {tableName} ");
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

    // Waitlist Custom Queries
    public ICustomSqlCommandBuilder getSid(string tableName, int rid)
    {
        ResetBuilder();
        _commandText.Append($"SELECT* FROM {tableName} WHERE reservationID = @rid");
        _command.Parameters.AddWithValue("@rid", rid);
        return this;
    }

    public ICustomSqlCommandBuilder getCid(string compName)
    {
        ResetBuilder();
        _commandText.Append("SELECT* FROM companyProfile WHERE companyName = @name");
        _command.Parameters.AddWithValue("@name", compName);
        return this;
    }

    public ICustomSqlCommandBuilder getFloor(int compId, int floorId)
    {
        ResetBuilder();
        _commandText.Append("SELECT * FROM companyFloor WHERE companyID = @cid AND floorPlanID = @fid");
        _command.Parameters.AddWithValue("@cid", compId);
        _command.Parameters.AddWithValue("@fid", floorId);
        return this;
    }

    public ICustomSqlCommandBuilder getFid(string tableName, int compId)
    {
        ResetBuilder();
        _commandText.Append($"SELECT * FROM {tableName} WHERE companyID = @cid");
        _command.Parameters.AddWithValue("@cid", compId);
        return this;
    }

    public ICustomSqlCommandBuilder CountWaitlistUsersForReservation(int reservationID)
    {
        ResetBuilder();
        _commandText.Append("SELECT COUNT(*) AS Count FROM Waitlist WHERE ReservationID = @ReservationID");
        _command.Parameters.AddWithValue("@ReservationID", reservationID);
        return this;
    }

    public ICustomSqlCommandBuilder UpdatePositionWaitlisted(int resId, int oldPos, int newPos)
    {
        ResetBuilder();
        _commandText.Append("UPDATE Waitlist SET Position = @new WHERE ReservationID = @id AND Position = @old");
        _command.Parameters.AddWithValue("@id", resId);
        _command.Parameters.AddWithValue("@old", oldPos);
        _command.Parameters.AddWithValue("@new", newPos);
        return this;
    }

    public ICustomSqlCommandBuilder UpdatePosition(int resId, string user, int newPos)
    {
        ResetBuilder();
        _commandText.Append("UPDATE Waitlist SET Position = @new WHERE ReservationID = @id AND Username = @user");
        _command.Parameters.AddWithValue("@id", resId);
        _command.Parameters.AddWithValue("@user", user);
        _command.Parameters.AddWithValue("@new", newPos);
        return this;
    }

    public ICustomSqlCommandBuilder GetAllWaitlist(int resId)
    {
        ResetBuilder();
        _commandText.Append("SELECT * FROM Waitlist WHERE ReservationID = @id");
        _command.Parameters.AddWithValue("@id", resId);
        return this;
    }

    public ICustomSqlCommandBuilder GetCompId(string tableName, int resId)
    {
        ResetBuilder();
        _commandText.Append($"SELECT * FROM {tableName} WHERE reservationID = @id");
        _command.Parameters.AddWithValue("@id", resId);
        return this;
    }

    public ICustomSqlCommandBuilder GetCompName(int compId)
    {
        ResetBuilder();
        _commandText.Append("SELECT * FROM companyProfile WHERE companyID = @id");
        _command.Parameters.AddWithValue("@id", compId);
        return this;
    }

    //public ICustomSqlCommandBuilder getResIDWithConflict(string tableName, int cid, int fid, string sid, DateTime s, DateTime e)
    //{
    //    ResetBuilder();
    //    _commandText.Append($"SELECT * FROM {tableName} WHERE companyID = @cid AND floorPlanID = @fid AND spaceID = @sid AND ((reservationStartTime <= @s AND reservationEndTime >= @s) OR (reservationStartTime <= @e AND reservationEndTime >= @e))");
    //    _command.Parameters.AddWithValue("@cid", cid);
    //    _command.Parameters.AddWithValue("@fid", fid);
    //    _command.Parameters.AddWithValue("@sid", sid);
    //    _command.Parameters.AddWithValue("@s", s);
    //    _command.Parameters.AddWithValue("@e", e);
    //    return this;
    //}

    public ICustomSqlCommandBuilder getResIDWithConflict(string tableName, int cid, int fid, string sid, string s, string e)
    {
        ResetBuilder();
        _commandText.Append($"SELECT * FROM {tableName} WHERE companyID = @cid AND floorPlanID = @fid AND spaceID = @sid AND ((reservationStartTime <= @s AND reservationEndTime >= @s) OR (reservationStartTime <= @e AND reservationEndTime >= @e))");
        _command.Parameters.AddWithValue("@cid", cid);
        _command.Parameters.AddWithValue("@fid", fid);
        _command.Parameters.AddWithValue("@sid", sid);
        _command.Parameters.AddWithValue("@s", s);
        _command.Parameters.AddWithValue("@e", e);
        return this;
    }

    public ICustomSqlCommandBuilder onWaitlist(string user, int rid)
    {
        ResetBuilder();
        _commandText.Append("SELECT * FROM Waitlist WHERE Username = @user AND ReservationID = @rid");
        _command.Parameters.AddWithValue("@user", user);
        _command.Parameters.AddWithValue("@rid", rid);
        return this;
    }

    public ICustomSqlCommandBuilder getNext(int rid)
    {
        ResetBuilder();
        _commandText.Append("SELECT * FROM Waitlist WHERE Position = 1 AND ReservationID = @rid");
        _command.Parameters.AddWithValue("@rid", rid);
        return this;
    }
}
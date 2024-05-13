
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

    // DELETION //

    public ICustomSqlCommandBuilder deleteLogs(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM Logs WHERE Username = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteUserAccount(string email)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM userAccount WHERE username = @username");
        _command.Parameters.AddWithValue("@username", email);
        return this;
    }

    public ICustomSqlCommandBuilder deleteUserHash(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM userHash WHERE hashedUsername = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteUserProfile(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM userProfile WHERE hashedUsername = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteActiveAccount(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM activeAccount WHERE hashedUsername = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteUserRequests(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM userRequests WHERE userHash = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteConfirmReservations(int resId)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM ConfirmReservations WHERE reservationID = @reservationId");
        _command.Parameters.AddWithValue("@reservationId", resId);
        return this;
    }

    public ICustomSqlCommandBuilder deleteCompanyFloor(int compId)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM companyFloor WHERE companyID = @compID");
        _command.Parameters.AddWithValue("@compID", compId);
        return this;
    }

    public ICustomSqlCommandBuilder deleteCompanyFloorSpaces(int compId)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM companyFloorSpaces WHERE companyID = @compID");
        _command.Parameters.AddWithValue("@compID", compId);
        return this;
    }

    public ICustomSqlCommandBuilder deleteReservations(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM reservations WHERE userHash = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteCompanyProfile(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM companyProfile WHERE hashedUsername = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteOTP(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM OTP WHERE Username = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteWaitlist(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM Waitlist WHERE Username = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteSystemObs(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM SystemObservability WHERE Username = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteFeatureAcc(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM FeatureAccess WHERE hashedUsername = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder deleteViewDur(string userHash)
    {
        ResetBuilder();
        _commandText.Append("DELETE FROM ViewDurations WHERE hashedUsername = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder getResId(string userHash)
    {
        ResetBuilder();
        _commandText.Append("SELECT * FROM reservations WHERE userHash = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder getCompId(string userHash)
    {
        ResetBuilder();
        _commandText.Append("SELECT * FROM companyProfile WHERE hashedUsername = @username");
        _command.Parameters.AddWithValue("@username", userHash);
        return this;
    }

    public ICustomSqlCommandBuilder getResDetails(string table, int resId)
    {
        ResetBuilder();
        _commandText.Append($"SELECT * FROM {table} WHERE reservationID = @id");
        _command.Parameters.AddWithValue("@id", resId);
        return this;
    }

    public ICustomSqlCommandBuilder insertIntoLoginAttempts(string userHash, DateTime time)
    {
        ResetBuilder();
        _commandText.Append($"INSERT INTO loginAttempts (Username, Timestamp, Attempts) VALUES ('{userHash}', '{time}', 0);");
        return this;
    }

    public ICustomSqlCommandBuilder resetLoginAttempts(string userHash, DateTime time)
    {
        ResetBuilder();
        _commandText.Append($"UPDATE loginAttempts SET Timestamp = '{time}', Attempts = 0 WHERE Username = '{userHash}';");
        return this;
    }

    public ICustomSqlCommandBuilder increaseLoginAttempts(string userHash, int attempts)
    {
        ResetBuilder();
        _commandText.Append($"UPDATE loginAttempts SET Attempts = {attempts} WHERE Username = '{userHash}';");
        return this;
    }

    public ICustomSqlCommandBuilder deactivateAccount(string userHash)
    {
        ResetBuilder();
        _commandText.Append($"UPDATE activeAccount SET isActive = 'No' WHERE hashedUsername = '{userHash}';");
        return this;
    }

    public ICustomSqlCommandBuilder clearLoginAttempts(string userHash)
    {
        ResetBuilder();
        _commandText.Append($"DELETE FROM loginAttempts WHERE Username = '{userHash}';");
        return this;
    }

    public ICustomSqlCommandBuilder checkLoginAttempts(string userHash)
    {
        ResetBuilder();
        _commandText.Append($"SELECT * FROM loginAttempts WHERE Username = '{userHash}';");
        return this;
    }
}
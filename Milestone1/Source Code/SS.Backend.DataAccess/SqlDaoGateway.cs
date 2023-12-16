

using System.Data.SqlClient;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using System.Text;

namespace SS.Backend.DataGateway;
public class SqlDaoGateway
    {
        public SqlDaoGateway(){

        }
        //this is just here to satify the current need for credentials but should be removed once we get the config file up
         Credential removeMeLater = Credential.CreateSAUser();


        public string GetSqlCommandText(SqlCommand command)
        {
            return command.CommandText;
        }

        public string GetSqlCommandParameters(SqlCommand command)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Parameters:");
            foreach (SqlParameter param in command.Parameters)
            {
                sb.AppendLine($"  @{param.ParameterName} = {param.Value}");
            }
            return sb.ToString();
        }
        
        public async Task<Response> Insert(string tableName, Dictionary<string, object> parameters)
        {

            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);

            Response response = new Response();

            SqlCommand insertCommand = new SqlCommand();
            
            var columns = string.Join(", ", parameters.Keys);
            var values = string.Join(", ", parameters.Keys.Select(p => "@" + p));
            insertCommand.CommandText = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

            foreach (var param in parameters)
            {
                insertCommand.Parameters.AddWithValue("@" + param.Key, param.Value);
            }

            //for testing purposes
            GetSqlCommandText(insertCommand);
            GetSqlCommandParameters(insertCommand);


            response = await SQLDao.SqlRowsAffected(insertCommand);

            return response;
        }

    public async Task<Response> Update(string tableName, Dictionary<string, object> parameters, string whereClause)
    {
        SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);

        Response response = new Response();

        SqlCommand updateCommand = new SqlCommand();

        var setClause = string.Join(", ", parameters.Keys.Select(p => $"{p} = @{p}"));
        updateCommand.CommandText = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

        foreach (var param in parameters)
        {
            updateCommand.Parameters.AddWithValue("@" + param.Key, param.Value);
        }

        //for testing purposes
        GetSqlCommandText(updateCommand);
        GetSqlCommandParameters(updateCommand);

        response = await SQLDao.SqlRowsAffected(updateCommand);

        return response;
        
    }

    public async Task<Response> Delete(string tableName, string whereClause)
    {   
        SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);

        Response response = new Response();

        SqlCommand DeleteCommand = new SqlCommand();

        DeleteCommand.CommandText = $"DELETE FROM {tableName} WHERE {whereClause}";

        //for testing purposes
        GetSqlCommandText(DeleteCommand);
        GetSqlCommandParameters(DeleteCommand);
        
        response = await SQLDao.SqlRowsAffected(DeleteCommand);

        return response;

    }
}
    
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;


namespace SS.Backend.SpaceManager
{

    public class SpaceManagerDao : ISpaceManagerDao
    {
        private ISqlDAO _sqldao;

        public SpaceManagerDao(ISqlDAO sqldao)
        {
            _sqldao = sqldao;
        }


        public async Task<Response> GeneralModifier(Dictionary<string, object> whereClauses, string fieldName, object newValue, string tableName)
        {
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            var columnValues = new Dictionary<string, object> { { fieldName, newValue } };
            foreach (var clause in whereClauses)
            {
                columnValues.Add(clause.Key, clause.Value);
            }

            var updateCommand = commandBuilder.BeginUpdate(tableName)
                                            .Set(new Dictionary<string, object> { { fieldName, newValue } })
                                            .WhereMultiple(whereClauses)
                                            .AddParameters(columnValues)
                                            .Build();

            response = await _sqldao.SqlRowsAffected(updateCommand);

            if (!response.HasError)
            {
                response.ErrorMessage += "- General Modifier - command successful -";
            }
            else
            {
                response.ErrorMessage += $"- General Modifier - command: {updateCommand.CommandText} not successful -";
            }
            return response;
        }



        public async Task<Response> DeleteField(Dictionary<string, object> conditions, string tableName)
        {
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            // Create the WHERE clause dynamically based on conditions
            string whereClause = string.Join(" AND ", conditions.Keys.Select(key => $"{key} = @{key}"));

            var deleteCommand = commandBuilder.BeginDelete(tableName)
                                            .Where(whereClause)
                                            .AddParameters(conditions)
                                            .Build();

            response = await _sqldao.SqlRowsAffected(deleteCommand);

            if (!response.HasError)
            {
                response.ErrorMessage += "- Delete Field - command successful -";
            }
            else
            {
                response.ErrorMessage += $"- Delete Field - command : {deleteCommand.CommandText} not successful -";
            }

            return response;
        }



        public async Task<Response> readTableWhere(string whereClause, object whereClauseval, string tableName)
        {

            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {{ whereClause, whereClauseval }};

            var command = commandBuilder.BeginSelectAll()
                        .From(tableName)
                        .Where($"{whereClause} = @{whereClause}")
                        .AddParameters(parameters)
                        .Build();
            
            response = await _sqldao.ReadSqlResult(command);

            if (response.HasError == false){
                response.ErrorMessage += "- readTableWhere- command successful -";
            }
            else{
                 response.ErrorMessage += $"- readTableWhere- {command.CommandText} -  command not successful -";

            }
            return response;



        }

        public async Task<Response> GetFloorPlanIdByNameAndCompanyId(string floorPlanName, int companyID)
        {
            var commandBuilder = new CustomSqlCommandBuilder();
            var parameters = new Dictionary<string, object>
            {
                { "floorPlanName", floorPlanName },
                { "companyID", companyID }
            };
            var selectCommand = commandBuilder.BeginSelect()
                .SelectColumns("floorPlanID")
                .From("companyFloor")
                .Where("floorPlanName = @floorPlanName AND companyID = @companyID")
                .AddParameters(parameters)
                .Build();

            return await _sqldao.ReadSqlResult(selectCommand);
        }



    }
}
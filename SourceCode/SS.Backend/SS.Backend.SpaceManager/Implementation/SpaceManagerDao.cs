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

        public async Task<Response> GetCompanyIDByHashedUsername(string hashedUsername)
        {
            Response response = new Response();
            // Use CustomSqlCommandBuilder to create SQL commands dynamically
            var commandBuilder = new CustomSqlCommandBuilder();

            // Prepare SQL parameters for the query
            var parameters = new Dictionary<string, object>
            {
                { "HashedUsername", hashedUsername }
            };

            // Build the SELECT SQL command
            var selectCommand = commandBuilder.BeginSelect()
                                            .SelectColumns("companyID") // Specify columns to select
                                            .From("companyProfile") // Specify the table
                                            .Where("hashedUsername = @HashedUsername") // Condition
                                            .AddParameters(parameters) // Add parameters
                                            .Build();

            // Execute the SELECT command
            response = await _sqldao.ReadSqlResult(selectCommand);

            // Log success or failure messages
            if (!response.HasError)
            {
                response.ErrorMessage += "- GetCompanyIDByHashedUsername - command successful -";
            }
            else
            {
                response.ErrorMessage += $"- GetCompanyIDByHashedUsername - command: {selectCommand.CommandText} not successful -";
            }

            return response;
        }

        // Asynchronous method to insert data into multiple tables
        public async Task<Response> InsertIntoMultipleTables(Dictionary<string, Dictionary<string, object>> tableData)
        {   
            // Use CustomSqlCommandBuilder to create SQL commands dynamically
            var builder = new CustomSqlCommandBuilder();
            Response tablesresponse = new Response();

            // Iterate through each table entry in the provided data
            foreach (var tableEntry in tableData)
            {
                string tableName = tableEntry.Key; // Table name
                Dictionary<string, object> parameters = tableEntry.Value; // Parameters for the table

                // Build the INSERT SQL command for each table
                var insertCommand = builder.BeginInsert(tableName)
                    .Columns(parameters.Keys) // Specify columns
                    .Values(parameters.Keys) // Specify values (same as columns for parameterized queries)
                    .AddParameters(parameters) // Add parameters
                    .Build();

                // Execute the INSERT command
                tablesresponse = await _sqldao.SqlRowsAffected(insertCommand);

                // Check for errors and return if any
                if (tablesresponse.HasError)
                {
                    tablesresponse.ErrorMessage += $"{tableName}: error inserting data; ";
                    return tablesresponse;
                }
            }
            return tablesresponse;
        }

        // Asynchronous method to retrieve a company floor ID using the floor plan name and company ID
        public async Task<Response> GetCompanyFloorIDByName(string floorPlanName, int companyID)
        {
            Response tablesresponse = new Response();
            var builder = new CustomSqlCommandBuilder();
            var command = builder.BeginSelect()
                                .SelectColumns("floorPlanID")
                                .From("companyFloor")
                                .Where("floorPlanName = @FloorPlanName AND companyID = @CompanyID")
                                .AddParameters(new Dictionary<string, object>
                                {
                                    { "FloorPlanName", floorPlanName },
                                    { "CompanyID", companyID }
                                })
                                .Build();

            var IDresponse = await _sqldao.ReadSqlResult(command);
            if (IDresponse.HasError)
            {
                tablesresponse.ErrorMessage += $"Error selecting floor ID; ";
            }
            return IDresponse;
        }

        public async Task<Response> ReadUserTable(string tableName)
        {
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            var selectCommand = commandBuilder.BeginSelectAll()
                                              .From(tableName)
                                              .Build();

            response = await _sqldao.ReadSqlResult(selectCommand);
            if (response.HasError)
            {
                response.ErrorMessage += $"{tableName}: error reading data; ";
            }
            else
            {
                response.ErrorMessage += "- ReadUserTable - command successful -";
            }

            return response;
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

        public async Task<Response> ExecuteReadCompanyTables(SqlCommand command){

            Response result = new Response();
            
            try
            {
                result = await _sqldao.ReadSqlResult(command);

            }
            catch (Exception ex)
            {
                result.HasError = true;
                Console.WriteLine($"Error in ExecuteReadCompanyTables: {ex.Message}");
                result.ErrorMessage = ex.Message;
            }

            if (result.HasError == false){
                result.ErrorMessage += "- ExecuteReadCompanyTables - command successful -";
                result.HasError = false;
            }
            else{
                result.ErrorMessage += $"- ExecuteReadCompanyTables - {command.CommandText} -  command not successful END OF \n\n -";
                result.HasError = true;

            }
            
            return result;
        }

        public async Task<Response> ExecuteWriteCompanyTables(SqlCommand command){

            Response result = new Response();
            
            try
            {
                result = await _sqldao.SqlRowsAffected(command);

            }
            catch (Exception ex)
            {
                result.HasError = true;
                Console.WriteLine($"Error in ExecuteReadCompanyTables: {ex.Message}");
                result.ErrorMessage = ex.Message;
            }

            if (result.HasError == false){
                result.ErrorMessage += "- ExecuteReadCompanyTables - command successful -";
                result.HasError = false;
            }
            else{
                result.ErrorMessage += $"- ExecuteReadCompanyTables - {command.CommandText} -  command not successful END OF \n\n -";
                result.HasError = true;

            }
            return result;
    }
}}
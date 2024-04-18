using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.TaskManagerHub;
using System.Data;


namespace SS.Backend.TaskManagerHub
{

    public class TaskManagerHubRepo : ITaskManagerHubRepo
    {
        private ISqlDAO _sqldao;

        public TaskManagerHubRepo(ISqlDAO sqldao)
        {
            _sqldao = sqldao;
        }

        public async Task<Response> ViewTasks(string hashedUsername)
        {
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            // Prepare SQL parameters for the query
            var parameters = new Dictionary<string, object>
            {
                { "HashedUsername", hashedUsername }
            };

            // Build the SELECT SQL command
            var selectCommand = commandBuilder.BeginSelectAll()
                                            .From("dbo.taskHub") 
                                            .Where("hashedUsername = @HashedUsername")
                                            .AddParameters(parameters) 
                                            .Build();

            response = await _sqldao.ReadSqlResult(selectCommand);

            if (response.ValuesRead != null)
            {
                // Convert DataTable to List<Dictionary<string, object>>
                foreach (DataRow row in response.ValuesRead.Rows)
                {
                    var task = new Dictionary<string, object>();
                    foreach (DataColumn col in response.ValuesRead.Columns)
                    {
#pragma warning disable CS8601 // Possible null reference assignment.
                        task[col.ColumnName] = row[col] != DBNull.Value ? row[col] : null;
#pragma warning restore CS8601 // Possible null reference assignment.
                    }
                    response.Values.Add(task);
                }
                response.HasError = false;  // Set HasError to false as operation was successful
            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = "No tasks found.";
            }

            return response;
        }


        public async Task<Response> ViewTasksByPriority(string hashedUsername, string priority)
        {
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                { "HashedUsername", hashedUsername }
            };

            var command = commandBuilder.BeginSelectAll()
                                .From("taskHub")
                                .Where("hashedUsername = @hashedUsername AND priority = @priority")
                                .AddParameters(new Dictionary<string, object>
                                {
                                    { "hashedUsername", hashedUsername},
                                    { "priority", priority}
                                })
                                .Build();

            response = await _sqldao.ReadSqlResult(command);

    
            if (!response.HasError)
            {
                response.ErrorMessage += "- View All Tasks - command successful -";
            }
            else
            {
                response.ErrorMessage += $"- View All Tasks - command: {command.CommandText} not successful -";
            }

            return response;
        }

        public async Task<Response> CreateTask(string hashedUsername, TaskHub task)
        {   
            // Use CustomSqlCommandBuilder to create SQL commands dynamically
            var builder = new CustomSqlCommandBuilder();
            Response tablesresponse = new Response();
            
            // Create a dictionary for the parameters
            var parameters = new Dictionary<string, object>
            {
                { "@hashedUsername", hashedUsername }, // Ensure keys include the '@' prefix to match SQL parameter usage
                { "@title", task.title },
                { "@description", task.description },
                { "@dueDate", task.dueDate ?? (object)DBNull.Value }, // Use DBNull.Value for null dates
                { "@priority", task.priority },
                { "@notificationSetting", task.notificationSetting ?? (object)DBNull.Value } // Use DBNull.Value for null settings
            };
            
            var insertCommand = builder.BeginInsert("taskHub")
                .Columns(new List<string>(parameters.Keys.Select(k => k.TrimStart('@')))) // Specify columns without '@'
                .Values(new List<string>(parameters.Keys)) // Use full parameter names including '@'
                .AddParameters(parameters) // Add parameters
                .Build();

            // Execute the INSERT command
            tablesresponse = await _sqldao.SqlRowsAffected(insertCommand);

            // Check for errors and return if any
            if (tablesresponse.HasError)
            {
                tablesresponse.ErrorMessage += $"taskHub: error inserting data; ";
                return tablesresponse;
            }

            return tablesresponse;
        }


        public async Task<Response> CreateMultipleTasks(string hashedUsername, List<TaskHub> tasks)
        {
            var builder = new CustomSqlCommandBuilder();
            Response overallResponse = new Response();

            foreach (var task in tasks)
            {
                var parameters = new Dictionary<string, object>
                {
                    { "hashedUsername", hashedUsername },
                    { "title", task.title },
                    { "description", task.description },
                    { "dueDate", task.dueDate ?? (object)DBNull.Value },
                    { "priority", task.priority },
                    { "notificationSetting", task.notificationSetting ?? (object)DBNull.Value }
                };
                
                 var insertCommand = builder.BeginInsert("taskHub")
                    .Columns(new List<string>(parameters.Keys.Select(k => k.TrimStart('@')))) // Specify columns without '@'
                    .Values(new List<string>(parameters.Keys)) // Use full parameter names including '@'
                    .AddParameters(parameters) // Add parameters
                    .Build();
                

                // Execute the INSERT command
                var response = await _sqldao.SqlRowsAffected(insertCommand);

                if (response.HasError)
                {
                    overallResponse.HasError = true;
                    overallResponse.ErrorMessage += $"Task {task.title}: error inserting data; {response.ErrorMessage}\n";
                }
            }

            if (!overallResponse.HasError)
            {
                overallResponse.HasError = false;
                overallResponse.ErrorMessage = "All tasks inserted successfully.";
            }

            return overallResponse;
        }


        // MODIFY use case: 
        // var fieldsToUpdate = new Dictionary<string, object>
        // {
        //     { "title", "New Title" },
        //     { "description", "New Description" },
        //     { "dueDate", new DateTime(2022, 12, 31) },
        // };
        // var response = await ModifyTaskFields("hashedUsernameExample", 123, fieldsToUpdate);
        public async Task<Response> ModifyTaskFields(string hashedUsername, string title, Dictionary<string, object> fieldsToUpdate)
        {
            var builder = new CustomSqlCommandBuilder();
            Response response = new Response();

            var updateCommand = builder.BeginUpdate("taskHub")
                                    .Set(fieldsToUpdate) 
                                    .Where("hashedUsername = @hashedUsername AND title = @title") 
                                    .AddParameters(new Dictionary<string, object>
                                    {
                                        { "hashedUsername", hashedUsername },
                                        { "taskId", title }
                                    }) // Add conditions parameters
                                    .AddParameters(fieldsToUpdate) 
                                    .Build();

            response = await _sqldao.SqlRowsAffected(updateCommand);

            if (response.HasError)
            {
                response.ErrorMessage = "Error updating task: " + response.ErrorMessage;
            }
            else
            {
                response.HasError = false;
                response.ErrorMessage = "Task updated successfully.";
            }

            return response;
        }

        public async Task<Response> DeleteTask(string hashedUsername, string taskTitle)
        {
            var builder = new CustomSqlCommandBuilder();
            Response response = new Response();

            // Build the DELETE SQL command
            var deleteCommand = builder.BeginDelete("taskHub")
                                    .Where("hashedUsername = @hashedUsername AND title = @title")
                                    .AddParameters(new Dictionary<string, object>
                                    {
                                        { "hashedUsername", hashedUsername },
                                        { "title", taskTitle }
                                    })
                                    .Build();

            // Execute the DELETE command
            response = await _sqldao.SqlRowsAffected(deleteCommand);

            if (response.HasError)
            {
                response.ErrorMessage = "Error deleting task: " + response.ErrorMessage;
            }
            else
            {
                response.HasError = false;
                response.ErrorMessage = "Task deleted successfully.";
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

}}
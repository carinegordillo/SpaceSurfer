// namespace SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;


namespace SS.Backend.TaskManagerHub
{
    public class TaskManagerHubService : ITaskManagerHubService
    {

        private readonly ITaskManagerHubRepo _taskManagerHubRepo;

        public TaskManagerHubService(ITaskManagerHubRepo taskManagerHubRepo)
        {
            _taskManagerHubRepo = taskManagerHubRepo;
        }
        public async Task<Response> ListTasks(string hashedUsername)
        {
            try
            {
                var viewTasksResponse = await _taskManagerHubRepo.ViewTasks(hashedUsername);
                
                if (!viewTasksResponse.HasError)
                {
                    viewTasksResponse.ErrorMessage += "View task list - command successful";
                }
                else
                {
                    viewTasksResponse.ErrorMessage += $"View Task List - command not successful - ErrorMessage {viewTasksResponse.ErrorMessage}";
                }
                
                return viewTasksResponse;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    HasError = true,
                    ErrorMessage = $"An unexpected error occurred while attempting to view tasks: {ex.Message}"
                };
            }

        }

        public async Task<Response> ListTasksByPriority(string hashedUsername, string priority){
            try
            {
                var viewTasksResponse = await _taskManagerHubRepo.ViewTasksByPriority(hashedUsername, priority);
                
                if (!viewTasksResponse.HasError)
                {
                    viewTasksResponse.ErrorMessage += "View task list by priority - command successful";
                }
                else
                {
                    viewTasksResponse.ErrorMessage += $"View Task List By Priority - command not successful - ErrorMessage {viewTasksResponse.ErrorMessage}";
                }
                
                return viewTasksResponse;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    HasError = true,
                    ErrorMessage = $"An unexpected error occurred while attempting to view tasks by priority: {ex.Message}"
                };
            }

        }

        private List<TaskHub> DataTableToList(DataTable dataTable)
        {
            var taskList = new List<TaskHub>();

            foreach (DataRow row in dataTable.Rows)
            {
                var task = new TaskHub
                {
                    // Assuming the presence of these columns in your DataTable
                    hashedUsername = row["hashedUsername"].ToString(),
                    title = row["title"].ToString(),
                    description = row["description"].ToString(),
                    priority = row["priority"].ToString(),
                    dueDate = Convert.ToDateTime(row["dueDate"]), 
                    notificationSetting = Convert.ToInt32(row["notificationSetting"]),
                    score = 0
                    // Add other necessary fields
                };
                
                taskList.Add(task);
            }
            return taskList;
        }

        private List<TaskHub> ScoreAndSortTasks(List<TaskHub> tasks)
        {
            var currentDate = DateTime.Now;

            // First, calculate and store scores in each task object, assuming TaskHub has a Score property
            foreach (var task in tasks)
            {
                if (task.dueDate < currentDate.AddDays(30)) // within 30 
                {
                    task.score = 2;
                }
                else if (task.dueDate < currentDate.AddDays(14)) // within 2 week 
                {
                    task.score = 4;
                }
                else if (task.dueDate < currentDate.AddDays(7)) // within 1 week
                {
                    task.score = 8;
                }
                else if (task.dueDate < currentDate.AddDays(3))
                {
                    task.score = 16;
                }
                else
                {
                    task.score = 0;
                }
                switch (task.priority.ToLower())
                {
                    case "high":
                        task.score = task.score * 5; // Assuming BaseScore holds the original score
                        break;
                    case "medium":
                        task.score = task.score * 2;
                        break;
                    case "low":
                        task.score = task.score * 0.5;
                        break;
                }
            
            }

            // Now sort the list based on the Score property
            tasks = tasks.OrderByDescending(task => task.score).ToList();

            return tasks;
        }


        public DataTable ListToDataTable(List<TaskHub> tasks)
        {
            // Create a new DataTable.
            DataTable table = new DataTable("Tasks");
        
            // Define columns
            table.Columns.Add("hashedUsername", typeof(string));
            table.Columns.Add("title", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("priority", typeof(string));
            table.Columns.Add("dueDate", typeof(DateTime));
            table.Columns.Add("notificationSetting", typeof(int));
            table.Columns.Add("score", typeof(double));
            

            // Iterate through all tasks
            foreach (TaskHub task in tasks)
            {
                // Create a new DataRow.
                DataRow row = table.NewRow();
                
                // Set column values
                row["hashedUsername"] = task.hashedUsername;
                row["title"] = task.title;
                row["description"] = task.description;
                row["priority"] = task.priority;
                row["dueDate"] = task.dueDate;
                row["notificationSetting"] = task.notificationSetting;
                row["score"] = task.score;
                

                // Add the row to the DataTable.
                table.Rows.Add(row);
            }

            return table;
        }

        public async Task<Response> ScoreTasks(string hashedUsername)
        {
            try
            {
                var viewTasksResponse = await _taskManagerHubRepo.AllTasks(hashedUsername);
                if (viewTasksResponse.HasError)
                {
                    return new Response { HasError = true, ErrorMessage = viewTasksResponse.ErrorMessage };
                }

                // Convert DataTable to List<TaskHub>
                var tasks = DataTableToList(viewTasksResponse.ValuesRead);

                // Score and sort tasks
                var sortedTasks = ScoreAndSortTasks(tasks);

                // Create new Response to return sorted tasks
                // Assuming you have a way to convert List<TaskHub> back to DataTable or similar structure as needed
                return new Response
                {
                    HasError = false,
                    ValuesRead = ListToDataTable(sortedTasks) 
                };
            }
            catch (Exception ex)
            {
                return new Response { HasError = true, ErrorMessage = $"Failed to score tasks: {ex.Message}" };
            }
        }
        public async Task<Response> CreateNewTask(TaskHub taskHub){
            try{
                var response = await _taskManagerHubRepo.CreateTask(taskHub);
                if (!response.HasError){
                    response.ErrorMessage += "Successful task creation in database!";
                }
                else{
                    response.ErrorMessage += $"New task did not insert in database - ErrorMessage {response.ErrorMessage}";
                }
                return response;
            }
            catch (Exception ex) {
                return new Response
                {
                    HasError = true,
                    ErrorMessage = $"An unexpected error occurred while attempting to create new task: {ex.Message}"
                };
             }

        }
    
        public async Task<Response> CreateMultipleNewTasks(string hashedUsername, List<TaskHub> tasks){
            try{
                var response = await _taskManagerHubRepo.CreateMultipleTasks(hashedUsername, tasks);
                if (!response.HasError){
                    response.ErrorMessage += "Successfully bulk uploaded multiple tasks!";
                }else{
                    response.ErrorMessage += "Unable to bulk upload multiple tasks";
                }
                return response;
            } catch(Exception ex){
                return new Response{
                    HasError = true,
                    ErrorMessage = $"An unexpected error occurred while attempting to bulk upload tasks: {ex.Message}"
                };
            }
        }
    
        public async Task<Response> ModifyTasks(TaskHub task, Dictionary<string, object> fieldsToUpdate){
            try{
                var response = await _taskManagerHubRepo.ModifyTaskFields(task, fieldsToUpdate);
                if (!response.HasError){
                    response.ErrorMessage += $"Successfully modified {fieldsToUpdate} in task {task.title}";

                }else{
                    response.ErrorMessage += $"Unable to modify {fieldsToUpdate} in task {task.title} - ErrorMessage {response.ErrorMessage}";
                }
                return response;
                
            }catch(Exception ex){
                return new Response{
                    HasError = true,
                    ErrorMessage = $"An unexpected error occurred while attempting to modify tasks: {ex.Message}"
                };

            }
        }

        public async Task<Response> DeleteTask(TaskHub task){
            try{
                var response = await _taskManagerHubRepo.DeleteTask(task);
                if (!response.HasError){
                    response.ErrorMessage += $"Successfully deleted {task}";
                }else{
                    response.ErrorMessage += $"Unable to delete {task} - ErrorMessage {response.ErrorMessage}";
                }
                return response;
            }catch(Exception ex){
                return new Response{
                    HasError = true,
                    ErrorMessage = $"An unexpected error occurred while attempting to delete task: {ex.Message}"
                };
            }
        }

    }
}

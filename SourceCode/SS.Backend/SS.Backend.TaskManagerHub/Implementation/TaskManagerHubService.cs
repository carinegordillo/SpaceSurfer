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

        private List<TaskHub> ScoreAndSortTasks(List<TaskHub> tasks)
        {
            var currentDate = DateTime.Now;

            foreach (var task in tasks)
            {
                // Calculate score based on due date proximity
                if (task.dueDate < currentDate.AddDays(1)){
                    task.score = 20;
                }
                else if (task.dueDate < currentDate.AddDays(3))
                {
                    task.score = 16;
                }
                else if (task.dueDate < currentDate.AddDays(7))
                {
                    task.score = 8;
                }
                else if (task.dueDate < currentDate.AddDays(14))
                {
                    task.score = 4;
                }
                else if (task.dueDate < currentDate.AddDays(30))
                {
                    task.score = 2;
                }
                else
                {
                    task.score = 0;
                }

                // Adjust score based on priority
                switch (task.priority.ToLower())
                {
                    case "high":
                        task.score *= 10;
                        break;
                    case "medium":
                        task.score *= 5;
                        break;
                    case "low":
                        task.score *= 2;
                        break;
                }
            }

            // Sort by score descending
            return tasks.OrderByDescending(t => t.score).ToList();
        }


        private List<TaskHub> ConvertDictionariesToTaskHubs(List<Dictionary<string, object>> dictionaries)
        {
            var taskList = new List<TaskHub>();

            foreach (var dict in dictionaries)
            {
                try
                {
                    var task = new TaskHub
                    {
                        hashedUsername = dict.TryGetValue("hashedUsername", out var hashedUsername) ? (string)hashedUsername : default,
                        title = dict.TryGetValue("title", out var title) ? (string)title : default,
                        description = dict.TryGetValue("description", out var description) ? (string)description : default,
                        dueDate = dict.TryGetValue("dueDate", out var dueDate) ? Convert.ToDateTime(dueDate) : default,
                        priority = dict.TryGetValue("priority", out var priority) ? (string)priority : default,
                        notificationSetting = dict.TryGetValue("notificationSetting", out var notificationSetting) ? Convert.ToInt32(notificationSetting) : default,
                        score = dict.TryGetValue("score", out var score) ? Convert.ToDouble(score) : default
                    };
                    taskList.Add(task);
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it accordingly
                    // Optionally continue to the next dictionary or handle the error based on your application's needs
                    Console.WriteLine($"Error converting dictionary to TaskHub: {ex.Message}");
                }
            }

            return taskList;
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

                // Convert dictionaries to TaskHub objects (assuming conversion from DataTable or similar structure)
                var tasks = ConvertDictionariesToTaskHubs(viewTasksResponse.Values);

                // Score and sort tasks
                var sortedTasks = ScoreAndSortTasks(tasks);

                // Convert sorted TaskHub list back to List<Dictionary<string, object>>
                var taskDictionaries = sortedTasks.Select(task => new Dictionary<string, object>
                {
                    {"hashedUsername", task.hashedUsername},
                    {"title", task.title},
                    {"description", task.description},
                    {"dueDate", task.dueDate.ToString()},
                    {"priority", task.priority},
                    {"notificationSetting", task.notificationSetting},
                    {"score", task.score}
                }).ToList();

                return new Response
                {
                    HasError = false,
                    Values = taskDictionaries
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

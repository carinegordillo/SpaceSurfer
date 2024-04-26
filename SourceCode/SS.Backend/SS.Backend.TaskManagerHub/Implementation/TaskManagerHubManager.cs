using SS.Backend.SharedNamespace;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.Backend.TaskManagerHub
{
    public class TaskManagerHubManager : ITaskManagerHubManager
    {
        private readonly ITaskManagerHubService _taskManagerHubService;

        public TaskManagerHubManager(ITaskManagerHubService taskManagerHubService)
        {
            _taskManagerHubService = taskManagerHubService;
        }

        public async Task<Response> ListTasks(string hashedUsername)
        {
            // Validate the input, for example, ensure hashedUsername is not null or empty
            if (string.IsNullOrWhiteSpace(hashedUsername))
                return new Response { HasError = true, ErrorMessage = "Invalid username." };

            return await _taskManagerHubService.ListTasks(hashedUsername);
        }

        public async Task<Response> ScoreTasks(string hashedUsername){
            if (string.IsNullOrWhiteSpace(hashedUsername))
                return new Response { HasError = true, ErrorMessage = "Invalid username." };

            return await _taskManagerHubService.ScoreTasks(hashedUsername);
        }

        public async Task<Response> ListTasksByPriority(string hashedUsername, string priority)
        {
            // Implement priority validation logic if necessary
            if (string.IsNullOrWhiteSpace(hashedUsername) || string.IsNullOrWhiteSpace(priority))
                return new Response { HasError = true, ErrorMessage = "Username or priority is invalid." };

            return await _taskManagerHubService.ListTasksByPriority(hashedUsername, priority);
        }

        public async Task<Response> CreateNewTask(TaskHub taskHub)
        {
            
            if (string.IsNullOrWhiteSpace(taskHub.title) || taskHub.title.Length > 20)
                return new Response { HasError = true, ErrorMessage = "Invalid task title." };

            if (taskHub.dueDate < DateTime.UtcNow)
                return new Response { HasError = true, ErrorMessage = "Due date cannot be in the past." };

            var validPriorities = new HashSet<string> { "low", "medium", "high" };
            if (!validPriorities.Contains(taskHub.priority.ToLower()))
                return new Response { HasError = true, ErrorMessage = "Invalid task priority." };

            if (string.IsNullOrWhiteSpace(taskHub.description))
                return new Response { HasError = true, ErrorMessage = "Invalid task description." };

            return await _taskManagerHubService.CreateNewTask(taskHub);
        }

        public async Task<Response> CreateMultipleNewTasks(string hashedUsername, List<TaskHub> tasks)
        {
            // Ensure the task list is not null and has items
            if (tasks == null || tasks.Count == 0)
                return new Response { HasError = true, ErrorMessage = "Task list is empty." };

            // Check individual tasks for validity
            foreach (var task in tasks)
            {
                if (string.IsNullOrWhiteSpace(task.title) || task.dueDate <= DateTime.Now)
                    return new Response { HasError = true, ErrorMessage = "One or more tasks are invalid." };
            }

            return await _taskManagerHubService.CreateMultipleNewTasks(hashedUsername, tasks);
        }

        public async Task<Response> ModifyTasks(TaskHub task, Dictionary<string, object> fieldsToUpdate)
        {
            // Validate input data
            if (string.IsNullOrWhiteSpace(task.hashedUsername) || string.IsNullOrWhiteSpace(task.title) || fieldsToUpdate == null)
                return new Response { HasError = true, ErrorMessage = "Invalid input for task modification." };
            
            if (fieldsToUpdate.ContainsKey("dueDate") && fieldsToUpdate["dueDate"] is DateTime newDueDate)
            {
                if (newDueDate < DateTime.UtcNow)
                    return new Response { HasError = true, ErrorMessage = "Due date cannot be in the past." };
            }

            if (fieldsToUpdate.ContainsKey("description"))
            {
                var newDescription = fieldsToUpdate["description"].ToString();
                if (string.IsNullOrWhiteSpace(newDescription))
                    return new Response { HasError = true, ErrorMessage = "Description invalid. Please provide more details." };
            }

            if (fieldsToUpdate.ContainsKey("priority"))
            {
                var newPriority = fieldsToUpdate["priority"].ToString().ToLower();
                var validPriorities = new HashSet<string> { "low", "medium", "high" };
                if (!validPriorities.Contains(newPriority.ToLower()))
                    return new Response { HasError = true, ErrorMessage = "Invalid task priority." };
            }
           
            return await _taskManagerHubService.ModifyTasks(task, fieldsToUpdate);
        }

        public async Task<Response> DeleteTask(TaskHub task)
        {
            // Validate that the username and task title are not empty
            if (string.IsNullOrWhiteSpace(task.hashedUsername) || string.IsNullOrWhiteSpace(task.title))
                return new Response { HasError = true, ErrorMessage = "Username or task title is invalid." };

            return await _taskManagerHubService.DeleteTask(task);
        }
    }
}

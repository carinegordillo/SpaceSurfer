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

        public async Task<Response> ListTasksByPriority(string hashedUsername, string priority)
        {
            // Implement priority validation logic if necessary
            if (string.IsNullOrWhiteSpace(hashedUsername) || string.IsNullOrWhiteSpace(priority))
                return new Response { HasError = true, ErrorMessage = "Username or priority is invalid." };

            return await _taskManagerHubService.ListTasksByPriority(hashedUsername, priority);
        }

        public async Task<Response> CreateNewTask(TaskHub taskHub)
        {
            // Implement task validation logic
            // For example, ensure that task title is not null or empty and due date is in the future
            if (string.IsNullOrWhiteSpace(taskHub.title))
                return new Response { HasError = true, ErrorMessage = "Invalid task details." };

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


using SS.Backend.SharedNamespace;

namespace SS.Backend.TaskManagerHub
{
    public interface ITaskManagerHubService
    {
        public Task<Response> ListTasks(string hashedUsername);
        public Task<Response> ListTasksByPriority(string hashedUsername, string priority);

        public Task<Response> CreateNewTask(string hashedUsername, TaskHub taskHub);

        public Task<Response> CreateMultipleNewTasks(string hashedUsername, List<TaskHub> tasks);
        public Task<Response> ModifyTasks(string hashedUsername, string title, Dictionary<string, object> fieldsToUpdate);

        public Task<Response> DeleteTask(string hashedUsername, string taskTitle);
    }
}

using SS.Backend.SharedNamespace;

namespace SS.Backend.TaskManagerHub
{
    public interface ITaskManagerHubService
    {
        public Task<Response> ListTasks(string hashedUsername);
        public Task<Response> ListTasksByPriority(string hashedUsername, string priority);
        public Task<Response> ScoreTasks(string hashedUsername);

        public Task<Response> CreateNewTask(TaskHub taskHub);

        public Task<Response> CreateMultipleNewTasks(string hashedUsername, List<TaskHub> tasks);
        public Task<Response> ModifyTasks(TaskHub task, Dictionary<string, object> fieldsToUpdate);

        public Task<Response> DeleteTask(TaskHub task);
    }
}
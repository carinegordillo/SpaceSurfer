using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.TaskManagerHub;
public interface ITaskManagerHubManager
    {
        Task<Response> ListTasks(string hashedUsername);
        Task<Response> ScoreTasks(string hashedUsername);

        Task<Response> ListTasksByPriority(string hashedUsername, string priority);
        Task<Response> CreateNewTask(TaskHub taskHub);
        Task<Response> CreateMultipleNewTasks(string hashedUsername, List<TaskHub> tasks);
        Task<Response> ModifyTasks(TaskHub task, Dictionary<string, object> fieldsToUpdate);
        Task<Response> DeleteTask(TaskHub task);
    }
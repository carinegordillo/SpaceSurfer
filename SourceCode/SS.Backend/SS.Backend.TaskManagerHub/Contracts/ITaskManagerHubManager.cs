using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.TaskManagerHub;
public interface ITaskManagerHubManager
    {
        Task<Response> ListTasks(string hashedUsername);
        Task<Response> ListTasksByPriority(string hashedUsername, string priority);
        Task<Response> CreateNewTask(string hashedUsername, TaskHub taskHub);
        Task<Response> CreateMultipleNewTasks(string hashedUsername, List<TaskHub> tasks);
        Task<Response> ModifyTasks(string hashedUsername, string title, Dictionary<string, object> fieldsToUpdate);
        Task<Response> DeleteTask(string hashedUsername, string taskTitle);
    }
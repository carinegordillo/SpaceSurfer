
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.TaskManagerHub;
namespace SS.Backend.TaskManagerHub
{

    public interface ITaskManagerHubRepo
    {
        public Task<Response> ViewTasks(string hashedUsername);
        public Task<Response> ViewTasksByPriority(string hashedUsername, string priority);

        public Task<Response> CreateTask(string hashedUsername, TaskHub taskHub);

        public Task<Response> CreateMultipleTasks(string hashedUsername, List<TaskHub> tasks);
        public Task<Response> ModifyTaskFields(string hashedUsername, string title, Dictionary<string, object> fieldsToUpdate);
        public Task<Response> DeleteTask(string hashedUsername, string taskTitle);
        

        public Task<Response> InsertIntoMultipleTables(Dictionary<string, Dictionary<string, object>> tableData);
        public Task<Response> GetCompanyFloorIDByName(string floorPlanName, int companyID);
    }
}
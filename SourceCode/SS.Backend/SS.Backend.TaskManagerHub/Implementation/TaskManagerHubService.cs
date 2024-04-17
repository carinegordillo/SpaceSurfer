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

        public async Task<Response> CreateNewTask(string hashedUsername, TaskHub taskHub){
            try{
                var response = await _taskManagerHubRepo.CreateTask(hashedUsername, taskHub);
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
    
        public async Task<Response> ModifyTasks(string hashedUsername, string title, Dictionary<string, object> fieldsToUpdate){
            try{
                var response = await _taskManagerHubRepo.ModifyTaskFields(hashedUsername, title, fieldsToUpdate);
                if (!response.HasError){
                    response.ErrorMessage += $"Successfully modified {fieldsToUpdate} in task {title}";

                }else{
                    response.ErrorMessage += $"Unable to modify {fieldsToUpdate} in task {title} - ErrorMessage {response.ErrorMessage}";
                }
                return response;
                
            }catch(Exception ex){
                return new Response{
                    HasError = true,
                    ErrorMessage = $"An unexpected error occurred while attempting to modify tasks: {ex.Message}"
                };

            }
        }

        public async Task<Response> DeleteTask(string hashedUsername, string taskTitle){
            try{
                var response = await _taskManagerHubRepo.DeleteTask(hashedUsername, taskTitle);
                if (!response.HasError){
                    response.ErrorMessage += $"Successfully deleted {taskTitle}";
                }else{
                    response.ErrorMessage += $"Unable to delete {taskTitle} - ErrorMessage {response.ErrorMessage}";
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

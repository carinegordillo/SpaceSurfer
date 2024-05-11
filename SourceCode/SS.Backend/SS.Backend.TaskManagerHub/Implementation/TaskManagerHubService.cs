// namespace SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using SS.Backend.Services.EmailService;

namespace SS.Backend.TaskManagerHub
{
    public class TaskManagerHubService : ITaskManagerHubService
    {

        private readonly ITaskManagerHubRepo _taskManagerHubRepo;
        private readonly ILogger _logger;
        private LogEntryBuilder logBuilder = new LogEntryBuilder();
        private LogEntry logEntry;

        public TaskManagerHubService(ITaskManagerHubRepo taskManagerHubRepo, ILogger logger)
        {
            _taskManagerHubRepo = taskManagerHubRepo;
            _logger = logger;
            logEntry = logBuilder.Build();
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

                //logging
                if (viewTasksResponse.HasError == false)
                {
                    logEntry = logBuilder.Info().DataStore().Description($"Successfully displays list tasks view.").User(hashedUsername).Build();
                }
                else
                {
                    logEntry = logBuilder.Error().DataStore().Description($"Failed to display list tasks view.").User(hashedUsername).Build();
                    
                }
                if (logEntry != null && _logger != null)
                {
                    _logger.SaveData(logEntry);
                }
                
                return viewTasksResponse;
            }
            catch (Exception ex)
            {
                //logging
                logEntry = logBuilder.Error().DataStore().Description($"Error to display list tasks view: Error {ex.Message}.").User(hashedUsername).Build();
                if (logEntry != null && _logger != null)
                {
                    _logger.SaveData(logEntry);
                }
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

                //logging
                if (viewTasksResponse.HasError == false)
                {
                    logEntry = logBuilder.Info().Business().Description($"Successfully list tasks by priority.").User(hashedUsername).Build();
                }
                else
                {
                    logEntry = logBuilder.Error().Business().Description($"Failed to list tasks by priority.").User(hashedUsername).Build();
                    
                }
                if (logEntry != null && _logger != null)
                {
                    _logger.SaveData(logEntry);
                }
                
                return viewTasksResponse;
            }
            catch (Exception ex)
            {
                logEntry = logBuilder.Error().Business().Description($"Error to list tasks by priority: Error {ex.Message}.").User(hashedUsername).Build();
                if (logEntry != null && _logger != null)
                {
                    _logger.SaveData(logEntry);
                }
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
                if (task.dueDate.HasValue)
                {
                    //calculate the difference in days from now to the due date
                    TimeSpan timeUntilDue = task.dueDate.Value - currentDate;
                    int daysUntilDue = timeUntilDue.Days;

                    //calculate score based on days until due, where the score decreases as the number of days increases
                   task.score = daysUntilDue > 30 ? 0 : daysUntilDue < 0 ? 40 + Math.Abs(daysUntilDue) : 30 - daysUntilDue;

                    // multiply by priority multiplier
                    switch (task.priority.ToLower())
                    {
                        case "high":
                            task.score *= 3;
                            break;
                        case "medium":
                            task.score *= 2;
                            break;
                        case "low":
                            task.score *= 1;
                            break;
                    }
                }
                else
                {
                    task.score = 0; 
                }
            }
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

                var response = new Response
                {
                    HasError = false,
                    Values = taskDictionaries
                };

                //logging
                if (viewTasksResponse.HasError == false)
                {
                    logEntry = logBuilder.Info().Business().Description($"Successfully scored tasks.").User(hashedUsername).Build();
                }
                else
                {
                    logEntry = logBuilder.Error().Business().Description($"Failed to score tasks.").User(hashedUsername).Build();
                    
                }
                if (logEntry != null && _logger != null)
                {
                    _logger.SaveData(logEntry);
                }

                return response;
            }
            catch (Exception ex)
            {
                logEntry = logBuilder.Error().Business().Description($"Error to score tasks: {ex.Message}.").User(hashedUsername).Build();
                if (logEntry != null && _logger != null)
                {
                    _logger.SaveData(logEntry);
                }
                return new Response { HasError = true, ErrorMessage = $"Failed to score tasks: {ex.Message}" };
            }
        }

        public async Task<Response> CreateNewTask(TaskHub taskHub){
            try{
                var response = await _taskManagerHubRepo.CreateTask(taskHub);
                if (!response.HasError){
                    response.ErrorMessage += "Successful task creation in database!";
                    var notifyResponse = await NotifyUser(taskHub);
                    if (notifyResponse.HasError)
                    {
                        response.HasError = true; // Propagate error status
                        response.ErrorMessage += " Notification failed: " + notifyResponse.ErrorMessage;
                    }
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

        public async Task<Response> NotifyUser(TaskHub task)
        {
            try
            {
                var emailResponse = await _taskManagerHubRepo.GetEmailByHash(task.hashedUsername);
                if (emailResponse.HasError || emailResponse.ValuesRead == null || emailResponse.ValuesRead.Rows.Count == 0)
                {
                    return new Response
                    {
                        HasError = true,
                        ErrorMessage = "Failed to retrieve email: " + (emailResponse.ErrorMessage ?? "No additional information")
                    };
                }

                // Extract email from the first DataRow
                 DataRow targetEmail= emailResponse.ValuesRead.Rows[0];
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                string userEmail = Convert.ToString(targetEmail["username"]);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (string.IsNullOrEmpty(userEmail))
                {
                    return new Response
                    {
                        HasError = true,
                        ErrorMessage = "Email address not found."
                    };
                }

                string subject = $"Reminder: Task '{task.title}'";
                string messageBody = $@"
                Hello,

                You have created a task titled '{task.title}'.

                Task details:
                {task.description}

                Due Date: {task.dueDate:yyyy-MM-dd}

                If you have any questions or need assistance, please don't hesitate to contact us at spacesurfers5@gmail.com.

                Thank you for choosing SpaceSurfer.

                Best regards,
                SpaceSurfer Team";

                // Send email immediately
                await MailSender.SendEmail(userEmail, subject, messageBody);

                var response = new Response
                {
                    HasError = false,
                    ErrorMessage = "Email notification sent successfully."
                };

                //logging
                if (response.HasError == false)
                {
                    logEntry = logBuilder.Info().Business().Description($"Successfully notified user of task {task.title}.").User(task.hashedUsername).Build();
                }
                else
                {
                    logEntry = logBuilder.Error().Business().Description($"Failed notify user of task {task.title}.").User(task.hashedUsername).Build();
                    
                }
                if (logEntry != null && _logger != null)
                {
                    _logger.SaveData(logEntry);
                }

                return response;
            }
            catch (Exception ex)
            {
                logEntry = logBuilder.Error().Business().Description($"Error to notify user of task {task.title}: {ex.Message}.").User(task.hashedUsername).Build();
                if (logEntry != null && _logger != null)
                {
                    _logger.SaveData(logEntry);
                }
                return new Response
                {
                    HasError = true,
                    ErrorMessage = $"An unexpected error occurred while attempting to send notification: {ex.Message}"
                };
            }
        }

    }
}
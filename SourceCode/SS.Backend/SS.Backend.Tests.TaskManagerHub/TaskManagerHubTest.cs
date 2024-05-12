global using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using SS.Backend.TaskManagerHub;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using SS.Backend.Services.LoggingService;


namespace SS.Backend.Tests.TaskManagerHubTests
{
    [TestClass]
    public class TaskManagerHubManagerTests
    {
        private TaskManagerHubManager _taskManagerHubManager;
        private ConfigService _configService;
        private TaskManagerHubService  _taskManagerHubService;
        private TaskManagerHubRepo _taskManagerHubRepo;
        private SqlDAO _sqlDao;
        private ILogTarget _logTarget;
        private ILogger _logger;


        [TestInitialize]
        public void Setup()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);
            _taskManagerHubRepo = new TaskManagerHubRepo(_sqlDao);
            _taskManagerHubService = new TaskManagerHubService(_taskManagerHubRepo, _logger);
            _taskManagerHubManager = new TaskManagerHubManager(_taskManagerHubService, _logger);

        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            await CleanupTestData();
        }

        private async Task CleanupTestData()
        {
           
            try
            {
                string sqlCMD = "DELETE FROM dbo.taskHub WHERE hashedUsername = 'kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      '";
                var cmd = new SqlCommand(sqlCMD);
                var response = await _sqlDao.SqlRowsAffected(cmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test cleanup: {ex}");
            }
        }

        [TestMethod]
        public async Task CreateTask_ValidData_ReturnsSuccess()
        {
            var newTask = new TaskHub
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                title = "This is a new test",
                description = "this is a success test",
                dueDate = DateTime.UtcNow.AddDays(1),
                priority = "Medium"
            };

            var result = await _taskManagerHubManager.CreateNewTask(newTask);
            Assert.IsFalse(result.HasError, "Task should be created successfully without errors.");
        }

        [TestMethod]
        public async Task CreateTask_EmptyTitle_ReturnsError()
        {
            var newTask = new TaskHub
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                title = "", // Empty title
                description = "Need to handle empty title",
                dueDate = DateTime.UtcNow.AddDays(1),
                priority = "Medium"
            };

            var result = await _taskManagerHubManager.CreateNewTask(newTask);
            Assert.IsTrue(result.HasError, "Task creation should fail due to empty title.");
        }

        [TestMethod]
        public async Task CreateTask_NullDescription_ReturnsError()
        {
            var newTask = new TaskHub
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                title = "Test Task",
                description = null, // Null description
                dueDate = DateTime.UtcNow.AddDays(1),
                priority = "Medium"
            };

            var result = await _taskManagerHubManager.CreateNewTask(newTask);
            Assert.IsTrue(result.HasError, "Task creation should fail due to null description.");
        }

        [TestMethod]
        public async Task CreateTask_PastDueDate_ReturnsError()
        {
            var newTask = new TaskHub
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                title = "Past Due Date Task",
                description = "This task has a past due date",
                dueDate = DateTime.UtcNow.AddDays(-1), // Past date
                priority = "Medium"
            };

            var result = await _taskManagerHubManager.CreateNewTask(newTask);
            Assert.IsTrue(result.HasError, "Task creation should fail due to a past due date.");
        }

        [TestMethod]
        public async Task CreateTask_InvalidPriority_ReturnsError()
        {
            var newTask = new TaskHub
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                title = "Invalid Priority Task",
                description = "This task has an invalid priority setting",
                dueDate = DateTime.UtcNow.AddDays(1),
                priority = "Undefined" // Invalid priority
            };

            var result = await _taskManagerHubManager.CreateNewTask(newTask);
            Assert.IsTrue(result.HasError, "Task creation should fail due to invalid priority.");
        }

        [TestMethod]
        public async Task CreateTask_EmptyHashedUsername_ReturnsError()
        {
            var newTask = new TaskHub
            {
                hashedUsername = "", // Empty hashed username
                title = "No User Task",
                description = "This task is assigned to no user",
                dueDate = DateTime.UtcNow.AddDays(1),
                priority = "High"
            };

            var result = await _taskManagerHubManager.CreateNewTask(newTask);
            Assert.IsTrue(result.HasError, "Task creation should fail due to empty hashed username.");
        }




        [TestMethod]
        public async Task Viewtasks_ValidData_ReturnsSuccess()
        {
            //this just views alll the tasks and scores them 
            var result = await _taskManagerHubService.ScoreTasks("hashedExampleUsername");
            Assert.IsFalse(result.HasError, "Task should be scored and arranged successfully without errors.");
        }

        [TestMethod]
        public async Task ReadTasks_ReturnsSuccess()
        {
            var hashedUsername = "hashedExampleUsername";
            var result = await _taskManagerHubManager.ListTasks(hashedUsername);
            Assert.IsFalse(result.HasError, "Task should read all tasks successfully without errors.");
        }

        // A user cant have two tasks with the same title
        [TestMethod]
        public async Task CreateTask_DuplicateTitle_ReturnsError()
        {
            var newTask = new TaskHub
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=      ",
                title = "Task2",
                description = "This task has a duplicate title",
                dueDate = DateTime.UtcNow.AddDays(10),
                priority = "Medium",
                notificationSetting = 2
            };

            var result1 = await _taskManagerHubManager.CreateNewTask(newTask);
            var result2 = await _taskManagerHubManager.CreateNewTask(newTask);
            Assert.IsTrue(result2.HasError, "Task should not be created due to duplicate title.");
        }


        [TestMethod]
        public async Task ModifyTaskFields_UpdatesSuccessfully_ReturnsSuccessMessage()
        {
            // Arrange
            var task = new TaskHub
            {
                hashedUsername = "hashedExampleUsername",
                title = "Task1"
            };
            var fieldsToUpdate = new Dictionary<string, object>
            {
                { "description", "Updated description" },
                { "dueDate", "2024-12-31" }
            };
           
            var response = await _taskManagerHubManager.ModifyTasks(task, fieldsToUpdate);
            Assert.IsFalse(response.HasError, "Task should be modified successfully.");

        }


        [TestMethod]
        public async Task UpdatesSuccessfully_ReturnsSuccessMessage()
        {
            // Arrange
            var task = new TaskHub
            {
                hashedUsername = "hashedExampleUsername",
                title = "Task2"
            };

            var jsonString = "{\"description\":\"JSON Updated description\", \"dueDate\":\"2025-12-31\"}";
            var fieldsToUpdateJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);
            var fieldsToUpdate = fieldsToUpdateJson.ToDictionary(
                kvp => kvp.Key, 
                kvp => ConvertJsonElement(kvp.Value));

            var response = await _taskManagerHubManager.ModifyTasks(task, fieldsToUpdate);
            Assert.IsFalse(response.HasError, "Task should be updated successfully.");
        }
        private object ConvertJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
#pragma warning disable CS8603 // Possible null reference return.
                    return element.GetString();
#pragma warning restore CS8603 // Possible null reference return.
                case JsonValueKind.Number:
                    return element.TryGetInt64(out long l) ? l : (object)element.GetDouble();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
#pragma warning disable CS8603 // Possible null reference return.
                    return null;
#pragma warning restore CS8603 // Possible null reference return.
                default:
                    throw new InvalidOperationException("Unsupported JsonValueKind: " + element.ValueKind);
            }
        }


        //Task cant be made in the past 
        [TestMethod]
        public async Task ModifyTask_InvalidField_ReturnsError()
        {
            // Arrange
            var newTask = new TaskHub
            {
               hashedUsername = "hashedExampleUsername",
               title = "Task1"
            };
            var fieldsToUpdate = new Dictionary<string, object>
            {
                { "dueDate", DateTime.UtcNow.AddDays(-1) } // Invalid due date in the past
            };

            var result = await _taskManagerHubManager.ModifyTasks(newTask, fieldsToUpdate);
            Assert.IsTrue(result.HasError, "Modification should fail due to invalid due date.");
        }

    }
}

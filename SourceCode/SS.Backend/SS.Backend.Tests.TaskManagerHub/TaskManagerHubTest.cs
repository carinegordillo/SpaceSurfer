global using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using SS.Backend.TaskManagerHub;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

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

        [TestInitialize]
        public void Setup()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);

            _taskManagerHubRepo = new TaskManagerHubRepo(_sqlDao);
            _taskManagerHubService = new TaskManagerHubService(_taskManagerHubRepo);
            _taskManagerHubManager = new TaskManagerHubManager(_taskManagerHubService);
        }

        [TestMethod]
        public async Task CreateTask_ValidData_ReturnsSuccess()
        {
            // Arrange
            // var hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=";
            var newTask = new TaskHub
            {
                hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=",
                title = "This is another task",
                description = "Description for new project task",
                dueDate = DateTime.UtcNow.AddDays(7),
                priority = "High",
                notificationSetting = 1
            };

            // Act
            var result = await _taskManagerHubManager.CreateNewTask(newTask);

            // Assert
            Assert.IsFalse(result.HasError, "Task should be created successfully without errors.");
        }


        [TestMethod]
        public async Task CreateMultipleTasks_ValidData_ReturnsSuccess()
        {
            // Arrange
            var hashedUsername = "hashedExampleUsername";
            var newTasks = new List<TaskHub>
            {
                new TaskHub
                {
                    title = "Testingggg Multiple againnn Tasks",
                    description = "Description for new project task",
                    dueDate = DateTime.UtcNow.AddDays(7), // Future date for validity
                    priority = "High",
                    notificationSetting = 1
                },
                new TaskHub
                {
                    title = "Testingggggggg Multiple AGAIN  New Tasks",
                    description = "Another description for new project task",
                    dueDate = DateTime.UtcNow.AddDays(7), // Future date for validity
                    priority = "High",
                    notificationSetting = 1
                }
            };

            var result = await _taskManagerHubManager.CreateMultipleNewTasks(hashedUsername, newTasks);

            // Assert
            Assert.IsFalse(result.HasError, "Task should be created successfully without errors.");
        }


        [TestMethod]
        public async Task ReadTasks_ReturnsSuccess()
        {
            // Arrange
            var hashedUsername = "validHashedUsername";
            var newTask = new TaskHub
            {
                title = "the one to delete",
                description = "Description for new project task",
                dueDate = DateTime.UtcNow.AddDays(7),
                priority = "High",
                notificationSetting = 1
            };

            // Act
            var result = await _taskManagerHubManager.ListTasks(hashedUsername);

            // Assert
            Assert.IsFalse(result.HasError, "Task should be created successfully without errors.");
        }

        [TestMethod]
        public async Task CreateTask_DuplicateTitle_ReturnsError()
        {
            // Arrange
            // var hashedUsername = "validHashedUsername";
            var newTask = new TaskHub
            {
                hashedUsername = "validHashedUsername",
                title = "New Project Task",
                description = "This task has a duplicate title",
                dueDate = DateTime.UtcNow.AddDays(10),
                priority = "Medium",
                notificationSetting = 2
            };

            // Act
            var result = await _taskManagerHubManager.CreateNewTask(newTask);

            // Assert
            Assert.IsTrue(result.HasError, "Task should not be created due to duplicate title.");
        }

        [TestMethod]
        public async Task DeleteTask_ValidTaskTitle_ReturnsSuccess()
        {
            // Arrange
            
            var newTask = new TaskHub
            {
               hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=",
               title = "This is another task"
            };


            // Act
            var result = await _taskManagerHubManager.DeleteTask(newTask);

            // Assert
            Assert.IsFalse(result.HasError, "Task should be deleted successfully.");
        }


        [TestMethod]
        public async Task ModifyTaskFields_UpdatesSuccessfully_ReturnsSuccessMessage()
        {
            // Arrange
            var task = new TaskHub
            {
                hashedUsername = "hashedExampleUsername",
                title = "Multiple Tasks"
            };
            var fieldsToUpdate = new Dictionary<string, object>
            {
                { "description", "Updated description" },
                { "dueDate", "2024-12-31" }
            };
            // var expectedResponse = new Response { HasError = false, ErrorMessage = "Task updated successfully." };

           
            var response = await _taskManagerHubManager.ModifyTasks(task, fieldsToUpdate);

            Assert.IsFalse(response.HasError, "Task should be deleted successfully.");

        }

        [TestMethod]
        public async Task ModifyTask_InvalidField_ReturnsError()
        {
            // Arrange
            var newTask = new TaskHub
            {
               hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=",
               title = "This is another task"
            };
            var fieldsToUpdate = new Dictionary<string, object>
            {
                { "dueDate", DateTime.UtcNow.AddDays(-1) } // Invalid due date in the past
            };

            // Act
            var result = await _taskManagerHubManager.ModifyTasks(newTask, fieldsToUpdate);

            // Assert
            Assert.IsTrue(result.HasError, "Modification should fail due to invalid due date.");
        }

        [TestMethod]
        public async Task ListTasksByPriority_NoTasksFound_ReturnsEmpty()
        {
            // Arrange
            var hashedUsername = "validHashed";
            var priority = "High";

            // Act
            var result = await _taskManagerHubManager.ListTasksByPriority(hashedUsername, priority);

            // Assert
            Assert.IsTrue(result.HasError, "Should handle no tasks found gracefully.");
            Assert.IsNull(result.ValuesRead, "No tasks should be found for the specified priority.");
        }

        // [TestCleanup]
        // public async Task Cleanup()
        // {
        //     // Optional: Implement any cleanup logic if necessary
        //     await _sqlDao.ExecuteCommandAsync("DELETE FROM TaskHub WHERE hashedUsername = @hashedUsername",
        //         new Dictionary<string, object> { { "@hashedUsername", "validHashedUsername" } });
        // }
    }
}

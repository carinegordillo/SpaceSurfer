// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using System;
// using System.IO;
// using System.Threading;
// using System.Threading.Tasks;
// using SS.Backend.Services.ArchivingService;
// using SS.Backend.SharedNamespace;

// namespace SS.Backend.Tests.Archiving
// {
//     [TestClass]
//     public class ArchivingServiceTests
//     {
//         private MockTargetArchivingDestination _mockArchivingDestination;
//         private ArchivingService _archivingService;
//         private string _configFilePath;

//         // [TestInitialize]
//         // public void Setup()
//         // {
//         //     _mockArchivingDestination = new MockTargetArchivingDestination();

//         //     // Create a temporary config file for testing
//         //     _configFilePath = Path.GetTempFileName();
//         //     File.WriteAllText(_configFilePath, @"
//         //     {
//         //         ""StartDate"": ""2024-05-14T00:00:00"",
//         //         ""Interval"": 1,
//         //         ""Unit"": ""Days""
//         //     }");

//         //     _archivingService = new ArchivingService(_mockArchivingDestination, _configFilePath);
//         // }

//         // [TestCleanup]
//         // public void Cleanup()
//         // {
//         //     // Delete the temporary config file
//         //     if (File.Exists(_configFilePath))
//         //     {
//         //         File.Delete(_configFilePath);
//         //     }
//         // }

//         // [TestMethod]
//         // public void Start_ShouldBeginArchivingProcess()
//         // {
//         //     // Act
//         //     _archivingService.Start();

//         //     // Wait for the thread to potentially start
//         //     Thread.Sleep(1000);

//         //     // Assert
//         //     Assert.IsTrue(_archivingService.IsRunning);
//         // }

//         // [TestMethod]
//         // public void Stop_ShouldEndArchivingProcess()
//         // {
//         //     // Arrange
//         //     _archivingService.Start();

//         //     // Act
//         //     _archivingService.Stop();

//         //     // Assert
//         //     Assert.IsFalse(_archivingService.IsRunning);
//         // }

//         // [TestMethod]
//         // public void GetNextRunTime_ShouldCalculateNextRunBasedOnInterval()
//         // {
//         //     // Arrange
//         //     DateTime expectedNextRun = _archivingService.LastRun.AddDays(1);

//         //     // Act
//         //     DateTime actualNextRun = _archivingService.GetNextRunTime();

//         //     // Assert
//         //     Assert.AreEqual(expectedNextRun, actualNextRun);
//         // }

//         [TestMethod]
//         public async Task PerformScheduledTask_ShouldUploadFileAndResetTable()
//         {
//             // Arrange
//             _mockArchivingDestination.ThrowException = false;

//             // Act
//             await Task.Run(() => _archivingService.PerformScheduledTask());

//             // Assert
//             Assert.IsTrue(_mockArchivingDestination.UploadedFiles.Count > 0);
//             Assert.AreEqual("space-surfer-archives", _mockArchivingDestination.UploadedFiles[0].destination);
//         }

//         [TestMethod]
//         public async Task PerformScheduledTask_ShouldHandleUploadException()
//         {
//             // Arrange
//             _mockArchivingDestination.ThrowException = true;

//             // Act & Assert
//             var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await Task.Run(() => _archivingService.PerformScheduledTask()));
//             Assert.AreEqual("Simulated exception", ex.Message);
//         }
//     }
// }

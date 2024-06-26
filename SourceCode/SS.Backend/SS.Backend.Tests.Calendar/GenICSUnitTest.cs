using SS.Backend.Services.CalendarService;

namespace SS.Backend.Tests.Calendar
{

    [TestClass]
    public class GenICSUnitTest
    {
        [TestMethod]
        public async Task GenICS_Success()
        {
            // Arrange
            var tempFilePath = Path.GetTempFileName();
            var reservationInfo = new ReservationInfo
            {
                filePath = tempFilePath,
                eventName = "Test Event",
                start = DateTime.UtcNow,
                end = DateTime.UtcNow.AddHours(1),
                description = "Test Description",
                location = "Test Location"
            };

            var calendarCreator = new CalendarCreator();

            // Act
            await calendarCreator.CreateCalendar(reservationInfo);

            // Assert
            Assert.IsTrue(File.Exists(tempFilePath), "The ICS file was not created.");

            string fileContents = File.ReadAllText(tempFilePath);
            Console.WriteLine("Contents of the ICS file:");
            Console.WriteLine(fileContents);

            // Additional assertions can go here...

            // Cleanup
            File.Delete(tempFilePath);
        }
    }
}
// using SS.Backend.EmailConfirm;
// using SS.Backend.SharedNamespace;
// using SS.Backend.DataAccess;
// using Microsoft.Data.SqlClient;
// using System.Diagnostics;
// using SS.Backend.ReservationManagement;
<<<<<<< Updated upstream
// using SS.Backend.Services.LoggingService;
=======
>>>>>>> Stashed changes

// namespace SS.Backend.Tests.EmailConfirm;

// [TestClass]
// public class SendConfirmationUnitTest
// {
//     private EmailConfirmSender _emailSender;
//     private EmailConfirmService _emailConfirm;
//     private IEmailConfirmDAO _emailDAO;
//     private SqlDAO _sqlDao;
//     private ConfigService _configService;
<<<<<<< Updated upstream
//     private ILogTarget _logTarget;
//     private ILogger _logger;
=======
>>>>>>> Stashed changes

//     [TestInitialize]
//     public void Setup()
//     {
        
//         var baseDirectory = AppContext.BaseDirectory;
//         var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//         var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
//         _configService = new ConfigService(configFilePath);
<<<<<<< Updated upstream
//         _logger = new Logger(_logTarget);
//         _sqlDao = new SqlDAO(_configService);
//         _emailDAO = new EmailConfirmDAO(_sqlDao);
//         _emailConfirm = new EmailConfirmService(_emailDAO);
//         _emailSender = new EmailConfirmSender(_emailConfirm, _emailDAO, _logger);
=======
//         _sqlDao = new SqlDAO(_configService);
//         _emailDAO = new EmailConfirmDAO(_sqlDao);
//         _emailConfirm = new EmailConfirmService(_emailDAO);
//         _emailSender = new EmailConfirmSender(_emailConfirm, _emailDAO);
>>>>>>> Stashed changes
//     }

//     private async Task CleanupTestData()
//     {
//         var baseDirectory = AppContext.BaseDirectory;
//         var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//         var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

//         ConfigService configFile = new ConfigService(configFilePath);
//         var connectionString = configFile.GetConnectionString();
//         try
//         {
//             using (SqlConnection connection = new SqlConnection(connectionString))
//             {
//                 await connection.OpenAsync().ConfigureAwait(false);

<<<<<<< Updated upstream
//                 string sql1 = $"DELETE FROM dbo.ConfirmReservations WHERE [reservationID] = '3'";
=======
//                 string sql1 = $"DELETE FROM dbo.ConfirmReservations WHERE [reservationID] = '5'";
>>>>>>> Stashed changes

//                 using (SqlCommand command1 = new SqlCommand(sql1, connection))
//                 {
//                     await command1.ExecuteNonQueryAsync().ConfigureAwait(false);
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Exception during test cleanup: {ex}");
//         }
//     }

//     [TestMethod]
//     public async Task GetUsername_Success()
//     {
//         //Arrange
//         Stopwatch timer = new Stopwatch();
//         Response result = new Response();
<<<<<<< Updated upstream
//         int reservationID = 3;
=======
//         int reservationID = 5;
>>>>>>> Stashed changes
//         string userHash = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=";
//         result = await _emailDAO.GetUsername(userHash);
//         string email = "4sarahsantos@gmail.com";

//         //Act
//         timer.Start();
//         result = await _emailDAO.GetUsername(userHash);
//         string targetEmail = result.ValuesRead.Rows[0]["username"].ToString();
//         if(targetEmail != email)
//         {
//             result.HasError = true;
//             result.ErrorMessage = $"{targetEmail} is not equal to {email}";
//         }
//         timer.Stop();

//         //Assert
//         Assert.IsFalse(result.HasError, result.ErrorMessage);
//         Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

//         //Cleanup
//         await CleanupTestData().ConfigureAwait(false);
//     }

//     [TestMethod]
<<<<<<< Updated upstream
//     public async Task GetUserReservationById_Success()
//     {
//         //Arrange
//         Stopwatch timer = new Stopwatch();
//         //Response result = new Response();
//         int reservationID = 14;

//         //Act
//         timer.Start();
//         var(reservation, result) = await _emailDAO.GetUserReservationByID(reservationID);
=======
//     public async Task SendEmail_Success()
//     {
//         //Arrange
//         Stopwatch timer = new Stopwatch();
//         Response result = new Response();
//         UserReservationsModel reservation = new UserReservationsModel
//         {
//             ReservationID = 3, // Assuming it's not set yet if it's a new reservation
//             CompanyID = 6, // Example company ID
//             FloorPlanID = 4, // Example floor plan ID
//             SpaceID = "SPACE101", // Identifier for the specific space being reserved
//             ReservationStartTime = new DateTime(2024, 4, 1, 11, 0, 0), // May 21, 2024, 14:00
//             ReservationEndTime = new DateTime(2024, 4, 1, 14, 0, 0), // May 21, 2024, 16:00
//             Status = ReservationStatus.Active, // Assuming the reservation is currently active
//             UserHash = "4sarahsantos@gmail.com"//"7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU="
//         };
//         //(string icsFile, string otp, string html, result) = await _emailConfirm.CreateConfirmation(reservationID);

//         //Act
//         timer.Start();
//         result = await _emailSender.SendConfirmation(reservation);
>>>>>>> Stashed changes
//         timer.Stop();

//         //Assert
//         Assert.IsFalse(result.HasError, result.ErrorMessage);
<<<<<<< Updated upstream
//         Assert.IsNotNull(reservation);
//         Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

//         //Cleanup
//         await CleanupTestData().ConfigureAwait(false);
//     }

//     [TestMethod]
//     public async Task SendEmail_Success()
//     {
//         //Arrange
//         Stopwatch timer = new Stopwatch();
//         Response result = new Response();
//         UserReservationsModel reservation = new UserReservationsModel
//         {
//             ReservationID = 3, // Assuming it's not set yet if it's a new reservation
//             CompanyID = 6, // Example company ID
//             FloorPlanID = 4, // Example floor plan ID
//             SpaceID = "SPACE101", // Identifier for the specific space being reserved
//             ReservationStartTime = new DateTime(2024, 4, 1, 11, 0, 0), // May 21, 2024, 14:00
//             ReservationEndTime = new DateTime(2024, 4, 1, 14, 0, 0), // May 21, 2024, 16:00
//             Status = ReservationStatus.Active, // Assuming the reservation is currently active
//             UserHash = "7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU="
//         };
//         //(string icsFile, string otp, string html, result) = await _emailConfirm.CreateConfirmation(reservationID);

//         //Act
//         timer.Start();
//         result = await _emailSender.SendConfirmation(reservation);
//         timer.Stop();

//         //Assert
//         Assert.IsFalse(result.HasError, result.ErrorMessage);
//         Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

//         //Cleanup
//         await CleanupTestData().ConfigureAwait(false);
=======
//         Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

//         //Cleanup
//         //await CleanupTestData().ConfigureAwait(false);
>>>>>>> Stashed changes
//     }


//     // [TestMethod]
//     // public async Task ResendConfirm_InvalidInputs_Fail()
//     // {
//     //     //Arrange
//     //     Stopwatch timer = new Stopwatch();
//     //     Response result = new Response();
//     //     int reservationID = -1;
//     //     var getOtp = new GenOTP();
//     //     var newOtp = getOtp.generateOTP();
//     //     (string icsFile, string otp, string html, result) = await _emailConfirm.CreateConfirmation(reservationID);

//     //      //Act
//     //     timer.Start();
//     //     result = await _emailConfirm.ConfirmReservation(reservationID, newOtp);
//     //     timer.Stop();

//     //     //Assert
//     //     Assert.IsTrue(result.HasError, "Expected ConfirmReservation to fail with invalid input.");
//     //     Assert.IsFalse(string.IsNullOrEmpty(result.ErrorMessage), "Expected an error message for invalid input.");
//     //     Assert.IsNotNull(icsFile);
//     //     Assert.IsNotNull(otp);
//     //     Assert.IsNotNull(html);
//     //     Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

//     //     //Cleanup
//     //     await CleanupTestData().ConfigureAwait(false);
//     // }

<<<<<<< Updated upstream
//     [TestMethod]
//     public async Task CreateConfirm_Timeout_Fail()
//     {
//         //Arrange
//         int reservationID = 5;
//         var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(3000));
//         (string icsFile, string otp, string html, Response result)= await _emailConfirm.CreateConfirmation(reservationID);

//         //Act
//         var operationTask =  _emailConfirm.ConfirmReservation(reservationID, otp);
//         var completedTask = await Task.WhenAny(operationTask, timeoutTask);

//         // Assert
//         if (completedTask == operationTask)
//         {
//             // Operation completed before timeout, now it's safe to await it and check results
//             result = await operationTask;

//             // Assert the operation's success
//             Assert.IsFalse(result.HasError, result.ErrorMessage);
//             Assert.IsNotNull(otp);
//         }
//         else
//         {
//             // Fail the test if we hit the timeout
//             Assert.Fail("The ConfirmReservation operation timed out.");
//         }

//         //Cleanup
//         await CleanupTestData().ConfigureAwait(false);
//     }
=======
//     // [TestMethod]
//     // public async Task CreateConfirm_Timeout_Fail()
//     // {
//     //     //Arrange
//     //     int reservationID = 5;
//     //     var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(3000));
//     //     (string icsFile, string otp, string html, Response result)= await _emailConfirm.CreateConfirmation(reservationID);

//     //     //Act
//     //     var operationTask =  _emailConfirm.ConfirmReservation(reservationID, otp);
//     //     var completedTask = await Task.WhenAny(operationTask, timeoutTask);

//     //     // Assert
//     //     if (completedTask == operationTask)
//     //     {
//     //         // Operation completed before timeout, now it's safe to await it and check results
//     //         result = await operationTask;

//     //         // Assert the operation's success
//     //         Assert.IsFalse(result.HasError, result.ErrorMessage);
//     //         Assert.IsNotNull(otp);
//     //     }
//     //     else
//     //     {
//     //         // Fail the test if we hit the timeout
//     //         Assert.Fail("The ConfirmReservation operation timed out.");
//     //     }

//     //     //Cleanup
//     //     await CleanupTestData().ConfigureAwait(false);
//     // }
>>>>>>> Stashed changes

// }
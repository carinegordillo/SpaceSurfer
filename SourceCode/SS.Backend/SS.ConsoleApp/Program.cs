using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Diagnostics;
using SS.Backend.Waitlist;
using System.Data;
using System.Text;
using System;
using SS.Backend.SpaceManager;
using SS.Backend.Services.EmailService;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

internal class Program
{
    //private static Response result;
    //private static CustomSqlCommandBuilder builder;
    //private static ConfigService configService;
    //private static SqlDAO dao;
    //private static WaitlistService waitlistService;
    //private static ReservationCreatorService _ReservationCreatorService;
    //private static ReservationManagementRepository _reservationManagementRepository;
    //private static ReservationValidationService _reservationValidationService;
    //private static ReservationCreationManager _reservationCreationManager;
    //private static ReservationCancellationService _reservationCancellationService;

    public static void Main(string[] args)
    {
        //result = new Response();
        //builder = new CustomSqlCommandBuilder();

        //var baseDirectory = AppContext.BaseDirectory;
        //var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        //var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

        //configService = new ConfigService(configFilePath);
        //dao = new SqlDAO(configService);
        //waitlistService = new WaitlistService(dao);

        //_reservationManagementRepository = new ReservationManagementRepository(dao);
        //_reservationValidationService = new ReservationValidationService(_reservationManagementRepository);
        //_ReservationCreatorService = new ReservationCreatorService(_reservationManagementRepository, waitlistService);
        //_reservationCreationManager = new ReservationCreationManager(_ReservationCreatorService, _reservationValidationService, waitlistService);
        //_reservationCancellationService = new ReservationCancellationService(_reservationManagementRepository, waitlistService);

        //// Arrange
        //string userHash = "testHash";
        //string userHash2 = "testHash2";
        //string userHash3 = "testHash3";
        //string tableName = "TestReservations";
        //string testEmail = "test@email.com";

        //UserReservationsModel userReservationsModel = new UserReservationsModel
        //{
        //    CompanyID = 1,
        //    FloorPlanID = 1,
        //    SpaceID = "OS-02",
        //    ReservationStartTime = new DateTime(2024, 02, 15, 14, 00, 00),
        //    ReservationEndTime = new DateTime(2024, 02, 15, 15, 00, 00),
        //    Status = ReservationStatus.Active,
        //    UserHash = userHash2
        //};

        //string sql = $"INSERT INTO TestReservations VALUES ({userReservationsModel.CompanyID},{userReservationsModel.FloorPlanID},'{userReservationsModel.SpaceID}','{userReservationsModel.ReservationStartTime}','{userReservationsModel.ReservationEndTime}','{userReservationsModel.Status}','{userHash}')";
        //var cmd = new SqlCommand(sql);
        //var response = await dao.SqlRowsAffected(cmd);
        //Console.WriteLine("Passed first insert");

        //int resId = await waitlistService.GetReservationID(tableName, 1, 1, "OS-02", userReservationsModel.ReservationStartTime, userReservationsModel.ReservationEndTime);
        //Console.WriteLine("resId: " + resId);

        //sql = $"INSERT INTO Waitlist VALUES ('{userHash}', {resId}, 0)";
        //cmd = new SqlCommand(sql);
        //response = await dao.SqlRowsAffected(cmd);
        //sql = $"INSERT INTO Waitlist VALUES ('{userHash2}', {resId}, 1)";
        //cmd = new SqlCommand(sql);
        //response = await dao.SqlRowsAffected(cmd);
        //sql = $"INSERT INTO Waitlist VALUES ('{userHash3}', {resId}, 2)";
        //cmd = new SqlCommand(sql);
        //response = await dao.SqlRowsAffected(cmd);
        //Console.WriteLine("Passed waitlist inserts");

        ////// Act
        //response = await _reservationCancellationService.CancelReservationAsync(tableName, resId);
        //Console.WriteLine("Passed cancel");

        //sql = $"SELECT * FROM Waitlist WHERE Username = '{userHash}'";
        //cmd = new SqlCommand(sql);
        //response = await dao.ReadSqlResult(cmd);
        //bool empty = false;
        //if (response.ValuesRead == null)
        //{
        //    empty = true;
        //}

        //int userPosition = await waitlistService.GetWaitlistPosition(userHash2, resId);
        //int userPosition2 = await waitlistService.GetWaitlistPosition(userHash3, resId);

        //bool isPositionCorrect = false;
        //if (userPosition == 1 && userPosition2 == 2)
        //{
        //    isPositionCorrect = true;
        //}

        //Console.WriteLine(empty);
        //Console.WriteLine(isPositionCorrect);

    }
}
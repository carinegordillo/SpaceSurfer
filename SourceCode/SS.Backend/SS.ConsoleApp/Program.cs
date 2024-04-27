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
using SS.Backend.Services.ArchivingService;

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
        

        ITargetArchivingDestination s3ArchivingDestination = new S3ArchivingDestination();

        Console.WriteLine("Hello World!");


        TwoMinuteArchivingService twoMinuteArchivingService = new TwoMinuteArchivingService(s3ArchivingDestination);
        twoMinuteArchivingService.Start();
        
        
    }
}
using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Diagnostics;
using SS.Backend.Waitlist;
using SS.Backend.UserDataProtection;
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
using Org.BouncyCastle.Bcpg;

internal class Program
{
    private Response result;
    private CustomSqlCommandBuilder builder;
    private ConfigService configService;
    private SqlDAO dao;
    private UserDataProtection udp;

    //public static void Main(string[] args)
    //{


    //    ITargetArchivingDestination s3ArchivingDestination = new S3ArchivingDestination();

    //    Console.WriteLine("Hello World!");


    //    TwoMinuteArchivingService twoMinuteArchivingService = new TwoMinuteArchivingService(s3ArchivingDestination);
    //    twoMinuteArchivingService.Start();


    //}

    public static async Task Main(string[] args)
    {
        //var result = new Response();
        //var builder = new CustomSqlCommandBuilder();

        //var baseDirectory = AppContext.BaseDirectory;
        //var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        //var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

        //var configService = new ConfigService(configFilePath);
        //var dao = new SqlDAO(configService);

        //string userHash = "AFFKahiwAYT+JMsdsVLyF+WWV9nKpyTvU1gaTp9Z+q4=";
        ////string userHash = "fEdvfXkpLAh88eklCsiNFuV9wMR4c846K+i/d8EW6u0=";

        //var userDataProtection = new UserDataProtection(dao);

        //UserDataModel userData = await userDataProtection.accessData_Manager(userHash);

        //string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceSurfers_UserData");

        //if (userData != null)
        //{
        //    await userDataProtection.WriteToFile_GeneralUser(userData, outputPath);
        //    Console.WriteLine("Successfully wrote to file.");
        //    await userDataProtection.sendAccessEmail(userData, outputPath);
        //    Console.WriteLine("Successfully sent email.");
        //}
        //else
        //{
        //    Console.WriteLine("Failed to retrieve user data.");
        //}

        
        
        
    }
}
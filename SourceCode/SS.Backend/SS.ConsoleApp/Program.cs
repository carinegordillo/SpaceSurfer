using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Diagnostics;
using SS.Backend.Waitlist;
using System.Data;
using System.Text;
using System;
using SS.Backend.SpaceManager;

internal class Program
{
    //public static async Task<IEnumerable<CompanyFloorStrImage>> GetCompanyFloorsAsync(int companyId)
    //{
    //    var floors = new Dictionary<int, CompanyFloorStrImage>();

    //    var commandBuilder = new CustomSqlCommandBuilder();
    //    var command = commandBuilder.BeginStoredProcedure("GetCompanyFloorPROD")
    //                                .AddParameters(new Dictionary<string, object> { { "companyId", companyId } })
    //                                .Build();

    //    Console.WriteLine("Command: " + command.CommandText);

    //    Response response = await _spaceManagerDao.ExecuteReadCompanyTables(command);

    //    Console.WriteLine(response.ErrorMessage);
    //    if (!response.HasError && response.ValuesRead != null)
    //    {
    //        foreach (DataRow row in response.ValuesRead.Rows)
    //        {
    //            int floorPlanID = row["floorPlanID"] as int? ?? default(int);
    //            string? floorPlanName = row["floorPlanName"]?.ToString().Trim();
    //            byte[]? floorPlanImage = row["floorPlanImage"] as byte[];
    //            string? spaceID = row["spaceID"]?.ToString().Trim();
    //            int timeLimit = row["timeLimit"] as int? ?? default;

    //            CompanyFloorStrImage? floor;
    //            if (!floors.TryGetValue(floorPlanID, out floor))
    //            {
    //                floor = new CompanyFloorStrImage
    //                {
    //                    FloorPlanID = floorPlanID,
    //                    FloorPlanName = floorPlanName ?? string.Empty,
    //                    FloorPlanImageBase64 = floorPlanImage != null ? Convert.ToBase64String(floorPlanImage) : null,
    //                };
    //                floors.Add(floorPlanID, floor);
    //            }

    //            floor.FloorSpaces[spaceID] = timeLimit;
    //        }
    //    }
    //    else
    //    {
    //        Console.WriteLine("No data found or error occurred.");
    //    }

    //    Console.WriteLine("Returning floors");

    //    return floors.Values;
    //}

    public static async Task Main(string[] args)
    {
        //Response result = new Response();
        //var baseDirectory = AppContext.BaseDirectory;
        //var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
        //var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
        //ConfigService configService = new ConfigService(configFilePath);
        //SqlDAO dao = new SqlDAO(configService);
        //SpaceManagerDao spacedao = new SpaceManagerDao(dao);
        //SpaceReader reader = new SpaceReader(spacedao);

        //await GetCompanyFloorsAsync(1);
    }
}
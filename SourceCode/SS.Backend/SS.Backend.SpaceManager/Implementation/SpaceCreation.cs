// namespace SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
// using SpaceCreation;

namespace SS.Backend.SpaceManager
{
    public class SpaceCreation : ISpaceCreation
    {
        public async Task<Response> CreateSpace(CompanyInfo? companyInfo, CompanyFloor? companyFloor)
        {
            Response response = new Response();
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO SQLDao = new SqlDAO(configService);
            Logger logger = new Logger(new SqlLogTarget(SQLDao));


            // Temporary business rules
            if (companyFloor is null || companyFloor.FloorPlanName == null)
            {
                response.ErrorMessage = "CompanyFloor cannot be null.";
                return response; // Or handle the null case appropriately
            }
            // Insert companyFloor entry
            var companyFloorParameters = new Dictionary<string, object>
            {
                // Assuming companyID is already available
                {"companyID", 4/* fetch or assume companyID here */},
                {"floorPlanName", companyFloor.FloorPlanName ?? "Name is Null"},
                {"floorPlanImage", companyFloor.FloorPlanImage ?? new byte[0]}
            };

            var floorTableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "companyFloor", companyFloorParameters }
            };

            // Insert the company floor first
            var floorInsertResponse = await InsertIntoMultipleTables(floorTableData);
            if (floorInsertResponse.HasError)
            {
                return floorInsertResponse; // Return or handle error
            }
            int companyID = Convert.ToInt32(companyFloorParameters["companyID"]);
            // Assuming floorPlanID retrieval method after insert
            if (companyFloor.FloorPlanName != null)
            {
                // Response tableResponse = new Response();
                Response tableResponse = await GetCompanyFloorIDByName(companyFloor.FloorPlanName, companyID);
                // DataTable? floorPlanID = await GetCompanyFloorIDByName(companyFloor.FloorPlanName);
                if(tableResponse.ValuesRead != null)
                {
                    foreach (DataRow row in tableResponse.ValuesRead.Rows)
                    {
                        int floorPlanID = Convert.ToInt32(row["floorPlanID"]);
                        Console.WriteLine($"Debug: Floor Plan ID  is {floorPlanID}"); 
                        if (floorPlanID > 0){
                            // Use the ListSpace method to prepare data for each space
                            var spaceList = ListSpace(companyFloor);

                            // Insert each space entry related to the companyFloor
                            foreach (var spaceDict in spaceList)
                            {
                                // Add floorPlanID to each dictionary since it's now available
                                spaceDict.Add("floorPlanID", floorPlanID);
                                spaceDict.Add("companyID", companyID);

                                var spaceTableData = new Dictionary<string, Dictionary<string, object>>
                                {
                                    { "companyFloorSpaces", spaceDict }
                                };

                                var spaceInsertResponse = await InsertIntoMultipleTables(spaceTableData);
                                if (spaceInsertResponse.HasError)
                                {
                                    return spaceInsertResponse;
                                }
                            }
                        }   
                    }
                    
                }

                
            }
            
            // Return response based on overall operation success or specific error handling
            return floorInsertResponse; // This should be replaced with appropriate response handling
        }

        public List<Dictionary<string, object>> ListSpace(CompanyFloor? companyFloor)
        {
            var listOfDicts = new List<Dictionary<string, object>>();

            if (companyFloor?.FloorSpaces != null)
            {
                foreach (var space in companyFloor.FloorSpaces)
                {
                    var spaceDict = new Dictionary<string, object>
                    {
                        {"spaceID", space.Key},
                        {"timeLimit", space.Value}
                    };
                    listOfDicts.Add(spaceDict);
                }
            }

            return listOfDicts;
        }

        public async Task<Response> InsertIntoMultipleTables(Dictionary<string, Dictionary<string, object>> tableData)
        {   
            // SealedSqlDAO SQLDao = new SealedSqlDAO(temp);
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO SQLDao = new SqlDAO(configService);

            var builder = new CustomSqlCommandBuilder();
            Response tablesresponse = new Response();

            //for each table 
            foreach (var tableEntry in tableData)
            {
                string tableName = tableEntry.Key;
                Dictionary<string, object> parameters = tableEntry.Value;
                var insertCommand = builder.BeginInsert(tableName)
                    .Columns(parameters.Keys)
                    .Values(parameters.Keys)
                    .AddParameters(parameters)
                    .Build();

                tablesresponse = await SQLDao.SqlRowsAffected(insertCommand);
                if (tablesresponse.HasError)
                {
                    tablesresponse.ErrorMessage += $"{tableName}: error inserting data; ";
                    return tablesresponse;
                }
            }
            return tablesresponse;
        }


        public async Task<Response> GetCompanyFloorIDByName(string floorPlanName, int companyID)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO SQLDao = new SqlDAO(configService);


            Response tablesresponse = new Response();
            var builder = new CustomSqlCommandBuilder();
            var command = builder.BeginSelect()
                                .SelectColumns("floorPlanID")
                                .From("companyFloor")
                                .Where("floorPlanName = @FloorPlanName AND companyID = @CompanyID")
                                .AddParameters(new Dictionary<string, object>
                                {
                                    { "FloorPlanName", floorPlanName },
                                    { "CompanyID", companyID }
                                })
                                .Build();


            var IDresponse = await SQLDao.ReadSqlResult(command);
            if (IDresponse.HasError)
            {
                tablesresponse.ErrorMessage += $"Error selecting floor ID; ";
            }
            return IDresponse;
            
        }

        public async Task<Response> ReadUserTable(string tableName)
        {

            // var baseDirectory = AppContext.BaseDirectory;
            // var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            // var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            ConfigService configService = new ConfigService(configFilePath);
            SqlDAO SQLDao = new SqlDAO(configService);
            Response response = new Response();
            var commandBuilder = new CustomSqlCommandBuilder();
            
            var insertCommand =  commandBuilder.BeginSelectAll()
                                            .From(tableName)
                                            .Build();

            response = await SQLDao.ReadSqlResult(insertCommand);
            if (response.HasError)
            {
                response.ErrorMessage += $"{tableName}: error inserting data; ";
                return response;
            }else{
                response.ErrorMessage += "- ReadUserTable- command successful -";
            }
          
            return response;

        }
        
    }
}

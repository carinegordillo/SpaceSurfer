// namespace SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;


namespace SS.Backend.SpaceManager
{
    public class SpaceCreation : ISpaceCreation
    {

        private readonly ISpaceManagerDao _spaceManagerDao;

        public SpaceCreation(ISpaceManagerDao spaceManagerDao)
        {
            _spaceManagerDao = spaceManagerDao;
        }
        public async Task<Response> CreateSpace(string hashedUsername, CompanyFloor? companyFloor)
        {
            Response response = new Response();
            var companyIDResponse = await _spaceManagerDao.GetCompanyIDByHashedUsername(hashedUsername);

            if (companyIDResponse.HasError || companyIDResponse.ValuesRead == null || companyIDResponse.ValuesRead.Rows.Count == 0)
            {
                response.ErrorMessage = "Invalid hashedUsername or companyID not found.";
                return response;
            }

            // Assuming we have only one row per hashedUsername
            DataRow companyIDRow = companyIDResponse.ValuesRead.Rows[0];
            int companyID = Convert.ToInt32(companyIDRow["companyID"]);
            Console.WriteLine($"Debug: CompanyID is {companyID}");
            // Response response = new Response();
                
            if (companyID <= 0)
            {
                response.ErrorMessage = "companyID is not valid.";
                return response;
            }
            // Temporary business rules
            if (companyFloor is null || companyFloor.FloorPlanName == null || companyFloor.FloorPlanImage == null || companyFloor.FloorSpaces == null)
            {
                response.ErrorMessage = "CompanyFloor cannot be null.";
                return response; // Or handle the null case appropriately
            }
            // Insert companyFloor entry
            var companyFloorParameters = new Dictionary<string, object>
            {
                // Assuming companyID is already available
                {"companyID", companyID},
                {"floorPlanName", companyFloor.FloorPlanName ?? "Name is Null"},
                {"floorPlanImage", companyFloor.FloorPlanImage ?? new byte[0]}
            };

            var floorTableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "companyFloor", companyFloorParameters }
            };

            // Insert the company floor first
            var floorInsertResponse = await _spaceManagerDao.InsertIntoMultipleTables(floorTableData);
            if (floorInsertResponse.HasError)
            {
                return floorInsertResponse; // Return or handle error
            }
            // int companyID = Convert.ToInt32(companyFloorParameters["companyID"]);
            if (companyFloor.FloorPlanName != null)
            {
                // Response tableResponse = new Response();
                Response tableResponse = await _spaceManagerDao.GetCompanyFloorIDByName(companyFloor.FloorPlanName, companyID);
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

                                var spaceInsertResponse = await _spaceManagerDao.InsertIntoMultipleTables(spaceTableData);
                                if (spaceInsertResponse.HasError)
                                {
                                    return spaceInsertResponse;
                                }
                            }
                        }   
                    }
                    
                }

                
            }
            if (floorInsertResponse.HasError == false)
            {
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Info",
                    username = hashedUsername,
                    category = "Data Store",
                    description = "Successful floor creation"
                };
                //await logger.SaveData(entry);
            }
            else
            {
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = hashedUsername,
                    category = "Data Store",
                    description = "Error inserting floor info in data store."
                };
                // await logger.SaveData(entry);
            }
            
            // Return response based on overall operation success or specific error handling
            return floorInsertResponse; 
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
    }
}

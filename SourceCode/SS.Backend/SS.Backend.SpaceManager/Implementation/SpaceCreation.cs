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
        public async Task<Response> CreateSpace(CompanyFloor? companyFloor)
        {
            Response response = new Response();
           // Attempt to retrieve the company ID using the hashed username
            var companyIDResponse = await _spaceManagerDao.GetCompanyIDByHashedUsername(companyFloor.hashedUsername);

            // Validate the response for errors or empty data
            if (companyIDResponse.HasError || companyIDResponse.ValuesRead == null || companyIDResponse.ValuesRead.Rows.Count == 0)
            {
                response.ErrorMessage = "Invalid hashedUsername or companyID not found.";
                return response;
            }

            // Extract company ID from the response
            DataRow companyIDRow = companyIDResponse.ValuesRead.Rows[0];
            int companyID = Convert.ToInt32(companyIDRow["companyID"]);

            // Validate the company ID
            if (companyID <= 0)
            {
                response.ErrorMessage = "companyID is not valid.";
                return response;
            }

            // Validate the provided company floor data
            if (companyFloor is null || companyFloor.FloorPlanName == null || companyFloor.FloorPlanImage == null || companyFloor.FloorSpaces == null)
            {
                response.ErrorMessage = "CompanyFloor cannot be null.";
                return response;
            }

            // Prepare parameters for inserting the company floor
            var companyFloorParameters = new Dictionary<string, object>
            {
                {"companyID", companyID},
                {"floorPlanName", companyFloor.FloorPlanName},
                {"floorPlanImage", companyFloor.FloorPlanImage}
            };

            // Package data for database insertion
            var floorTableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "companyFloor", companyFloorParameters }
            };

            // Insert the company floor into the database
            var floorInsertResponse = await _spaceManagerDao.InsertIntoMultipleTables(floorTableData);

            // Check for insertion errors
            if (floorInsertResponse.HasError)
            {
                return floorInsertResponse;
            }
            if (companyFloor.FloorPlanName != null)
            {

                // Retrieve the company floor ID for further operations
                Response tableResponse = await _spaceManagerDao.GetCompanyFloorIDByName(companyFloor.FloorPlanName, companyID);

                // Validate the response and proceed with space insertion
                if(tableResponse.ValuesRead != null)
                {
                    foreach (DataRow row in tableResponse.ValuesRead.Rows)
                    {
                        int floorPlanID = Convert.ToInt32(row["floorPlanID"]);
                        if (floorPlanID > 0)
                        {
                            // Prepare data for each space associated with the floor
                            var spaceList = ListSpace(companyFloor);

                            // Insert each space entry
                            foreach (var spaceDict in spaceList)
                            {
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
            
            // Return response based on overall operation success or specific error handling
            return floorInsertResponse; 
        }

        // Helper method to prepare space data from the provided CompanyFloor object
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

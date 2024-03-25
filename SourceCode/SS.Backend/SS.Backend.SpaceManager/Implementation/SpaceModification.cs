using SS.Backend.SharedNamespace;
using System.Data.SqlClient;
using System.Data;


namespace SS.Backend.SpaceManager
{
    
    public class SpaceModification : ISpaceModification
    {
        private readonly ISpaceManagerDao _spaceManagerDao;

        // SpaceManagerDao spaceManagerDao = new SpaceManagerDao();
        public SpaceModification(ISpaceManagerDao spaceManagerDao)
        {
            _spaceManagerDao = spaceManagerDao;
        }

        public async Task<Response> ModifyFloorImage(string hashedUsername, string floorPlanName, byte[] newFloorPlanImage)
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

            // Check for valid companyID and companyFloor
            if (companyID <= 0)
            {
                response.ErrorMessage = "companyID is not valid.";
                return response;
            }

            // Check if companyID is involved in any reservations
            var checkResponse = await CheckCompanyReservation(companyID);
            if (checkResponse.HasError)
            {
                // Abort operation if there are existing reservations
                return checkResponse;
            }

            // Prepare the whereClauses dictionary
            var whereClauses = new Dictionary<string, object>
            {
                {"companyID", companyID}, 
                {"floorPlanName", floorPlanName}
            };

            // Now call the GeneralModifier with the updated signature
            response = await _spaceManagerDao.GeneralModifier(whereClauses, "floorPlanImage", newFloorPlanImage, "dbo.companyFloor");
            if (response.HasError == false)
            {
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Info",
                    username = hashedUsername,
                    category = "Data Store",
                    description = "Successful floor image modification"
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
                    description = "Error modifying floor image in data store."
                };
                // await logger.SaveData(entry);
            }
            return response;
        }

        public async Task<Response> ModifyTimeLimit(string hashedUsername, string spaceID, int newTimeLimit)
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

            // Check for valid companyID and companyFloor
            if (companyID <= 0)
            {
                response.ErrorMessage = "companyID is not valid.";
                return response;
            }

            // Check if companyID is involved in any reservations
            var checkResponse = await CheckCompanyReservation(companyID);
            if (checkResponse.HasError)
            {
                // Abort operation if there are existing reservations
                return checkResponse;
            }

            // Prepare the whereClauses dictionary to include both conditions
            var whereClauses = new Dictionary<string, object>
            {
                {"companyID", companyID},
                {"spaceID", spaceID}
            };

            // Now call the GeneralModifier with the updated signature
            response = await _spaceManagerDao.GeneralModifier(whereClauses, "timeLimit", newTimeLimit, "dbo.companyFloorSpaces");
            if (response.HasError == false)
            {
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Info",
                    username = hashedUsername,
                    category = "Data Store",
                    description = "Successful time limit modifcation"
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
                    description = "Error modifying time limit in data store."
                };
                // await logger.SaveData(entry);
            }
            return response;
        }

        public async Task<Response> DeleteSpace(string hashedUsername, string spaceID)
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

            // Check for valid companyID and companyFloor
            if (companyID <= 0)
            {
                response.ErrorMessage = "companyID is not valid.";
                return response;
            }
            
            // Check if companyID is involved in any reservations
            var checkResponse = await CheckCompanyReservation(companyID);
            if (checkResponse.HasError)
            {
                // Abort operation if there are existing reservations
                return checkResponse;
            }
            var conditions = new Dictionary<string, object>
            {
                { "companyID", companyID },
                { "spaceID", spaceID }
            };

            response = await _spaceManagerDao.DeleteField(conditions, "dbo.companyFloorSpaces");
            if (response.HasError == false)
            {
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Info",
                    username = hashedUsername,
                    category = "Data Store",
                    description = "Successful space deletion"
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
                    description = "Error deleting space in data store."
                };
                // await logger.SaveData(entry);
            }
            return response;
        }

        public async Task<Response> getCompanyFloor(string hashedUsername){
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

            // Check for valid companyID and companyFloor
            if (companyID <= 0)
            {
                response.ErrorMessage = "companyID is not valid.";
                return response;
            }
            response = await _spaceManagerDao.readTableWhere("companyID", companyID, "dbo.companyFloor");

            return response;
        }


        public async Task<Response> DeleteFloor(string hashedUsername, string floorPlanName)
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

            // Check for valid companyID and companyFloor
            if (companyID <= 0)
            {
                response.ErrorMessage = "companyID is not valid.";
                return response;
            }

            // Check if companyID is involved in any reservations
            var checkResponse = await CheckCompanyReservation(companyID);
            if (checkResponse.HasError)
            {
                // Abort operation if there are existing reservations
                return checkResponse;
            }
    
            var fetchFloorPlanIdResponse = await _spaceManagerDao.GetFloorPlanIdByNameAndCompanyId(floorPlanName, companyID);
            if(fetchFloorPlanIdResponse.HasError || fetchFloorPlanIdResponse.ValuesRead == null || fetchFloorPlanIdResponse.ValuesRead.Rows.Count == 0)
            {
                return new Response { HasError = true, ErrorMessage = "Floor plan not found or error fetching floor plan ID." };
            }
            int floorPlanID = Convert.ToInt32(fetchFloorPlanIdResponse.ValuesRead.Rows[0]["floorPlanID"]);

     
            var deleteSpacesResponse = await _spaceManagerDao.DeleteField(new Dictionary<string, object> { { "floorPlanID", floorPlanID } }, "dbo.companyFloorSpaces");
            if(deleteSpacesResponse.HasError)
            {
                return deleteSpacesResponse; // Return error response from deleting spaces
            }

            var deleteFloorResponse = await _spaceManagerDao.DeleteField(new Dictionary<string, object> { { "floorPlanID", floorPlanID }, { "companyID", companyID } }, "dbo.companyFloor");
            if (deleteFloorResponse.HasError == false)
            {
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Info",
                    username = hashedUsername,
                    category = "Data Store",
                    description = "Successful floor deletion"
                };
            }
            else
            {
                LogEntry entry = new LogEntry()

                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = hashedUsername,
                    category = "Data Store",
                    description = "Error deleting floor in data store."
                };
                // await logger.SaveData(entry);
            }
            return deleteFloorResponse;
        }

        private async Task<Response> CheckCompanyReservation(int companyID)
        {
            var response = await _spaceManagerDao.readTableWhere("companyID", companyID, "dbo.Reservations");

            if (response.ValuesRead != null && response.ValuesRead.Rows.Count > 0)
            {
                // If there are rows, then there are reservations for this companyID
                return new Response { HasError = true, ErrorMessage = "Operation cannot be performed due to existing reservations." };
            }
            // No reservations found for this companyID, operation can proceed
            return new Response { HasError = false };
        }
        
    }


}
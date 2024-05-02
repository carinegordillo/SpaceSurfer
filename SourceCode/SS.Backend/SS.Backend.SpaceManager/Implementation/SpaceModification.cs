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

        public async Task<Response> ModifyFloorImage(CompanyFloor? companyFloor)
        {
            Response response = new Response();
            var companyIDResponse = await _spaceManagerDao.GetCompanyIDByHashedUsername(companyFloor.hashedUsername);
            var newFloorPlanImage = companyFloor.FloorPlanImage;
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

            if(newFloorPlanImage == null){
                response.ErrorMessage = "Must insert new floor plan to modify";
                return response;
            }

            // Prepare the whereClauses dictionary
            var whereClauses = new Dictionary<string, object>
            {
                {"companyID", companyID}, 
                {"floorPlanName", companyFloor.FloorPlanName}
            };

            // Now call the GeneralModifier with the updated signature
            response = await _spaceManagerDao.GeneralModifier(whereClauses, "floorPlanImage", companyFloor.FloorPlanImage, "dbo.companyFloor");

            return response;
        }

        public async Task<Response> ModifyTimeLimit(SpaceModifier? spaceModifier)
        {
            Response response = new Response();
            var companyIDResponse = await _spaceManagerDao.GetCompanyIDByHashedUsername(spaceModifier.hashedUsername);

            if (companyIDResponse.HasError || companyIDResponse.ValuesRead == null || companyIDResponse.ValuesRead.Rows.Count == 0)
            {
                response.ErrorMessage = "Invalid hashedUsername or companyID not found.";
                return response;
            }
            var newTimeLimit = spaceModifier.newTimeLimit;

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

            if(newTimeLimit< 0){
                response.ErrorMessage = "New Time Limit Cannot be Negative";
                return response;
            }

            // Prepare the whereClauses dictionary to include both conditions
            var whereClauses = new Dictionary<string, object>
            {
                {"companyID", companyID},
                {"spaceID", spaceModifier.spaceID}
            };

            // Now call the GeneralModifier with the updated signature
            response = await _spaceManagerDao.GeneralModifier(whereClauses, "timeLimit", spaceModifier.newTimeLimit, "dbo.companyFloorSpaces");
        
            return response;
        }

        public async Task<Response> DeleteSpace(SpaceModifier? spaceModifier)
        {
            Response response = new Response();
            var companyIDResponse = await _spaceManagerDao.GetCompanyIDByHashedUsername(spaceModifier.hashedUsername);

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
                { "spaceID", spaceModifier.spaceID }
            };

            response = await _spaceManagerDao.DeleteField(conditions, "dbo.companyFloorSpaces");
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


        public async Task<Response> DeleteFloor(CompanyFloor? companyFloor)
        {
            Response response = new Response();
            var companyIDResponse = await _spaceManagerDao.GetCompanyIDByHashedUsername(companyFloor.hashedUsername);

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
    
            var fetchFloorPlanIdResponse = await _spaceManagerDao.GetFloorPlanIdByNameAndCompanyId(companyFloor.FloorPlanName, companyID);
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
        
            return deleteFloorResponse;
        }

        private async Task<Response> CheckCompanyReservation(int companyID)
        {
            var response = await _spaceManagerDao.readTableWhere("companyID", companyID, "dbo.NewAutoIDReservations");

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
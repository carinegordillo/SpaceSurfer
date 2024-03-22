using SS.Backend.SharedNamespace;
using System.Data.SqlClient;


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

        public async Task<Response> ModifyFloorImage(int companyID, string floorPlanName, byte[] newFloorPlanImage)
        {
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
            Response response = await _spaceManagerDao.GeneralModifier(whereClauses, "floorPlanImage", newFloorPlanImage, "dbo.companyFloor");

            return response;
        }

        public async Task<Response> ModifyTimeLimit(int companyID, string spaceID, int newTimeLimit)
        {
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
            Response response = await _spaceManagerDao.GeneralModifier(whereClauses, "timeLimit", newTimeLimit, "dbo.companyFloorSpaces");

            return response;
        }

        public async Task<Response> DeleteSpace(int companyID, string spaceID)
        {
            
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

            Response response = await _spaceManagerDao.DeleteField(conditions, "dbo.companyFloorSpaces");

            return response;
        }

        public async Task<Response> getCompanyFloor(int companyID){
            Response response = await (_spaceManagerDao.readTableWhere("companyID", companyID, "dbo.companyFloor"));

            return response;
        }


        public async Task<Response> DeleteFloor(int companyID, string floorPlanName)
        {

            // Check if companyID is involved in any reservations
            var checkResponse = await CheckCompanyReservation(companyID);
            if (checkResponse.HasError)
            {
                // Abort operation if there are existing reservations
                return checkResponse;
            }
            // Step 1: Retrieve the floorPlanID
            var fetchFloorPlanIdResponse = await _spaceManagerDao.GetFloorPlanIdByNameAndCompanyId(floorPlanName, companyID);
            if(fetchFloorPlanIdResponse.HasError || fetchFloorPlanIdResponse.ValuesRead == null || fetchFloorPlanIdResponse.ValuesRead.Rows.Count == 0)
            {
                return new Response { HasError = true, ErrorMessage = "Floor plan not found or error fetching floor plan ID." };
            }
            int floorPlanID = Convert.ToInt32(fetchFloorPlanIdResponse.ValuesRead.Rows[0]["floorPlanID"]);

            // Step 2: Delete all spaces associated with the floorPlanID
            var deleteSpacesResponse = await _spaceManagerDao.DeleteField(new Dictionary<string, object> { { "floorPlanID", floorPlanID } }, "dbo.companyFloorSpaces");
            if(deleteSpacesResponse.HasError)
            {
                return deleteSpacesResponse; // Return error response from deleting spaces
            }

            // Step 3: Delete the floor entry itself
            var deleteFloorResponse = await _spaceManagerDao.DeleteField(new Dictionary<string, object> { { "floorPlanID", floorPlanID }, { "companyID", companyID } }, "dbo.companyFloor");
            return deleteFloorResponse;
        }

        private async Task<Response> CheckCompanyReservation(int companyID)
        {
            var response = await _spaceManagerDao.readTableWhere("companyID", companyID, "dbo.Reservations");
            // if (response.HasError)
            // {
            //     return new Response { HasError = true, ErrorMessage = "Error checking for reservations." };
            // }
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
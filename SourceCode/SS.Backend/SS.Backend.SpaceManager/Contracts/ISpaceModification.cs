using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.SpaceManager
{
    public interface ISpaceModification
    {
        public Task<Response> ModifyFloorImage(int companyID, string floorPlanName, byte[] newFloorPlanImage);
        public Task<Response> ModifyTimeLimit(int companyID, string spaceID, int newTimeLimit); // need to extract these from dictionary?
        public Task<Response> DeleteSpace(int companyID, string spaceID);
        public Task<Response> getCompanyFloor(int companyID);

        public Task<Response> DeleteFloor(int companyID, string floorPlanName);
        // private Task<Response> CheckCompanyReservation(int companyID);

    }
}

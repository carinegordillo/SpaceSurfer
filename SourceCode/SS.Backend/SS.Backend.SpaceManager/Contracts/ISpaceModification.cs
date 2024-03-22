using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.SpaceManager
{
    public interface ISpaceModification
    {
        public Task<Response> ModifyFloorImage(string hashedUsername, string floorPlanName, byte[] newFloorPlanImage);
        public Task<Response> ModifyTimeLimit(string hashedUsername, string spaceID, int newTimeLimit); // need to extract these from dictionary?
        public Task<Response> DeleteSpace(string hashedUsername, string spaceID);
        public Task<Response> getCompanyFloor(string hashedUsername);

        public Task<Response> DeleteFloor(string hashedUsername, string floorPlanName);
        // private Task<Response> CheckCompanyReservation(string hashedUsernam);

    }
}

using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;

namespace SS.Backend.SpaceManager
{
    public interface ISpaceModification
    {
        public Task<Response> ModifyFloorImage(CompanyFloor? companyFloor);
        public Task<Response> ModifyTimeLimit(SpaceModifier? spaceModifier); // need to extract these from dictionary?
        public Task<Response> DeleteSpace(SpaceModifier? spaceModifier);
        public Task<Response> getCompanyFloor(string hashedUsername);

        public Task<Response> DeleteFloor(CompanyFloor? companyFloor);
        // private Task<Response> CheckCompanyReservation(string hashedUsernam);

    }
}


// foreach (var entry in spaceTimeLimits)
//        {
//            var spaceID = entry.Key;
//            var newTimeLimit = entry.Value;

//            var response = await _spaceModification.ModifyTimeLimit(spaceModifier);
//            if (response.HasError)
//            {
//                return BadRequest($"Error modifying time limit for space ID {spaceID}: {response.ErrorMessage}");
//            }
//            else
//            {
//                messages.Add($"Timelimit for space ID {spaceID} modified successfully!");
//            }
//        }
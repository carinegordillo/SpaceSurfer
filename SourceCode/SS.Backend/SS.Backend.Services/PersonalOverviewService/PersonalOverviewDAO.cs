using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Services.PersonalOverviewService
{
    public class PersonalOverviewDAO : IPersonalOverviewDAO
    {
        private ISqlDAO _sqlDAO;
        public PersonalOverviewDAO(ISqlDAO sqlDAO)
        {
            _sqlDAO = sqlDAO;
        }
        public async Task<Response> GetReservationList(string hashedUsername, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            Response result = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                { "HashedUsername", hashedUsername }
            };

            if (fromDate.HasValue)
            {
                parameters.Add("FromDate", fromDate.Value);
            }

            if (toDate.HasValue)
            {
                parameters.Add("ToDate", toDate.Value);
            }

            var getCommand = null as SqlCommand;

            if (parameters.Count > 1)
            {
                getCommand = commandBuilder.BeginSelectAll()
                                            .From("reservations")
                                            .Join("companyProfile", "reservations.companyID", "companyProfile.companyID")
                                            .Join("companyFloorSpaces", "reservations.spaceID", "companyFloorSpaces.spaceID")
                                            .WhereMultiple(parameters).Build();
            }
            else
            {

                getCommand = commandBuilder.BeginSelectAll()
                                            .From("reservations")
                                            .Join("companyProfile", "reservations.companyID", "companyProfile.companyID")
                                            .Join("companyFloorSpaces", "reservations.spaceID", "companyFloorSpaces.spaceID")
                                            .Where("hashedUsername = @HashedUsername").AddParameters(parameters).Build();
            }

            result = await _sqlDAO.ReadSqlResult(getCommand);

            if (result.HasError)
            {
                result.ErrorMessage += "Error retrieving information";
                return result;
            }

            return result;
        }
    }
}

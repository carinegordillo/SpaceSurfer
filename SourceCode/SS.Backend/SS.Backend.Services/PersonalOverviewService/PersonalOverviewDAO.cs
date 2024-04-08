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

        public async Task<Response> GetReservationList(string username, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            Response result = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();

            if (fromDate == null)
            {
                fromDate = DateOnly.MinValue;
            }

            if (toDate == null)
            {
                toDate = DateOnly.MaxValue;
            }

            var getCommand = commandBuilder.BeginSelect()
                                        .SelectColumns("r.reservationID, c.companyName", "r.companyID", "c.address", "r.floorPlanID", "r.spaceID",
                                            "CAST(r.reservationStartTime AS DATE) AS reservationDate",
                                            "CAST(r.reservationStartTime AS TIME) AS startTime",
                                            "CAST(r.reservationEndTime AS TIME) AS endTime",
                                            "r.status")
                                        .From("reservations r")
                                        .Join("userHash uh", "r.userHash", "uh.hashedUsername")
                                        .Join("companyProfile c", "r.companyID", "c.companyID")
                                        .Where($"userHash = '{username}' AND CAST(r.reservationStartTime AS DATE) >= '{fromDate}' AND CAST(r.reservationStartTime AS DATE) <= '{toDate}'")
                                        .OrderBy("reservationDate").Build();

            result = await _sqlDAO.ReadSqlResult(getCommand);

            if (result.HasError)
            {
                result.ErrorMessage += "Error retrieving information";
                return result;
            }

            return result;
        }

        public async Task<Response> DeleteReservation(string username, int reservationID)
        {
            Response response = new Response();

            var commandBuilder = new CustomSqlCommandBuilder();

            var getCommand = commandBuilder.BeginDelete("dbo.reservations")
                                        .Where($"reservationID = {reservationID} AND userHash = '{username}'")
                                        .Build();

            response = await _sqlDAO.ReadSqlResult(getCommand);

            if (response.HasError)
            {
                response.ErrorMessage += "Error deleting the information";
                return response;
            }

            return response;
        }
    }
}

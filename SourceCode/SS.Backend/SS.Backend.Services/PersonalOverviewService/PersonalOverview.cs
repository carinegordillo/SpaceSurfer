using SS.Backend.SharedNamespace;
using System.Data;

namespace SS.Backend.Services.PersonalOverviewService
{
    public class PersonalOverview : IPersonalOverview
    {

        private readonly IPersonalOverviewDAO _personalOverviewDAO;

        public PersonalOverview(IPersonalOverviewDAO personalOverviewDAO)
        {
            _personalOverviewDAO = personalOverviewDAO;
        }

        public async Task<IEnumerable<ReservationInformation>> GetUserReservationsAsync(string userHash, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            List<ReservationInformation> reservationInfo = new List<ReservationInformation>();
            Response result = new Response();
            try
            {
                result = await _personalOverviewDAO.GetReservationList(userHash, fromDate, toDate);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage += $"Encountered an error retrieving the reservation information:{ex.Message}";
                return reservationInfo;
            }


            if (!result.HasError && result.ValuesRead != null)
            {
                foreach (DataRow row in result.ValuesRead.Rows)
                {
                    var reservation = new ReservationInformation
                    {
                        CompanyName = Convert.ToString(row["companyName"]).Trim(),
                        SpaceID = Convert.ToInt32(row["spaceID"]),
                        ReservationDate = new DateOnly(((DateTime)row["reservationDate"]).Year, ((DateTime)row["reservationDate"]).Month, ((DateTime)row["reservationDate"]).Day),
                        ReservationStartTime = Convert.ToInt32(row["reservationStartTime"]),
                        ReservationEndTime = Convert.ToInt32(row["reservationEndTime"]),
                        status = Convert.ToString(row["status"]).Trim(),
                        userHash = Convert.ToString(row["userHash"]).Trim()
                    };
                }
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage += $"No data found.";
            }

            return reservationInfo;
        }
    }
}

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

        public async Task<IEnumerable<ReservationInformation>> GetUserReservationsAsync(string username, DateOnly? fromDate = null, DateOnly? toDate = null)
        {
            List<ReservationInformation> reservationList = new List<ReservationInformation>();
            Response result = new Response();
            try
            {
                result = await _personalOverviewDAO.GetReservationList(username, fromDate, toDate);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage += $"Encountered an error retrieving the reservation information:{ex.Message}";
                return reservationList;
            }


            if (!result.HasError && result.ValuesRead != null)
            {
                foreach (DataRow row in result.ValuesRead.Rows)
                {
                    var reservationDate = ((DateTime)row["reservationDate"]).Date;

                    var reservation = new ReservationInformation
                    {
                        ReservationID = Convert.ToInt32(row["reservationID"]),
                        CompanyName = row["companyName"].ToString(),
                        CompanyID = Convert.ToInt32(row["companyID"]),
                        Address = row["address"].ToString(),
                        FloorPlanID = Convert.ToInt32(row["floorPlanID"]),
                        SpaceID = row["spaceID"].ToString(),
                        ReservationDate = DateOnly.FromDateTime(reservationDate),
                        ReservationStartTime = ((TimeSpan)row["startTime"]),
                        ReservationEndTime = ((TimeSpan)row["endTime"]),
                        Status = row["status"].ToString()
                    };

                    reservationList.Add(reservation);
                }
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage += $"No data found.";
            }

            return reservationList;
        }

        public async Task<Response> DeleteUserReservationsAsync(string username, int reservationID)
        {

            Response response = new Response();
            try
            {
                response = await _personalOverviewDAO.DeleteReservation(username, reservationID);
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage += $"Encountered an error deleting the reservation:{ex.Message}";
                return response;
            }

            return response;
        }
    }
}

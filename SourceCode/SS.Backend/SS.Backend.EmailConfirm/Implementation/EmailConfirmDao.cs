using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;

namespace SS.Backend.EmailConfirm
{
    public class EmailConfirmDAO : IEmailConfirmDAO
    {
        private ISqlDAO _sqlDao;

        public EmailConfirmDAO(ISqlDAO sqlDao)
        {
            _sqlDao = sqlDao;
        }

        public async Task<Response> GetReservationInfo(int ReservationID)
        {
            Response response = new Response();
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                {"ReservationID", ReservationID}
            };

            var cmd = builder.BeginSelect()
                            .SelectColumns("r.*", "cp.address AS CompanyAddress")
                            .From("Reservations AS r")
                            .Join("companyProfile AS cp", "r.companyID", "cp.companyID")
                            .WhereMuliple("reservationID = @ReservationID")
                            .AddParameters(parameters)
                            .Build();


            response = await _sqlDao.ReadSqlResult(cmd);

            if (!response.HasError)
            {
                response.ErrorMessage += " -- GetReservationInfo Command: Successful";
            }
            else
            {
                response.ErrorMessage += $" -- GetReservationInfo Command: {cmd.CommandText} Failed";
            }
            return response;

        }

        public async Task<Response> GetConfirmInfo(int ReservationID)
        {
            Response response = new Response();
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                {"ReservationID", ReservationID}
            };

            var cmd = builder.BeginSelect()
                            .SelectColumns("confirmStatus")
                            .From("ConfirmReservations")
                            .Where("reservationID = @ReservationID")
                            .AddParameters(parameters)
                            .Build();

            response = await _sqlDao.ReadSqlResult(cmd);

            if (!response.HasError)
            {
                response.ErrorMessage += " -- GetConfirmInfo Command: Successful";
            }
            else
            {
                response.ErrorMessage += $" -- GetConfirmInfo Command: {cmd.CommandText} Failed";
            }
            return response;
        }

        public async Task<Response> InsertConfirmStatus(int ReservaitonID)
        {
            Response response = new Response();
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                {"ReservationID", ReservationID}
            };

            var cmd = builder.BeginUpdate("ConfirmReservations")
                            .Set(new Dictionary<string, object>
                                {{"ConfirmStatus", 'Yes'}})
                            .Where("reservationID = @ReservationID")
                            .AddParameters(parameters)
                            .Build();

            response = await _sqlDao.SqlRowsAffected(cmd);

            if (!response.HasError)
            {
                response.ErrorMessage += " -- InsertConfirmStatus Command: Successful";
            }
            else
            {
                response.ErrorMessage += $" -- InsertConfirmStatus Command: {cmd.CommandText} Failed";
            }
            return response;

        }
    }
}
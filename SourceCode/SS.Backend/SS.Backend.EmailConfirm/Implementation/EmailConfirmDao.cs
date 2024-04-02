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
                {"reservationID", ReservationID}
            };

            var cmd = builder.BeginSelect()
                            .SelectColumns("r.*", "cp.address AS CompanyAddress", "cp.companyName as CompanyName")
                            .From("Reservations AS r")
                            .Join("companyProfile AS cp", "r.companyID", "cp.companyID")
                            .Where("r.ReservationID = reservationID")
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
                {"reservationID", ReservationID}
            };

            var cmd = builder.BeginSelectAll()
                            .From("ConfirmReservations")
                            .Where("reservationID = reservationID")
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

        public async Task<Response> InsertConfirmationInfo(int ReservationID, string otp, byte[]? file)
        {
            Response response = new Response();
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                {"reservationID", ReservationID},
                {"reservationOtp", otp},
                {"confirmStatus", "no"},
                {"icsFile", file}
            };

            var cmd = builder.BeginInsert("ConfirmReservations")
                            .Columns(parameters.Keys)
                            .Values(parameters.Keys)
                            .AddParameters(parameters)
                            .Build();

            response = await _sqlDao.SqlRowsAffected(cmd);

            if (!response.HasError)
            {
                response.ErrorMessage += " -- InsertConfirmationInfo Command: Successful";
            }
            else
            {
                response.ErrorMessage += $" -- InsertConfirmationInfo Command: {cmd.CommandText} Failed";
            }
            return response;
        }

        public async Task<Response> UpdateConfirmStatus(int ReservationID)
        {
            Response response = new Response();
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                {"ReservationID", ReservationID},
                {"ConfirmStatus", "yes"} 
            };

            var cmd = builder.BeginUpdate("ConfirmReservations")
                            .Set(new Dictionary<string, object>
                                {{"confirmStatus", "@ConfirmStatus"}})
                            .Where("reservationID = @ReservationID")
                            .AddParameters(parameters)
                            .Build();

            response = await _sqlDao.SqlRowsAffected(cmd);

            if (!response.HasError)
            {
                response.ErrorMessage += " -- UpdateConfirmStatus Command: Successful";
            }
            else
            {
                response.ErrorMessage += $" -- UpdateConfirmStatus Command: {cmd.CommandText} Failed";
            }
            return response;
        }

        public async Task<Response> UpdateOtp(int ReservationID, string otp)
        {
            Response response = new Response();
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                {"reservationID", ReservationID},
                {"reservationOtp", otp}
            };

            var cmd = builder.BeginUpdate("ConfirmReservations")
                            .Set(new Dictionary<string, object>
                                {{"reservationOTP", "reservationOtp"}})
                            .Where("reservationID = reservationID")
                            .AddParameters(parameters)
                            .Build();

            response = await _sqlDao.SqlRowsAffected(cmd);

            if (!response.HasError)
            {
                response.ErrorMessage = " -- UpdateOtp Command: Successful";
            }
            else
            {
                response.ErrorMessage = $" -- UpdateOtp Command: {cmd.CommandText} Failed";
            }
            return response;
        }

    }
}
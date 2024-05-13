using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using SS.Backend.ReservationManagement;
using System.Data;

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
                            .SelectColumns("Reservations.*", "companyProfile.address AS CompanyAddress", "companyProfile.companyName as CompanyName")
                            .From("Reservations")
                            .Join("companyProfile", "Reservations.companyID", "companyProfile.companyID")
                            .Where($"Reservations.reservationID = {ReservationID}")
                            .AddParameters(parameters)
                            .Build();


            response = await _sqlDao.ReadSqlResult(cmd);

            Console.WriteLine(response.ValuesRead.Rows[0]["companyName"].ToString());

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
                            .Where($"reservationID = {ReservationID}")
                            .AddParameters(parameters)
                            .Build();

            response = await _sqlDao.ReadSqlResult(cmd);

            Console.WriteLine(response.ValuesRead.Rows[0]["reservationID"].ToString());

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
                                {{"reservationOTP", "@reservationOtp"}})
                            .Where("reservationID = @reservationID")
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

        public async Task<Response> GetUsername(string userHash)
        {
            Response tablesresponse = new Response();
            var builder = new CustomSqlCommandBuilder();
            var command = builder.BeginSelect()
                                .SelectColumns("username")
                                .From("dbo.userHash")
                                .Where("hashedUsername = @HashedUsername")
                                .AddParameters(new Dictionary<string, object>
                                {
                                    { "HashedUsername", userHash}
                                })
                                .Build();

            var username = await _sqlDao.ReadSqlResult(command);

            return username;
        }

        public async Task<(UserReservationsModel, Response)> GetUserReservationByID (int reservationID)
        {
            Response response = new Response();
            var builder = new CustomSqlCommandBuilder();
            UserReservationsModel reservation = new UserReservationsModel();

            response = await GetReservationInfo(reservationID);

            try
            {
                if (response.ValuesRead != null && response.ValuesRead.Rows.Count > 0)
                {
                    DataRow row = response.ValuesRead.Rows[0];
#pragma warning disable CS8601 // Possible null reference assignment.
                    reservation = new UserReservationsModel
                    {
                        ReservationID = Convert.ToInt32(row["reservationID"]),
                        CompanyID = Convert.ToInt32(row["companyID"]),
                        FloorPlanID = Convert.ToInt32(row["floorPlanID"]),
                        SpaceID = row["spaceID"].ToString(),
                        ReservationStartTime = Convert.ToDateTime(row["reservationStartTime"]),
                        ReservationEndTime = Convert.ToDateTime(row["reservationEndTime"]),
                        Status = Enum.Parse<ReservationStatus>(row["status"].ToString(), true),
                        UserHash = row["userHash"].ToString()
                    };
#pragma warning restore CS8601 // Possible null reference assignment.

                    response.ErrorMessage = $"GetUserReservationByID Command: Successful: {reservation}";
                }
                else
                {
                    response.HasError = true;
                    response.ErrorMessage = $"No reservation found with the given ID, {reservationID}.";
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = "GetUserReservationByID Command Failed: " + ex.Message;
            }
            return (reservation, response);
        }

        public async Task<Response> GetAllTableInfo (string tableName)
        {

            Response response = new Response();
            var builder = new CustomSqlCommandBuilder();

            var cmd = builder.BeginSelectAll()
                            .From($"{tableName}")
                            .Build();

            response = await _sqlDao.ReadSqlResult(cmd);

            if (!response.HasError)
            {
                response.ErrorMessage += " -- GetAllTableInfo Command: Successful";
            }
            else
            {
                response.ErrorMessage += $" -- GetAllTableInfo Command: {cmd.CommandText} Failed";
            }
            Console.WriteLine(response.ErrorMessage);
            return response;
        }

        public async Task<Response> CancelConfirmation(int reservationID)
        {
            Response response = new Response();
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                {"ReservationID", reservationID},
                {"ConfirmStatus", "no"} 
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
                response.ErrorMessage += " -- CancelConfirmation Command: Successful";
            }
            else
            {
                response.ErrorMessage += $" -- CancelConfirmation Command: {cmd.CommandText} Failed";
            }
            Console.WriteLine(response.ErrorMessage);
            return response;
        }
        public async Task<Response> DeleteReservation(string tableName, int reservationID)
        {
            Response response = new Response();
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                {"ReservationID", reservationID}
            };

            var cmd = builder.BeginDelete($"{tableName}")
                            .Where($"reservationID = {reservationID}")
                            .AddParameters(parameters)
                            .Build();

            response = await _sqlDao.SqlRowsAffected(cmd);

            if (!response.HasError)
            {
                response.ErrorMessage += " -- DeleteReservation Command: Successful";
            }
            else
            {
                response.ErrorMessage += $" -- DeleteReservation Command: {cmd.CommandText} Failed";
            }
            Console.WriteLine(response.ErrorMessage);
            return response;
        }

    }
}
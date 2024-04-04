using SS.Backend.DataAccess;
using SS.Backend.Services.EmailService;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using System;

namespace SS.Backend.Waitlist
{
    public class WaitlistService
    {
        private readonly SqlDAO _sqldao;

        public WaitlistService(SqlDAO sqldao)
        {
            _sqldao = sqldao;
        }

        public async Task SendConfirmationEmail(WaitlistEntry info)
        {
            var builder = new CustomSqlCommandBuilder();
            Response result = new Response();

            try
            {
                // Get user email from DB
                var getEmail = builder
                    .BeginSelectAll()
                    .From("userHash")
                    .Where($"hashedUsername = '{info.userHash}'")
                    .Build();
                result = await _sqldao.ReadSqlResult(getEmail);
                string? email = result.ValuesRead?.Rows[0]?["username"].ToString();

                // Get first name from DB
                var getName = builder
                    .BeginSelectAll()
                    .From("userProfile")
                    .Where($"hashedUsername = '{info.userHash}'")
                    .Build();
                result = await _sqldao.ReadSqlResult(getName);
                string? name = result.ValuesRead?.Rows[0]?["firstName"].ToString();

                string targetEmail = email;
                string subject = $@"Waitlist Confirmation at {info.companyName}";
                string msg = $@"
                    Dear {name},

                    Thank you for joining the waitlist for Space {info.spaceID} at {info.companyName}.

                    Your reservation details:
                    - Space: {info.spaceID}
                    - Company: {info.companyName}
                    - Date: {info.startTime} to {info.endTime}
                    - Current Waitlist Position: {info.position}

                    Please note:
                    - If you become first on the waitlist, you will receive an email to confirm your reservation.
                    - You have 3 hours to confirm your reservation once you reach the top of the waitlist.
                    - If you no longer wish to be on the waitlist, you can cancel your reservation by visiting the Waitlist tab.

                    If you have any questions or need assistance, please don't hesitate to contact us at spacesurfers5@gmail.com.

                    Thank you for choosing SpaceSurfer.

                    Best regards,
                    SpaceSurfer Team";

                try
                {
                    await MailSender.SendEmail(targetEmail, subject, msg);
                    result.HasError = false;
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage = ex.Message;
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        public async Task SendNotificationEmail(WaitlistEntry info)
        {
            CustomSqlCommandBuilder builder = new CustomSqlCommandBuilder();
            Response result = new Response();

            try
            {
                // Get user email from DB
                var getEmail = builder
                    .BeginSelectAll()
                    .From("userHash")
                    .Where($"hashedUsername = '{info.userHash}'")
                    .Build();
                result = await _sqldao.ReadSqlResult(getEmail);
                string? email = result.ValuesRead?.Rows[0]?["username"].ToString();

                // Get first name from DB
                var getName = builder
                    .BeginSelectAll()
                    .From("userProfile")
                    .Where($"hashedUsername = '{info.userHash}'")
                    .Build();
                result = await _sqldao.ReadSqlResult(getName);
                string? name = result.ValuesRead?.Rows[0]?["firstName"].ToString();

                string targetEmail = email;
                string subject = $@"Top of Waitlist at {info.companyName}";
                string msg = $@"
                    Dear {name},

                    The reservation for Space {info.spaceID} at {info.companyName} has opened up!

                    Your reservation details:
                    - Space: {info.spaceID}
                    - Company: {info.companyName}
                    - Date: {info.startTime} to {info.endTime}

                    Please note:
                    - You have 3 hours to confirm your reservation on the Waitlist tab otherwise your spot will be given to the next user if one exists.
                    - If you would like to cancel this reservation, please visit the Waitlist tab.

                    If you have any questions or need assistance, please don't hesitate to contact us at spacesurfers5@gmail.com.

                    Thank you for choosing SpaceSurfer.

                    Best regards,
                    SpaceSurfer Team";

                try
                {
                    await MailSender.SendEmail(targetEmail, subject, msg);
                    result.HasError = false;
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage = ex.Message;
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        // method to insert approved user into waitlist as #0 (called when reservation is made)
        public async Task InsertApprovedUser(string userHash, int resId)
        {
            var builder = new CustomSqlCommandBuilder();
            Response result = new Response();

            try
            {
                var parameters = new Dictionary<string, object>
                    {
                        { "Username", userHash },
                        { "ReservationID", resId },
                        { "Position", 0 }
                    };
                var insertCmd = builder
                    .BeginInsert("Waitlist")
                    .Columns(parameters.Keys)
                    .Values(parameters.Keys)
                    .AddParameters(parameters)
                    .Build();
                result = await _sqldao.SqlRowsAffected(insertCmd);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        // method to check position on waitlist (called before adding waitlisted user to waitlist)
        public async Task<int> GetWaitlistSize(int resId)
        {
            CustomSqlCommandBuilder builder = new CustomSqlCommandBuilder();
            Response result = new Response();

            try
            {
                // Get count of users on waitlist for reservation
                var countCmd = builder.CountWaitlistUsersForReservation(resId).Build();
                result = await _sqldao.ReadSqlResult(countCmd);
                int count = Convert.ToInt32(result.ValuesRead?.Rows[0]?["Count"]);

                return count;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        // method to check user's position on a waitlist
        public async Task<int> GetWaitlistPosition(string userHash, int resId)
        {
            CustomSqlCommandBuilder builder = new CustomSqlCommandBuilder();
            Response result = new Response();

            try
            {
                var getPos = builder
                    .BeginSelectAll()
                    .From("Waitlist")
                    .Where("ReservationID = @id AND Username = @user")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "id", resId },
                        { "username", userHash}
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getPos);
                int pos = Convert.ToInt32(result.ValuesRead?.Rows[0]?["Position"].ToString());
                return pos;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        // method to insert waitlisted user into waitlist as #1 and up (called when joining waitlist)
        public async Task InsertWaitlistedUser(string userHash, int resId)
        {
            var builder = new CustomSqlCommandBuilder();
            Response result = new Response();

            try
            {
                int pos = await GetWaitlistSize(resId);
                var parameters = new Dictionary<string, object>
                    {
                        { "Username", userHash },
                        { "ReservationID", resId },
                        { "Position", pos }
                    };
                var insertCmd = builder
                    .BeginInsert("Waitlist")
                    .Columns(parameters.Keys)
                    .Values(parameters.Keys)
                    .AddParameters(parameters)
                    .Build();
                result = await _sqldao.SqlRowsAffected(insertCmd);

                // spaceID
                var getSpaceId = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getSpaceId);
                var spaceId = result.ValuesRead?.Rows[0]?["spaceID"].ToString();

                // companyName
                var getCompId = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getCompId);
                int compId = Convert.ToInt32(result.ValuesRead?.Rows[0]?["companyID"].ToString());
                var getCompName = builder
                    .BeginSelectAll()
                    .From("companyProfile")
                    .Where("companyID = @id")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "id", compId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getCompName);
                string companyName = result.ValuesRead?.Rows[0]?["companyName"].ToString();

                // startTime
                var getStart = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getStart);
                DateTime startTime = Convert.ToDateTime(result.ValuesRead?.Rows[0]?["reservationStartTime"]);

                // endTime
                var getEnd = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getEnd);
                DateTime endTime = Convert.ToDateTime(result.ValuesRead?.Rows[0]?["reservationEndTime"]);

                // Populate WaitlistEntry with info needed for confirmation email
                WaitlistEntry entry = new WaitlistEntry
                {
                    userHash = userHash,
                    spaceID = spaceId,
                    companyName = companyName,
                    startTime = startTime,
                    endTime = endTime,
                    position = pos
                };

                await SendConfirmationEmail(entry);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }

        }

        // method to update user positions on waitlist (called when user who had reservation cancels)
        public async Task UpdateWaitlist_ApprovedUserLeft(int resId)
        {
            var builder = new CustomSqlCommandBuilder();
            Response result = new Response();

            try
            {
                // Delete the user who cancelled
                var deleteUserCmd = builder
                    .BeginDelete("Waitlist")
                    .Where("ReservationID = @id AND Position = @pos")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "id", resId },
                        { "pos", 0 }
                    })
                    .Build();
                result = await _sqldao.SqlRowsAffected(deleteUserCmd);

                // Retrieve all users on the waitlist for the reservation
                var getUsersCmd = builder
                    .BeginSelectAll()
                    .From("Waitlist")
                    .Where("ReservationID = @id")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "id", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getUsersCmd);

                // Update positions for each user
                if (result.ValuesRead != null)
                {
                    foreach (DataRow user in result.ValuesRead.Rows)
                    {
                        int oldPosition = Convert.ToInt32(user["Position"]);
                        int newPosition = oldPosition - 1;

                        var updatePositionCmd = builder
                            .BeginUpdate("Waitlist")
                            .Set(new Dictionary<string, object>
                            {
                                { "Position", newPosition }
                            })
                            .Where("ReservationID = @id AND Username = @username")
                            .AddParameters(new Dictionary<string, object>
                            {
                                { "id", resId },
                                { "username", user["Username"] }
                            })
                            .Build();
                        result = await _sqldao.SqlRowsAffected(updatePositionCmd);
                    }
                }

                // userHash
                var getHash = builder
                    .BeginSelectAll()
                    .From("Waitlist")
                    .Where("Position = 0")
                    .Build();
                result = await _sqldao.ReadSqlResult(getHash);
                var userHash = result.ValuesRead?.Rows[0]?["spaceID"].ToString();

                // spaceID
                var getSpaceId = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                    { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getSpaceId);
                var spaceId = result.ValuesRead?.Rows[0]?["spaceID"].ToString();

                // companyName
                var getCompId = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                    { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getCompId);
                int compId = Convert.ToInt32(result.ValuesRead?.Rows[0]?["companyID"].ToString());
                var getCompName = builder
                    .BeginSelectAll()
                    .From("companyProfile")
                    .Where("companyID = @id")
                    .AddParameters(new Dictionary<string, object>
                    {
                    { "id", compId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getCompName);
                string companyName = result.ValuesRead?.Rows[0]?["companyName"].ToString();

                // startTime
                var getStart = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                    { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getStart);
                DateTime startTime = Convert.ToDateTime(result.ValuesRead?.Rows[0]?["reservationStartTime"]);

                // endTime
                var getEnd = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                    { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getEnd);
                DateTime endTime = Convert.ToDateTime(result.ValuesRead?.Rows[0]?["reservationEndTime"]);

                // Populate WaitlistEntry with info needed for notification email
                WaitlistEntry entry = new WaitlistEntry
                {
                    userHash = userHash,
                    spaceID = spaceId,
                    companyName = companyName,
                    startTime = startTime,
                    endTime = endTime,
                    position = 0
                };

                await SendNotificationEmail(entry);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }
        }


        // user unapproved for reservation leaves waitlist
        public async Task UpdateWaitlist_WaitlistedUserLeft(int resId, int leavePos)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            try
            {
                // Retrieve all users on the waitlist for the reservation
                var getUsersCmd = builder
                    .BeginSelectAll()
                    .From("Waitlist")
                    .Where("ReservationID = @id")
                    .AddParameters(new Dictionary<string, object>
                    {
                    { "id", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getUsersCmd);

                // Update positions for users below the leaving user
                if (result.ValuesRead != null)
                {
                    foreach (DataRow user in result.ValuesRead.Rows)
                    {
                        int oldPosition = Convert.ToInt32(user["Position"]);

                        // Check if the user's position needs to be updated
                        if (oldPosition > leavePos)
                        {
                            int newPosition = oldPosition - 1;

                            var updatePositionCmd = builder
                                .BeginUpdate("Waitlist")
                                .Set(new Dictionary<string, object>
                                {
                                { "Position", newPosition }
                                })
                                .Where("ReservationID = @id AND Username = @username")
                                .AddParameters(new Dictionary<string, object>
                                {
                                { "id", resId },
                                { "username", user["Username"] }
                                })
                                .Build();
                            result = await _sqldao.SqlRowsAffected(updatePositionCmd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        public async Task<List<WaitlistEntry>> GetUserWaitlists(string userHash)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            var waitlists = new List<WaitlistEntry>();

            // Get waitlisted reservations for the specified user
            var getReservations = builder
                .BeginSelectAll()
                .From("Waitlist")
                .Where("Username = @user")
                .AddParameters(new Dictionary<string, object>
                {
                    { "user", userHash }
                })
                .Build();

            result = await _sqldao.ReadSqlResult(getReservations);

            // Iterate over each row in the result and create a WaitlistEntry object for each waitlisted reservation
            foreach (DataRow row in result.ValuesRead.Rows)
            {
                int reservationId = Convert.ToInt32(row["ReservationID"]);

                // Get reservation details using reservationId
                var getResDetails = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("reservationID = @id")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "id", reservationId }
                    })
                    .Build();

                var resDetailsResult = await _sqldao.ReadSqlResult(getResDetails);

                // Populate WaitlistEntry with reservation details
                var entry = new WaitlistEntry
                {
                    userHash = userHash,
                    spaceID = resDetailsResult.ValuesRead?.Rows[0]?["spaceID"].ToString(),
                    companyName = resDetailsResult.ValuesRead?.Rows[0]?["companyName"].ToString(),
                    startTime = Convert.ToDateTime(resDetailsResult.ValuesRead?.Rows[0]?["reservationStartTime"]),
                    endTime = Convert.ToDateTime(resDetailsResult.ValuesRead?.Rows[0]?["reservationEndTime"]),
                    position = Convert.ToInt32(row["Position"])
                };

                waitlists.Add(entry);
            }

            return waitlists;
        }

        public async Task<WaitlistEntry> GetReservationDetails(string userHash, int reservationId)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            try
            {
                // Get reservation details using reservationId
                var getResDetails = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("reservationID = @id")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "id", reservationId }
                    })
                    .Build();

                var resDetailsResult = await _sqldao.ReadSqlResult(getResDetails);

                if (resDetailsResult.ValuesRead.Rows.Count == 0)
                {
                    return null;
                }

                // Get position on waitlist for user
                var getPosition = builder
                    .BeginSelectAll()
                    .From("Waitlist")
                    .Where("Username = @user")
                    .AddParameters(new Dictionary<string, object>
                    {
                    { "user", userHash }
                    })
                    .Build();

                var posResult = await _sqldao.ReadSqlResult(getPosition);
                int pos = Convert.ToInt32(result.ValuesRead?.Rows[0]?["Position"].ToString());

                // Populate WaitlistEntry with reservation details
                var entry = new WaitlistEntry
                {
                    userHash = userHash,
                    spaceID = resDetailsResult.ValuesRead.Rows[0]["spaceID"].ToString(),
                    companyName = resDetailsResult.ValuesRead.Rows[0]["companyName"].ToString(),
                    startTime = Convert.ToDateTime(resDetailsResult.ValuesRead.Rows[0]["reservationStartTime"]),
                    endTime = Convert.ToDateTime(resDetailsResult.ValuesRead.Rows[0]["reservationEndTime"]),
                    position = pos
                };

                return entry;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> GetReservationID(int compID, int floorID, string spaceID, DateTime sTime, DateTime eTime)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            var getResId = builder
                    .BeginSelectAll()
                    .From("reservations")
                    .Where("companyID = @cid AND floorID = @fid AND spaceID = @sid AND reservationStartTime = @s AND reservationEndTime = @e")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "cid", compID },
                        { "fid", floorID },
                        { "sid", spaceID },
                        { "s", sTime },
                        { "e", eTime }
                    })
                    .Build();

            result = await _sqldao.ReadSqlResult(getResId);
            int rid = Convert.ToInt32(result.ValuesRead?.Rows[0]?["reservationID"].ToString());
            return rid;
        }
    }
}

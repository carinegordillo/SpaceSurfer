using SS.Backend.DataAccess;
using SS.Backend.Services.EmailService;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;
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

        /// <summary>
        /// This method sends a confirmation email to the user after they have joined the waitlist
        /// </summary>
        /// <param name="info">Contains information pertaining to the waitlisted reservation</param>
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

                string? start = info.startTime.ToString("h:mm tt");
                string? end = info.endTime.ToString("h:mm tt");

                string? targetEmail = email;
                string? subject = $@"Waitlist Confirmation at {info.companyName}";
                string? msg = $@"
                    Dear {name},

                    Thank you for joining the waitlist for Space {info.spaceID} at {info.companyName}.

                    Your reservation details:
                    - Space: {info.spaceID}
                    - Company: {info.companyName.Trim()}
                    - Date: {start} to {end}
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
                    // Send email
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

        /// <summary>
        /// This method sends an email to the next user on the waitlist after the spot opens up
        /// </summary>
        /// <param name="info">Contains information pertaining to the waitlist and reservation</param>
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
                Console.WriteLine("Sending notification email to: " + email);

                // Get first name from DB
                var getName = builder
                    .BeginSelectAll()
                    .From("userProfile")
                    .Where($"hashedUsername = '{info.userHash}'")
                    .Build();
                result = await _sqldao.ReadSqlResult(getName);
                string? name = result.ValuesRead?.Rows[0]?["firstName"].ToString();

                string? start = info.startTime.ToString("h:mm tt");
                string? end = info.endTime.ToString("h:mm tt");

                string? targetEmail = email;
                Console.WriteLine("Target email for notification email: " + targetEmail);
                string? subject = $@"Top of Waitlist at {info.companyName}";
                string? msg = $@"
                    Dear {name},

                    The reservation for Space {info.spaceID} at {info.companyName} has opened up!

                    Your reservation details:
                    - Space: {info.spaceID}
                    - Company: {info.companyName.Trim()}
                    - Date: {start} to {end}

                    Please note:
                    - You have 3 hours to confirm your reservation on the Waitlist tab otherwise your spot will be given to the next user if one exists.
                    - If you would like to cancel this reservation, please visit the Waitlist tab.

                    If you have any questions or need assistance, please don't hesitate to contact us at spacesurfers5@gmail.com.

                    Thank you for choosing SpaceSurfer.

                    Best regards,
                    SpaceSurfer Team";

                try
                {
                    // Send email
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
            }

        }

        /// <summary>
        /// This method is used when a reservation is made successfully. It adds the user to the waitlist table as position 0 for that reservation id
        /// </summary>
        /// <param name="userHash">hashed username</param>
        /// <param name="resId">reservation id</param>
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

        /// <summary>
        /// This method gets the size of the waitlist for that reservation id
        /// </summary>
        /// <param name="resId">reservation id</param>
        /// <returns>The size of the waitlist for that reservation id</returns>
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

        /// <summary>
        /// This method gets the position of a user on a waitlist for a specific reservation id
        /// </summary>
        /// <param name="userHash">hashed username</param>
        /// <param name="resId">reservation id</param>
        /// <returns>Returns the position</returns>
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
                        { "user", userHash}
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getPos);
                int pos = Convert.ToInt32(result.ValuesRead?.Rows[0]?["Position"]);
                return pos;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// This method checks to see if a certain user is on the waitlist for a reservation id
        /// </summary>
        /// <param name="user">hashed username</param>
        /// <param name="resId">reservation id</param>
        /// <returns>True if they are, False if they are not on the waitlist for that reservation id</returns>
        public async Task<bool> IsUserOnWaitlist(string user, int resId)
        {
            var builder = new CustomSqlCommandBuilder();
            Response result = new Response();

            var cmd = builder.onWaitlist(user, resId).Build();
            result = await _sqldao.ReadSqlResult(cmd);

            if (result.ValuesRead != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method inserts a user who isn't approved for the rservation into the waitlist
        /// </summary>
        /// <param name="resTable">Reservation table name</param>
        /// <param name="userHash">hashed username</param>
        /// <param name="resId">reservation id</param>
        public async Task InsertWaitlistedUser(string resTable, string userHash, int resId)
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
                var getSpaceId = builder.getSid(resTable, resId).Build();
                result = await _sqldao.ReadSqlResult(getSpaceId);
                var spaceId = result.ValuesRead?.Rows[0]?["spaceID"].ToString();

                // companyName
                var getCompId = builder.GetCompId(resTable, resId).Build();
                result = await _sqldao.ReadSqlResult(getCompId);
                int compId = Convert.ToInt32(result.ValuesRead?.Rows[0]?["companyID"].ToString());
                var getCompName = builder.GetCompName(compId).Build();
                result = await _sqldao.ReadSqlResult(getCompName);
                string? companyName = result.ValuesRead?.Rows[0]?["companyName"].ToString();
                // startTime
                var getStart = builder
                    .BeginSelectAll()
                    .From(resTable)
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
                    .From(resTable)
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getEnd);
                DateTime endTime = Convert.ToDateTime(result.ValuesRead?.Rows[0]?["reservationEndTime"]);

                // Populate WaitlistEntry with info needed for confirmation email
                WaitlistEntry waitlistentry = new WaitlistEntry
                {
                    userHash = userHash,
                    spaceID = spaceId,
                    companyName = companyName,
                    startTime = startTime,
                    endTime = endTime,
                    position = pos
                };
                await SendConfirmationEmail(waitlistentry);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }

        }

        /// <summary>
        /// This method removes the approved user from their waitlist and updates the positions for the rest of the users
        /// </summary>
        /// <param name="resTable">Reservation table name</param>
        /// <param name="resId">reservation id</param>
        public async Task UpdateWaitlist_ApprovedUserLeft(string resTable, int resId)
        {
            Response result = new Response();
            var builder = new CustomSqlCommandBuilder();

            try
            {
                // userHash
                var getHash = builder
                    .BeginSelectAll()
                    .From("Waitlist")
                    .Where("Position = 0")
                    .Build();
                result = await _sqldao.ReadSqlResult(getHash);
                var user = result.ValuesRead?.Rows[0]?["Username"].ToString();

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
                var getUsersCmd = builder.GetAllWaitlist(resId).Build();
                result = await _sqldao.ReadSqlResult(getUsersCmd);

                // Update positions for each user
                if (result.ValuesRead != null)
                {
                    foreach (DataRow row in result.ValuesRead.Rows)
                    {
                        int oldPosition = Convert.ToInt32(row["Position"]);
                        if (oldPosition > 0)
                        {
                            int newPosition = oldPosition - 1;
                            var updateCmd = builder.UpdatePosition(resId, user, newPosition).Build();
                            await _sqldao.SqlRowsAffected(updateCmd);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ValuesRead is null");
                }

                // spaceID
                var getSpaceId = builder
                    .BeginSelectAll()
                    .From(resTable)
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                    { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getSpaceId);
                var spaceId = result.ValuesRead?.Rows[0]?["spaceID"].ToString();

                // companyName
                var getCompId = builder.GetCompId(resTable, resId).Build();
                result = await _sqldao.ReadSqlResult(getCompId);
                int compId = Convert.ToInt32(result.ValuesRead?.Rows[0]?["companyID"].ToString());
                var getCompName = builder.GetCompName(compId).Build();
                result = await _sqldao.ReadSqlResult(getCompName);
                string? companyName = result.ValuesRead?.Rows[0]?["companyName"].ToString();

                // startTime
                var getStart = builder
                    .BeginSelectAll()
                    .From(resTable)
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
                    .From(resTable)
                    .Where("reservationID = @ResId")
                    .AddParameters(new Dictionary<string, object>
                    {
                    { "ResId", resId }
                    })
                    .Build();
                result = await _sqldao.ReadSqlResult(getEnd);
                DateTime endTime = Convert.ToDateTime(result.ValuesRead?.Rows[0]?["reservationEndTime"]);

                // nextUser
                var getNext = builder.getNext(resId).Build();
                result = await _sqldao.ReadSqlResult(getNext);
                var nextUser = result.ValuesRead?.Rows[0]?["Username"].ToString();

                Console.WriteLine("UserHash for next user on the waitlist after top leaves: " + nextUser);

                // Populate WaitlistEntry with info needed for notification email
                WaitlistEntry entry = new WaitlistEntry
                {
                    userHash = nextUser,
                    spaceID = spaceId,
                    companyName = companyName,
                    startTime = startTime,
                    endTime = endTime,
                    position = 0
                };

                Console.WriteLine("Sending notification email (inside waitlistService 507)");
                await SendNotificationEmail(entry);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// This method removes a user at a certain reservation and position on a waitlist
        /// </summary>
        /// <param name="resId">reservation id</param>
        /// <param name="leavePos">current position of the user leaving</param>
        public async Task DeleteUser(int resId, int leavePos)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            // Delete the user who left
            var deleteUserCmd = builder
                .BeginDelete("Waitlist")
                .Where("ReservationID = @id AND Position = @pos")
                .AddParameters(new Dictionary<string, object>
                {
            { "id", resId },
            { "pos", leavePos }
                })
                .Build();
            result = await _sqldao.SqlRowsAffected(deleteUserCmd);

        }

        /// <summary>
        /// This method gets all of the rows in the waitlist table for a certain reservation id
        /// </summary>
        /// <param name="resId">reservation id</param>
        /// <returns>Response object</returns>
        public async Task<Response> SelectAll(int resId)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

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
            return result;
        }


        /// <summary>
        /// This method removes the user leaving the waitlist (unapproved for the reservation) and updates the position of the rest of the users
        /// </summary>
        /// <param name="resId">reservation id</param>
        /// <param name="leavePos">position of user leaving the waitlist</param>
        public async Task UpdateWaitlist_WaitlistedUserLeft(int resId, int leavePos)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            try
            {
                await DeleteUser(resId, leavePos);
                result = await SelectAll(resId);
                if (result.ValuesRead != null)
                {
                    foreach (DataRow row in result.ValuesRead.Rows)
                    {
                        int oldPosition = Convert.ToInt32(row["Position"]);
                        if (oldPosition > leavePos)
                        {
                            int newPosition = oldPosition - 1;
                            var updateCmd = builder.UpdatePositionWaitlisted(resId, oldPosition, newPosition).Build();
                            await _sqldao.SqlRowsAffected(updateCmd);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ValuesRead is null");
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// This method removes the user leaving the waitlist (unapproved for the reservation) and updates the position of the rest of the users
        /// </summary>
        /// <param name="resTable">reservation table name</param>
        /// <param name="userHash">hashed username</param>
        /// <returns>a list of waitlist entries containing information on all the waitlists that a certain user is currently on</returns>
        public async Task<List<WaitlistEntry>> GetUserWaitlists(string resTable, string userHash)
        {
            var builder = new CustomSqlCommandBuilder();
            var waitlists = new List<WaitlistEntry>();

            // Get waitlisted reservations for the specified user
            var getReservations = builder
                .BeginSelectAll()
                .From("Waitlist")
                .Where("Username = @user AND Position > 0")
                .AddParameters(new Dictionary<string, object>
                {
                    { "user", userHash }
                })
                .Build();

            var result = await _sqldao.ReadSqlResult(getReservations);

            if (result != null && result.ValuesRead != null)
            {
                foreach (DataRow row in result.ValuesRead.Rows)
                {
                    int reservationId = Convert.ToInt32(row["ReservationID"]);

                    // Get reservation details using reservationId
                    var getResDetails = builder
                        .BeginSelectAll()
                        .From(resTable)
                        .Where("reservationID = @id")
                        .AddParameters(new Dictionary<string, object>
                        {
                            { "id", reservationId }
                        })
                        .Build();

                    var resDetailsResult = await _sqldao.ReadSqlResult(getResDetails);
                    int compId = Convert.ToInt32(resDetailsResult.ValuesRead?.Rows[0]?["companyID"].ToString());

                    if (resDetailsResult != null && resDetailsResult.ValuesRead != null && resDetailsResult.ValuesRead.Rows.Count > 0)
                    {
                        string compName = await GetCompanyName(compId);
                        var entry = new WaitlistEntry
                        {
                            userHash = userHash,
                            spaceID = resDetailsResult.ValuesRead.Rows[0]["spaceID"].ToString(),
                            companyID = compId,
                            floorID = Convert.ToInt32(resDetailsResult.ValuesRead?.Rows[0]?["floorPlanID"].ToString()),
                            companyName = compName,
                            startTime = Convert.ToDateTime(resDetailsResult.ValuesRead.Rows[0]["reservationStartTime"]),
                            endTime = Convert.ToDateTime(resDetailsResult.ValuesRead.Rows[0]["reservationEndTime"]),
                            position = Convert.ToInt32(row["Position"])
                        };

                        waitlists.Add(entry);
                    }
                }
            }

            return waitlists;
        }

        /// <summary>
        /// This method gets the company name
        /// </summary>
        /// <param name="cid">company id</param>
        /// <returns>the company name based on the company id</returns>
        public async Task<string?> GetCompanyName(int cid)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            // Get company name using companyid
            var getCompanyName = builder
                .BeginSelectAll()
                .From("companyProfile")
                .Where("companyID = @id")
                .AddParameters(new Dictionary<string, object>
                {
                    { "id", cid }
                })
                .Build();

            result = await _sqldao.ReadSqlResult(getCompanyName);
            string compName = result.ValuesRead?.Rows[0]?["companyName"]?.ToString() ?? "Unknown Company";

            return compName;
        }

        /// <summary>
        /// This method gets the rervation details for a waitlist entry
        /// </summary>
        /// <param name="resTable">reservation table name</param>
        /// <param name="userHash">hashed username</param>
        /// <param name="reservationId">reservation id</param>
        /// <returns>Waitlist entry containing reservation details</returns>
        public async Task<WaitlistEntry?> GetReservationDetails(string resTable, string userHash, int reservationId)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            try
            {
                // Get reservation details using reservationId
                var getResDetails = builder
                    .BeginSelectAll()
                    .From(resTable)
                    .Where("reservationID = @id")
                    .AddParameters(new Dictionary<string, object>
                    {
                        { "id", reservationId }
                    })
                    .Build();

                var resDetailsResult = await _sqldao.ReadSqlResult(getResDetails);

                if (resDetailsResult.ValuesRead.Rows.Count == 0)
                {
                    throw new KeyNotFoundException($"Reservation with ID {reservationId} not found.");
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

                DateTime startTime = Convert.ToDateTime(resDetailsResult.ValuesRead.Rows[0]["reservationStartTime"]);
                DateTime endTime = Convert.ToDateTime(resDetailsResult.ValuesRead.Rows[0]["reservationEndTime"]);
                string formattedStartTime = startTime.ToString("h:mm tt");
                string formattedEndTime = endTime.ToString("h:mm tt");

                // Populate WaitlistEntry with reservation details
                var entry = new WaitlistEntry
                {
                    userHash = userHash,
                    spaceID = resDetailsResult.ValuesRead.Rows[0]["spaceID"].ToString(),
                    companyName = resDetailsResult.ValuesRead.Rows[0]["companyName"].ToString(),
                    startTime = startTime,
                    endTime = endTime,
                    position = pos
                };

                return entry;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "error" + ex.Message;
                return null;
            }

        }

        /// <summary>
        /// This method gets the company id given the name
        /// </summary>
        /// <param name="compName">company name</param>
        /// <returns>returns the company id</returns>
        public async Task<int> GetCompanyId(string compName)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            var getCIDCmd = builder.getCid(compName).Build();
            result = await _sqldao.ReadSqlResult(getCIDCmd);
            int cid = Convert.ToInt32(result.ValuesRead.Rows[0]["companyID"]);
            Console.WriteLine("service cid (725): " + cid);

            return cid;
        }

        /// <summary>
        /// This method gets the floor id given the company id
        /// </summary>
        /// <param name="tableName">reservation table name</param>
        /// <param name="compId">company id</param>
        /// <returns>floor id</returns>
        public async Task<int> GetFloorID(string tableName, int compId)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            var getFIDCmd = builder.getFid(tableName, compId).Build();
            result = await _sqldao.ReadSqlResult(getFIDCmd);
            int fid = Convert.ToInt32(result.ValuesRead.Rows[0]["floorPlanID"]);
            Console.WriteLine("service fid (738): " + fid);

            return fid;
        }

        /// <summary>
        /// This method gets the floor plan image
        /// </summary>
        /// <param name="compId">company id</param>
        /// <param name="floorId">floor id</param>
        /// <returns>the company floor image</returns>
        public async Task<IEnumerable<CompanyFloorStrImage>> GetCompanyFloorsAsync(int compId, int floorId)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            var floors = new Dictionary<int, CompanyFloorStrImage>();

            var command = builder.getFloor(compId, floorId).Build();

            Console.WriteLine("Command: " + command.CommandText);

            result = await _sqldao.ReadSqlResult(command);

            if (!result.HasError && result.ValuesRead != null)
            {
                foreach (DataRow row in result.ValuesRead.Rows)
                {
                    int floorPlanID = row["floorPlanID"] as int? ?? default(int);
                    string? floorPlanName = row["floorPlanName"]?.ToString().Trim();
                    byte[]? floorPlanImage = row["floorPlanImage"] as byte[];

                    CompanyFloorStrImage? floor;
                    if (!floors.TryGetValue(floorPlanID, out floor))
                    {
                        floor = new CompanyFloorStrImage
                        {
                            FloorPlanID = floorPlanID,
                            FloorPlanName = floorPlanName ?? string.Empty,
                            FloorPlanImageBase64 = floorPlanImage != null ? Convert.ToBase64String(floorPlanImage) : null,
                        };
                        floors.Add(floorPlanID, floor);
                    }
                }
            }
            else
            {
                Console.WriteLine("No data found or error occurred.");
            }

            Console.WriteLine("Returning floors");

            return floors.Values;
        }

        /// <summary>
        /// This method gets the reservation id
        /// </summary>
        /// <param name="tableName">reservation table name</param>
        /// <param name="compID">company id</param>
        /// <param name="floorID">floor id</param>
        /// <param name="spaceID">space id</param>
        /// <param name="sTime">reservation start time</param>
        /// <param name="eTime">reservation end time</param>
        /// <returns>the reservation id</returns>
        public async Task<int> GetReservationID(string tableName, int compID, int floorID, string spaceID, DateTime sTime, DateTime eTime)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            // Convert DateTime objects to string representation
            string startTime = sTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string endTime = eTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            var getResId = builder.getResIDWithConflict(tableName, compID, floorID, spaceID, startTime, endTime).Build();

            result = await _sqldao.ReadSqlResult(getResId);

            if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0)
            {
                int rid = Convert.ToInt32(result.ValuesRead.Rows[0]["reservationID"]);
                return rid;
            }
            else
            {
                Console.WriteLine("WaitlistService GetReservationID: No reservation found.");
                return -1;
            }
        }

        /// <summary>
        /// This method gets the reservation id
        /// </summary>
        /// <param name="resTable">reservation table name</param>
        /// <param name="compName">company name</param>
        /// <param name="spaceID">space id</param>
        /// <param name="sTime">reservation start time</param>
        /// <param name="eTime">reservation end time</param>
        /// <returns>the reservation id</returns>
        public async Task<int> GetReservationID_NoFloor(string resTable, string compName, string spaceID, DateTime sTime, DateTime eTime)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            // get compId
            var getCompId = builder
                .BeginSelectAll()
                .From("companyProfile")
                .Where("companyName = @name")
                .AddParameters(new Dictionary<string, object>
                {
                    { "name", compName }
                })
                .Build();

            result = await _sqldao.ReadSqlResult(getCompId);
            if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0)
            {
                int cid = Convert.ToInt32(result.ValuesRead.Rows[0]["companyID"]);

                // get floorId
                var getFloorId = builder
                    .BeginSelectAll()
                    .From(resTable)
                    .Where("companyID = @cid")
                    .AddParameters(new Dictionary<string, object>
                    {
                    { "cid", cid }
                    })
                    .Build();

                result = await _sqldao.ReadSqlResult(getFloorId);
                if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0)
                {
                    int fid = Convert.ToInt32(result.ValuesRead.Rows[0]["floorPlanID"]);
                    return await GetReservationID(resTable, cid, fid, spaceID, sTime, eTime);
                }
                else
                {
                    Console.WriteLine("No floorId found.");
                    return -1;
                }
            }
            else
            {
                Console.WriteLine("No company with that ID found.");
                return -1;
            }
        }

    }
}

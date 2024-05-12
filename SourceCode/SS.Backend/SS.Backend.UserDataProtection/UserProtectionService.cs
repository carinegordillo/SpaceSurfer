using SS.Backend.DataAccess;
using SS.Backend.Services.EmailService;
using SS.Backend.SharedNamespace;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;
using System.Data;
using System;
using System.IO;

using System.Text.Json;
using System.Security.Cryptography;

namespace SS.Backend.UserDataProtection;

public class UserProtectionService
{
    private readonly SqlDAO _sqldao;
    private readonly GenOTP genotp;
    private readonly Hashing hasher;

    public UserProtectionService(SqlDAO sqldao, GenOTP genotp, Hashing hasher)
    {
        _sqldao = sqldao;
        this.genotp = genotp;
        this.hasher = hasher;
    }

    public async Task<string> getUsername(string userhash)
    {
        var builder = new CustomSqlCommandBuilder();
        var result = new Response();

        var getUser = builder
            .BeginSelectAll()
            .From("userHash")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getUser);
        string? username = result.ValuesRead?.Rows[0]?["username"].ToString();

        return username;
    }

    public async Task<List<ReservationData>> GetReservations(string userhash)
    {
        var builder = new CustomSqlCommandBuilder();
        var result = new Response();

        var getReservations = builder
            .BeginSelectAll()
            .From("reservations")
            .Where($"userHash = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getReservations);

        List<ReservationData> reservations = new List<ReservationData>();

        if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0)
        {
            foreach (DataRow row in result.ValuesRead.Rows)
            {
                reservations.Add(new ReservationData
                {
                    ReservationID = Convert.ToInt32(row["reservationID"]),
                    CompanyID = Convert.ToInt32(row["companyID"]),
                    FloorPlanID = Convert.ToInt32(row["floorPlanID"]),
                    SpaceID = row["spaceID"].ToString(),
                    StartTime = Convert.ToDateTime(row["reservationStartTime"]),
                    EndTime = Convert.ToDateTime(row["reservationEndTime"]),
                    Status = row["status"].ToString()
                });
            }
        }

        return reservations;
    }

    public async Task<List<FloorData>> GetFloors(int companyID)
    {
        var builder = new CustomSqlCommandBuilder();
        var result = new Response();

        var getFloors = builder
            .BeginSelectAll()
            .From("companyFloor")
            .Where($"companyID = '{companyID}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getFloors);

        List<FloorData> floors = new List<FloorData>();

        if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0)
        {
            foreach (DataRow row in result.ValuesRead.Rows)
            {
                int floorID = Convert.ToInt32(row["floorPlanID"]);
                string floorName = row["floorPlanName"].ToString();

                var getSpaces = builder
                    .BeginSelectAll()
                    .From("companyFloorSpaces")
                    .Where($"floorPlanID = '{floorID}' AND companyID = '{companyID}'")
                    .Build();
                result = await _sqldao.ReadSqlResult(getSpaces);

                List<SpaceData> spaces = new List<SpaceData>();
                foreach (DataRow spaceRow in result.ValuesRead?.Rows)
                {
                    spaces.Add(new SpaceData
                    {
                        SpaceID = spaceRow["spaceID"].ToString(),
                        TimeLimit = Convert.ToInt32(spaceRow["timeLimit"])
                    });
                }

                floors.Add(new FloorData
                {
                    FloorID = floorID,
                    FloorName = floorName,
                    Spaces = spaces
                });
            }
        }

        return floors;
    }

    public async Task<List<WaitlistData>> GetWaitlists(string userhash)
    {
        var builder = new CustomSqlCommandBuilder();
        var result = new Response();

        var getWaitlists = builder
            .BeginSelectAll()
            .From("Waitlist")
            .Where($"Username = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getWaitlists);

        List<WaitlistData> waitlists = new List<WaitlistData>();

        if (result.ValuesRead != null && result.ValuesRead.Rows.Count > 0)
        {
            foreach (DataRow row in result.ValuesRead.Rows)
            {
                waitlists.Add(new WaitlistData
                {
                    ReservationID = Convert.ToInt32(row["ReservationID"]),
                    Position = Convert.ToInt32(row["Position"])
                });
            }
        }

        return waitlists;
    }

    public async Task<UserDataModel> accessData_GeneralUser(string userhash)
    {
        var builder = new CustomSqlCommandBuilder();
        var result = new Response();

        var username = await getUsername(userhash); // Username
        Console.WriteLine("Inside accessData_Gen: Username - " + username);

        var getFirstName = builder
            .BeginSelectAll()
            .From("userProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getFirstName);
        string? firstName = result.ValuesRead?.Rows[0]?["firstName"].ToString(); // First name
        Console.WriteLine("Inside accessData_Gen: First name - " + firstName);

        var getLastName = builder
            .BeginSelectAll()
            .From("userProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getLastName);
        string? lastName = result.ValuesRead?.Rows[0]?["lastName"].ToString(); // Last name
        Console.WriteLine("Inside accessData_Gen: Last name - " + lastName);

        var getBirthDate = builder
            .BeginSelectAll()
            .From("userAccount")
            .Where($"username = '{username}'")
            .Build();

        result = await _sqldao.ReadSqlResult(getBirthDate);
        DateTime birthDate = Convert.ToDateTime(result.ValuesRead?.Rows[0]?["birthDate"]); // Birth date
        Console.WriteLine("Inside accessData_Gen: Bday - " + birthDate);

        var getBackup = builder
            .BeginSelectAll()
            .From("userProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getBackup);
        string? backupEmail = result.ValuesRead?.Rows[0]?["backupEmail"].ToString(); // Backup email
        if (backupEmail == "string" || backupEmail == null)
        {
            backupEmail = null;
        }
        Console.WriteLine("Inside accessData_Gen: Backup - " + backupEmail);

        var getRole = builder
            .BeginSelectAll()
            .From("userProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getRole);
        int appRole = Convert.ToInt32(result.ValuesRead?.Rows[0]?["appRole"]); // App role
        Console.WriteLine("Inside accessData_Gen: Role - " + appRole);

        var getIsActive = builder
            .BeginSelectAll()
            .From("activeAccount")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getIsActive);
        string? activeStatus = result.ValuesRead?.Rows[0]?["isActive"].ToString(); // Is Active
        Console.WriteLine("Inside accessData_Gen: IsActive - " + activeStatus);

        var getOTP = builder
            .BeginSelectAll()
            .From("OTP")
            .Where($"Username = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getOTP);
        string? otp = result.ValuesRead?.Rows[0]?["OTP"].ToString(); // Last used OTP
        Console.WriteLine("Inside accessData_Gen: OTP - " + otp);

        List<ReservationData> reservations = await GetReservations(userhash); // Reservations
        Console.WriteLine("Inside accessData_Gen: got through reservations");

        List<WaitlistData> waitlists = await GetWaitlists(userhash); // Waitlists
        Console.WriteLine("Inside accessData_Gen: got through waitlists");

        UserDataModel userData = new UserDataModel
        {
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            BackupEmail = backupEmail,
            AppRole = appRole,
            IsActive = activeStatus,
            OTP = otp,
            Reservations = reservations,
            Waitlist = waitlists,
            CompanyName = null,
            CompanyID = -1,
            CompanyFloors = null,
            CompanyAddress = null,
            CompanyOpeningHours = TimeSpan.Zero,
            CompanyClosingHours = TimeSpan.Zero,
            CompanyDaysOpen = null
        };

        return userData;
    }

    public async Task<UserDataModel> accessData_Manager(string userhash)
    {
        var builder = new CustomSqlCommandBuilder();
        var result = new Response();

        var username = await getUsername(userhash); // Username

        var getFirstName = builder
            .BeginSelectAll()
            .From("userProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getFirstName);
        string? firstName = result.ValuesRead?.Rows[0]?["firstName"].ToString(); // First name

        var getLastName = builder
            .BeginSelectAll()
            .From("userProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getLastName);
        string? lastName = result.ValuesRead?.Rows[0]?["lastName"].ToString(); // Last name

        var getBirthDate = builder
            .BeginSelectAll()
            .From("userAccount")
            .Where($"username = '{username}'")
            .Build();

        result = await _sqldao.ReadSqlResult(getBirthDate);
        DateTime birthDate = Convert.ToDateTime(result.ValuesRead?.Rows[0]?["birthDate"]); // Birth date

        var getBackup = builder
            .BeginSelectAll()
            .From("userProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getBackup);
        string? backupEmail = result.ValuesRead?.Rows[0]?["backupEmail"].ToString(); // Backup email
        if (backupEmail == "string" || backupEmail == null)
        {
            backupEmail = null;
        }

        var getRole = builder
            .BeginSelectAll()
            .From("userProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getRole);
        int appRole = Convert.ToInt32(result.ValuesRead?.Rows[0]?["appRole"]); // App role

        var getIsActive = builder
            .BeginSelectAll()
            .From("activeAccount")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getIsActive);
        string? activeStatus = result.ValuesRead?.Rows[0]?["isActive"].ToString(); // Backup email

        var getOTP = builder
            .BeginSelectAll()
            .From("OTP")
            .Where($"Username = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getOTP);
        string? otp = result.ValuesRead?.Rows[0]?["OTP"].ToString(); // Last used OTP

        List<ReservationData> reservations = await GetReservations(userhash); // Reservations

        List<WaitlistData> waitlists = await GetWaitlists(userhash); // Waitlists

        var getCompanyName = builder
            .BeginSelectAll()
            .From("companyProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getCompanyName);
        string? companyName = result.ValuesRead?.Rows[0]?["companyName"].ToString(); // Company name

        var getCompanyID = builder
            .BeginSelectAll()
            .From("companyProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getCompanyID);
        int companyID = Convert.ToInt32(result.ValuesRead?.Rows[0]?["companyID"]); // Company ID

        var getAddress = builder
            .BeginSelectAll()
            .From("companyProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getAddress);
        string? address = result.ValuesRead?.Rows[0]?["address"].ToString(); // Address

        List<FloorData> floors = await GetFloors(companyID); // Company floors

        var getOpeningHours = builder
            .BeginSelectAll()
            .From("companyProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getOpeningHours);
        TimeSpan companyOpeningHours;
        string openingHoursStr = result.ValuesRead?.Rows[0]?["openingHours"].ToString();
        if (openingHoursStr != null && TimeSpan.TryParse(openingHoursStr, out TimeSpan openingHours))
        {
            companyOpeningHours = openingHours;
        }
        else
        {
            companyOpeningHours = TimeSpan.Zero;
        }

        var getClosingHours = builder
                .BeginSelectAll()
                .From("companyProfile")
                .Where($"hashedUsername = '{userhash}'")
                .Build();
        result = await _sqldao.ReadSqlResult(getClosingHours);
        TimeSpan companyClosingHours;
        string closingHoursStr = result.ValuesRead?.Rows[0]?["closingHours"].ToString();
        if (closingHoursStr != null && TimeSpan.TryParse(closingHoursStr, out TimeSpan closingHours))
        {
            companyClosingHours = closingHours;
        }
        else
        {
            companyClosingHours = TimeSpan.Zero;
        }

        var getDays = builder
            .BeginSelectAll()
            .From("companyProfile")
            .Where($"hashedUsername = '{userhash}'")
            .Build();
        result = await _sqldao.ReadSqlResult(getDays);
        string? companyDaysOpen = result.ValuesRead?.Rows[0]?["daysOpen"].ToString(); // Days open

        UserDataModel userData = new UserDataModel
        {
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            BackupEmail = backupEmail,
            AppRole = appRole,
            IsActive = activeStatus,
            OTP = otp,
            Reservations = reservations,
            Waitlist = waitlists,
            CompanyName = companyName,
            CompanyID = companyID,
            CompanyFloors = floors,
            CompanyAddress = address,
            CompanyOpeningHours = companyOpeningHours,
            CompanyClosingHours = companyClosingHours,
            CompanyDaysOpen = companyDaysOpen
        };

        return userData;
    }

    public async Task sendAccessEmail(UserDataModel userData, string attachmentPath)
    {
        var builder = new CustomSqlCommandBuilder();
        var result = new Response();

        string? subject = $@"Request for Data";
        string? msg = $@"
        Dear {userData.FirstName},

        Thank you for reaching out to us regarding your request for access to your data saved by our application.

        As per your request, please find attached a file containing your data. This information includes details such as your username, first name, last name, birth date, backup email, app role, active status, reservations, waitlist information, company details, and other relevant data that we have on file.

        Please review the attached document to ensure that it contains the information you requested. If you have any questions, concerns, or would like to request further details, please don't hesitate to contact us.

        We take the privacy and security of your data seriously, and we appreciate your cooperation in helping us ensure the accuracy and integrity of your information.

        Thank you for choosing our services.

        Best regards,
        Space Surfer Team";

        try
        {
            await MailSender.SendEmailWithAttachment(userData.Username, subject, msg, attachmentPath);
            result.HasError = false;
        }
        catch (Exception ex)
        {
            result.HasError = true;
            result.ErrorMessage = ex.Message;
        }

    }

    public async Task WriteToFile_GeneralUser(UserDataModel userData, string outputPath)
    {
        using (StreamWriter writer = File.CreateText(outputPath))
        {
            await writer.WriteLineAsync($"Username: {userData.Username}");
            await writer.WriteLineAsync($"BirthDate: {userData.BirthDate}");
            await writer.WriteLineAsync($"FirstName: {userData.FirstName}");
            await writer.WriteLineAsync($"LastName: {userData.LastName}");
            await writer.WriteLineAsync($"BackupEmail: {userData.BackupEmail}");
            await writer.WriteLineAsync($"AppRole: {userData.AppRole}");
            await writer.WriteLineAsync($"IsActive: {userData.IsActive}");
            await writer.WriteLineAsync();

            await writer.WriteLineAsync("Reservations:");
            foreach (var reservation in userData.Reservations)
            {
                await writer.WriteLineAsync($"  ReservationID: {reservation.ReservationID}");
                await writer.WriteLineAsync($"  CompanyID: {reservation.CompanyID}");
                await writer.WriteLineAsync($"  FloorPlanID: {reservation.FloorPlanID}");
                await writer.WriteLineAsync($"  SpaceID: {reservation.SpaceID}");
                await writer.WriteLineAsync($"  StartTime: {reservation.StartTime}");
                await writer.WriteLineAsync($"  EndTime: {reservation.EndTime}");
                await writer.WriteLineAsync($"  Status: {reservation.Status}");
                await writer.WriteLineAsync();
            }

            await writer.WriteLineAsync("Waitlist:");
            foreach (var waitlist in userData.Waitlist)
            {
                await writer.WriteLineAsync($"  ReservationID: {waitlist.ReservationID}");
                await writer.WriteLineAsync($"  Position: {waitlist.Position}");
                await writer.WriteLineAsync();
            }
        }
    }

    public async Task WriteToFile_Manager(UserDataModel userData, string outputPath)
    {
        using (StreamWriter writer = File.CreateText(outputPath))
        {
            await writer.WriteLineAsync($"Username: {userData.Username}");
            await writer.WriteLineAsync($"BirthDate: {userData.BirthDate}");
            await writer.WriteLineAsync($"FirstName: {userData.FirstName}");
            await writer.WriteLineAsync($"LastName: {userData.LastName}");
            await writer.WriteLineAsync($"BackupEmail: {userData.BackupEmail}");
            await writer.WriteLineAsync($"AppRole: {userData.AppRole}");
            await writer.WriteLineAsync($"IsActive: {userData.IsActive}");
            await writer.WriteLineAsync();

            await writer.WriteLineAsync("Reservations:");
            foreach (var reservation in userData.Reservations)
            {
                await writer.WriteLineAsync($"  ReservationID: {reservation.ReservationID}");
                await writer.WriteLineAsync($"  CompanyID: {reservation.CompanyID}");
                await writer.WriteLineAsync($"  FloorPlanID: {reservation.FloorPlanID}");
                await writer.WriteLineAsync($"  SpaceID: {reservation.SpaceID}");
                await writer.WriteLineAsync($"  StartTime: {reservation.StartTime}");
                await writer.WriteLineAsync($"  EndTime: {reservation.EndTime}");
                await writer.WriteLineAsync($"  Status: {reservation.Status}");
                await writer.WriteLineAsync();
            }

            await writer.WriteLineAsync("Waitlist:");
            foreach (var waitlist in userData.Waitlist)
            {
                await writer.WriteLineAsync($"  ReservationID: {waitlist.ReservationID}");
                await writer.WriteLineAsync($"  Position: {waitlist.Position}");
                await writer.WriteLineAsync();
            }

            await writer.WriteLineAsync($"CompanyName: {userData.CompanyName}");
            await writer.WriteLineAsync($"CompanyID: {userData.CompanyID}");
            await writer.WriteLineAsync($"CompanyAddress: {userData.CompanyAddress}");

            await writer.WriteLineAsync("CompanyFloors:");
            foreach (var floor in userData.CompanyFloors)
            {
                await writer.WriteLineAsync($"  FloorID: {floor.FloorID}");
                await writer.WriteLineAsync($"  FloorName: {floor.FloorName}");

                await writer.WriteLineAsync("  Spaces:");
                foreach (var space in floor.Spaces)
                {
                    await writer.WriteLineAsync($"    SpaceID: {space.SpaceID}");
                    await writer.WriteLineAsync($"    TimeLimit: {space.TimeLimit}");
                }

                await writer.WriteLineAsync();
            }

            await writer.WriteLineAsync($"CompanyOpeningHours: {userData.CompanyOpeningHours}");
            await writer.WriteLineAsync($"CompanyClosingHours: {userData.CompanyClosingHours}");
            await writer.WriteLineAsync($"CompanyDaysOpen: {userData.CompanyDaysOpen}");

        }
    }

    // just deleting logs for now
    public async Task deleteData(string userhash)
    {
        var builder = new CustomSqlCommandBuilder();
        var result = new Response();

        var deleteCmd = builder.deleteLogs(userhash).Build();
        result = await _sqldao.SqlRowsAffected(deleteCmd);
    }

    public async Task sendDeleteEmail(UserDataModel userData, string attachmentPath)
    {
        var builder = new CustomSqlCommandBuilder();
        var result = new Response();

        string? subject = $@"Confirmation of Data Deletion Request";
        string? msg = $@"
        Dear {userData.FirstName},

        We have received your request to delete your data in accordance with the Personal Information Protection and California Privacy Rights Act (PII and CPRA) data laws. We understand and respect your privacy concerns and are committed to complying with these regulations.

        Your data deletion request has been processed, and we have taken the necessary steps to remove all identifiable personal information associated with your account from our system. This includes details such as your username, first name, last name, birth date, backup email, app role, reservations, waitlist information, company details, and any other relevant data that we may have on file.

        Please note that while your personal information has been deleted from our active databases, some residual data may remain in our backup systems for a limited period as part of our data retention policies. Rest assured, this data is securely stored and will be permanently deleted in accordance with our retention schedules.

        If you have any further questions or concerns regarding your data deletion request, or if you believe there has been an error in the deletion process, please feel free to contact us at spacesurfers5@gmail.com. We are here to assist you and ensure that your privacy rights are upheld.

        Thank you for your understanding and cooperation in this matter and look forward to seeing you again.

        Best regards,
        Space Surfer Team";

        try
        {
            Console.WriteLine("Inside method (email): " + userData.Username);
            Console.WriteLine("Inside method (output path): " + attachmentPath);
            await MailSender.SendEmailWithAttachment(userData.Username, subject, msg, attachmentPath);
            result.HasError = false;
        }
        catch (Exception ex)
        {
            result.HasError = true;
            result.ErrorMessage = ex.Message;
        }
    }

    public async Task<(string otp, Response res)> SendCode(string userhash)
    {
        var builder = new CustomSqlCommandBuilder();
        Response result = new();

        try
        {
            // generate the otp and salt
            string otp = genotp.generateOTP();
            string salt = hasher.GenSalt();

            // hash the otp
            string hashedOTP = hasher.HashData(otp, salt);

            // check if username already tried authenticating before, ie username exists in otp table
            var otpRead = builder
                .BeginSelectAll()
                .From("OTP")
                .Where($"Username = '{userhash}'")
                .Build();
            result = await _sqldao.ReadSqlResult(otpRead);
            if (result.ValuesRead == null || result.ValuesRead.Rows.Count == 0)
            {
                // create and execute the sql command to insert the username, otp, salt, and timestamp to the DB
                var parameters = new Dictionary<string, object>
                    {
                        { "OTP", hashedOTP },
                        { "Salt", salt },
                        { "Timestamp", DateTime.UtcNow },
                        { "Username", userhash }
                    };
                var insertCommand = builder
                    .BeginInsert("OTP")
                    .Columns(parameters.Keys)
                    .Values(parameters.Keys)
                    .AddParameters(parameters)
                    .Build();
                result = await _sqldao.SqlRowsAffected(insertCommand);

                return (otp, result);
            }
            else
            {
                var parameters = new Dictionary<string, object>
                    {
                        { "OTP", hashedOTP },
                        { "Salt", salt },
                        { "Timestamp", DateTime.UtcNow },
                        { "Username", userhash }
                    };
                var updateCommand = builder
                    .BeginUpdate("OTP")
                    .Set(parameters)
                    .Where("Username = @username")
                    .AddParameters(parameters)
                    .Build();

                //update the otp and salt in table for that user
                result = await _sqldao.SqlRowsAffected(updateCommand);

                return (otp, result);
            }
        }
        catch (Exception ex)
        {
            result.HasError = true;
            result.ErrorMessage = ex.Message;
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return (null, result);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }
    }

    public async Task<Response> VerifyCode(string userhash, string otp)
    {
        var builder = new CustomSqlCommandBuilder();
        Response result = new();

        try
        {
            // create and execute sql command to read the hashedOTP from the DB
            var selectCommand = builder
                .BeginSelectAll()
                .From("OTP")
                .Where($"Username = '{userhash}'")
                .Build();
            result = await _sqldao.ReadSqlResult(selectCommand);

            string? dbOTP = result.ValuesRead?.Rows[0]?["OTP"].ToString();
            string? dbSalt = result.ValuesRead?.Rows[0]?["Salt"].ToString();
            DateTime timestamp = result.ValuesRead?.Rows[0]?["Timestamp"] != DBNull.Value
            ? (DateTime)result.ValuesRead.Rows[0]["Timestamp"]
            : DateTime.MinValue;

            TimeSpan timeElapsed = DateTime.UtcNow - timestamp;

            // compare the otp stored in DB with user inputted otp
            string HashedProof = hasher.HashData(otp, dbSalt);
            if (dbOTP == HashedProof)
            {
                if (timeElapsed.TotalMinutes > 2)
                {
                    result.HasError = true;
                    result.ErrorMessage = "OTP has expired.";
                    return result;
                }
                else
                {
                    result.HasError = false;
                    result.ErrorMessage = "Code matches.";
                    return result;
                }

            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = "Failed to authenticate.";
                return result;
            }
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            return result;
        }

    }
}

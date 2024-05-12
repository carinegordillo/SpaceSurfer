using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Data;

namespace SS.Backend.Services.DeletingService
{
    /// <summary>
    ///     DatabaseHelper class is responsible of retrieving table names from a database
    /// </summary>
    ///
    public class DatabaseHelper : IDatabaseHelper
    {

        private ISqlDAO _sqlDAO;
        private ConfigService? configService;

        public DatabaseHelper(ISqlDAO sqlDAO)
        {
            _sqlDAO = sqlDAO;
        }

        public async Task<string?> getUsername(string userhash)
        {
            var builder = new CustomSqlCommandBuilder();
            var result = new Response();

            var getUser = builder
                .BeginSelectAll()
                .From("userHash")
                .Where($"hashedUsername = '{userhash}'")
                .Build();
            result = await _sqlDAO.ReadSqlResult(getUser);
            string? username = result.ValuesRead?.Rows[0]?["username"].ToString();

            return username;
        }

        public async Task<Response> DeleteAccount(string username)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {
                var commandBuild = new CustomSqlCommandBuilder();
                Console.WriteLine("Inside DatabaseHelper.cs - beginning queries");

                // get email from userHash
                Console.WriteLine("Inside DatabaseHelper.cs - get email");
                var email = await getUsername(username);
                Console.WriteLine("email: " + email);
                // delete userAccount
                Console.WriteLine("Inside DatabaseHelper.cs - delete userAccount");
                var query = commandBuild.deleteUserAccount(email).Build();
                response = await _sqlDAO.SqlRowsAffected(query);
                response.HasError = false;
                response.ErrorMessage = "";
                // delete userHash
                Console.WriteLine("Inside DatabaseHelper.cs - delete userHash");
                query = commandBuild.deleteUserHash(username).Build();
                response = await _sqlDAO.SqlRowsAffected(query);
                response.HasError = false;
                response.ErrorMessage = "";
                // delete userProfile
                Console.WriteLine("Inside DatabaseHelper.cs - delete userProfile");
                query = commandBuild.deleteUserProfile(username).Build();
                response = await _sqlDAO.SqlRowsAffected(query);
                response.HasError = false;
                response.ErrorMessage = "";
                // delete activeAccount
                Console.WriteLine("Inside DatabaseHelper.cs - delete activeAccount");
                query = commandBuild.deleteActiveAccount(username).Build();
                response = await _sqlDAO.SqlRowsAffected(query);
                response.HasError = false;
                response.ErrorMessage = "";
                // delete userRequests
                query = commandBuild.deleteUserRequests(username).Build();
                response = await _sqlDAO.SqlRowsAffected(query);
                response.HasError = false;
                response.ErrorMessage = "";
                // get redId from reservations
                Console.WriteLine("Inside DatabaseHelper.cs - get resId");
                query = commandBuild.getResId(username).Build();
                response = await _sqlDAO.ReadSqlResult(query);
                response.HasError = false;
                response.ErrorMessage = "";
                var reservationIds = new List<int>();
                foreach (DataRow row in response.ValuesRead?.Rows)
                {
                    reservationIds.Add(Convert.ToInt32(row["reservationID"]));
                }
                // delete ConfirmReservations
                Console.WriteLine("Inside DatabaseHelper.cs - delete ConfirmReservations");
                foreach (var reservationId in reservationIds)
                {
                    query = commandBuild.deleteConfirmReservations(reservationId).Build();
                    response = await _sqlDAO.SqlRowsAffected(query);
                    response.HasError = false;
                    response.ErrorMessage = "";
                }
                // delete reservations
                Console.WriteLine("Inside DatabaseHelper.cs - delete reservations");
                query = commandBuild.deleteReservations(username).Build();
                response = await _sqlDAO.SqlRowsAffected(query);
                response.HasError = false;
                response.ErrorMessage = "";
                // get compId from companyProfile
                Console.WriteLine("Inside DatabaseHelper.cs - get compId");
                query = commandBuild.getCompId(username).Build();
                response = await _sqlDAO.ReadSqlResult(query);
                response.HasError = false;
                response.ErrorMessage = "";
                var companyIds = new List<int>();
                foreach (DataRow row in response.ValuesRead?.Rows)
                {
                    companyIds.Add(Convert.ToInt32(row["companyID"]));
                }
                // delete companyFloor and companyFloorSpaces
                Console.WriteLine("Inside DatabaseHelper.cs - delete companyFloor and floor spaces");
                foreach (var companyId in companyIds)
                {
                    query = commandBuild.deleteCompanyFloor(companyId).Build();
                    response = await _sqlDAO.SqlRowsAffected(query);
                    response.HasError = false;
                    response.ErrorMessage = "";
                    query = commandBuild.deleteCompanyFloorSpaces(companyId).Build();
                    response = await _sqlDAO.SqlRowsAffected(query);
                    response.HasError = false;
                    response.ErrorMessage = "";
                }
                // delete companyProfile
                Console.WriteLine("Inside DatabaseHelper.cs - delete companyProfile");
                query = commandBuild.deleteCompanyProfile(username).Build();
                response = await _sqlDAO.SqlRowsAffected(query);
                response.HasError = false;
                response.ErrorMessage = "";
                // delete OTP
                Console.WriteLine("Inside DatabaseHelper.cs - delete OTP");
                query = commandBuild.deleteOTP(username).Build();
                response = await _sqlDAO.SqlRowsAffected(query);
                response.HasError = false;
                response.ErrorMessage = "";
                // delete Waitlist
                query = commandBuild.deleteWaitlist(username).Build();
                response = await _sqlDAO.SqlRowsAffected(query);
                response.HasError = false;
                response.ErrorMessage = "";

                if (!response.HasError)
                {

                    Console.WriteLine("Inside DatabaseHelper.cs - delete success");
                    // Successful Deletion
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Database Helper Account Deletion"
                    };

                    await logger.SaveData(entry);
                }
                else
                {
                    Console.WriteLine("Inside DatabaseHelper.cs - delete fail");
                    //Unsuccessful Deletion
                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Database Helper Account Deletion"
                    };

                    await logger.SaveData(errorEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Inside DatabaseHelper.cs - delete fail, in catch");
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data Store",
                    description = "Database Helper Service Encounterd An Error"
                };

                await logger.SaveData(errorEntry);
            }

            //var tableNames = commandBuild.BeginSelect().SelectOne("Username").From("dbo.userAccount ").Where("Username = '@user';").Build();



            return response;

        }
    }
}

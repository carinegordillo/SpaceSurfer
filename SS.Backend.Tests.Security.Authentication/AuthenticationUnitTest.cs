namespace SS.Backend.Tests.Security.Authentication
{
    [TestClass]
    public class AuthenticationUnitTest
    {
        //    private async Task CleanupTestData()
        //    {
        //        var SAUser = Credential.CreateSAUser();
        //        var connectionString = string.Format(@"Data Source=localhost\SpaceSurfer;Initial Catalog=SS_Server;User Id={0};Password={1};", SAUser.user, SAUser.pass);
        //        try
        //        {
        //            using (SqlConnection connection = new SqlConnection(connectionString))
        //            {
        //                await connection.OpenAsync();

        //                string sql1 = $"DELETE FROM OTP WHERE Username = 'test@email'";
        //                string sql2 = $"DELETE FROM userProfile WHERE hashedUsername = 'test@email'";
        //                string sql3 = $"DELETE FROM Logs WHERE Username = 'test@email'";

        //                using (SqlCommand command1 = new SqlCommand(sql1, connection))
        //                {
        //                    await command1.ExecuteNonQueryAsync();
        //                }
        //                using (SqlCommand command2 = new SqlCommand(sql2, connection))
        //                {
        //                    await command2.ExecuteNonQueryAsync();
        //                }
        //                using (SqlCommand command3 = new SqlCommand(sql3, connection))
        //                {
        //                    await command3.ExecuteNonQueryAsync();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Exception during test cleanup: {ex}");
        //        }
        //    }

        //    [TestMethod]
        //    public async Task Authenticate_RegisteredUser_AuthenticatedBefore_Pass()
        //    {
        //        // Arrange
        //        Response result = new Response();
        //        var builder = new CustomSqlCommandBuilder();
        //        string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.local.txt");
        //        ConfigService configService = new ConfigService(configFilePath);
        //        GenOTP genotp = new GenOTP();
        //        Hashing hasher = new Hashing();
        //        SqlDAO dao = new SqlDAO(configService);
        //        SqlLogTarget target = new SqlLogTarget(dao);
        //        Logger log = new Logger(target);
        //        SSAuthService auth = new SSAuthService(genotp, hasher, dao, log);
        //        AuthenticationRequest request = new AuthenticationRequest();
        //        SSPrincipal principal = new SSPrincipal();
        //        Stopwatch timer = new Stopwatch();

        //        var parameters = new Dictionary<string, object>
        //        {
        //            { "hashedUsername", "test@email" },
        //            { "firstName", "benny" },
        //            { "lastName", "bennington" },
        //            { "backupEmail", "backup@email" },
        //            { "appRole", "1" }
        //        };
        //        var insertCommand = builder
        //            .BeginInsert("userProfile")
        //            .Columns(parameters.Keys)
        //            .Values(parameters.Keys)
        //            .AddParameters(parameters)
        //            .Build();
        //        await dao.SqlRowsAffected(insertCommand);

        //        var parameters2 = new Dictionary<string, object>
        //        {
        //            { "OTP", "previousOTP" },
        //            { "Salt", "testSalt" },
        //            { "Timestamp", "2023-12-18 03:10:21.470" },
        //            { "Username", "test@email" }
        //        };
        //        var insertCommand2 = builder
        //            .BeginInsert("OTP")
        //            .Columns(parameters2.Keys)
        //            .Values(parameters2.Keys)
        //            .AddParameters(parameters2)
        //            .Build();
        //        await dao.SqlRowsAffected(insertCommand2);

        //        request.UserIdentity = "test@email";
        //        request.Proof = null;

        //        (string otp, result) = await auth.SendOTP_and_SaveToDB(request);

        //        request.Proof = otp;

        //        // Act
        //        timer.Start();
        //        (principal, result) = await auth.Authenticate(request);
        //        timer.Stop();

        //        // Assert
        //        Assert.IsFalse(result.HasError);
        //        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //        // Cleanup
        //        await CleanupTestData();
        //    }


        //    [TestMethod]
        //    public async Task Authenticate_RegisteredUser_NeverAuthenticatedBefore_Pass()
        //    {
        //        // Arrange
        //        Response result = new Response();
        //        var builder = new CustomSqlCommandBuilder();
        //        string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.local.txt");
        //        ConfigService configService = new ConfigService(configFilePath);
        //        GenOTP genotp = new GenOTP();
        //        Hashing hasher = new Hashing();
        //        SqlDAO dao = new SqlDAO(configService);
        //        SqlLogTarget target = new SqlLogTarget(dao);
        //        Logger log = new Logger(target);
        //        SSAuthService auth = new SSAuthService(genotp, hasher, dao, log);
        //        AuthenticationRequest request = new AuthenticationRequest();
        //        SSPrincipal principal = new SSPrincipal();
        //        Stopwatch timer = new Stopwatch();

        //        var parameters = new Dictionary<string, object>
        //        {
        //            { "hashedUsername", "test@email" },
        //            { "firstName", "benny" },
        //            { "lastName", "bennington" },
        //            { "backupEmail", "backup@email" },
        //            { "appRole", "1" }
        //        };
        //        var insertCommand = builder
        //            .BeginInsert("userProfile")
        //            .Columns(parameters.Keys)
        //            .Values(parameters.Keys)
        //            .AddParameters(parameters)
        //            .Build();
        //        await dao.SqlRowsAffected(insertCommand);

        //        request.UserIdentity = "test@email";
        //        request.Proof = null;

        //        (string otp, result) = await auth.SendOTP_and_SaveToDB(request);

        //        request.Proof = otp;

        //        // Act
        //        timer.Start();
        //        (principal, result) = await auth.Authenticate(request);
        //        timer.Stop();

        //        // Assert
        //        Assert.IsFalse(result.HasError);
        //        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //        //Cleanup
        //        await CleanupTestData();
        //    }

        //    [TestMethod]
        //    public async Task Authenticate_NonRegisteredUser_Pass()
        //    {
        //        // Arrange
        //        Response result = new Response();
        //        var builder = new CustomSqlCommandBuilder();
        //        string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.local.txt");
        //        ConfigService configService = new ConfigService(configFilePath);
        //        GenOTP genotp = new GenOTP();
        //        Hashing hasher = new Hashing();
        //        SqlDAO dao = new SqlDAO(configService);
        //        SqlLogTarget target = new SqlLogTarget(dao);
        //        Logger log = new Logger(target);
        //        SSAuthService auth = new SSAuthService(genotp, hasher, dao, log);
        //        AuthenticationRequest request = new AuthenticationRequest();
        //        SSPrincipal principal = new SSPrincipal();
        //        Stopwatch timer = new Stopwatch();

        //        request.UserIdentity = "test@email";
        //        request.Proof = null;

        //        // Act
        //        timer.Start();
        //        (string otp, result) = await auth.SendOTP_and_SaveToDB(request);
        //        timer.Stop();

        //        // Assert
        //        Assert.IsTrue(result.HasError);
        //        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //        //Cleanup
        //        await CleanupTestData();
        //    }

        //    [TestMethod]
        //    public async Task Authenticate_ValidOtp_Pass()
        //    {
        //        // Arrange
        //        Response result = new Response();
        //        var builder = new CustomSqlCommandBuilder();
        //        string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.local.txt");
        //        ConfigService configService = new ConfigService(configFilePath);
        //        GenOTP genotp = new GenOTP();
        //        Hashing hasher = new Hashing();
        //        SqlDAO dao = new SqlDAO(configService);
        //        SqlLogTarget target = new SqlLogTarget(dao);
        //        Logger log = new Logger(target);
        //        SSAuthService auth = new SSAuthService(genotp, hasher, dao, log);
        //        AuthenticationRequest request = new AuthenticationRequest();
        //        SSPrincipal principal = new SSPrincipal();
        //        Stopwatch timer = new Stopwatch();

        //        var parameters = new Dictionary<string, object>
        //        {
        //            { "hashedUsername", "test@email" },
        //            { "firstName", "benny" },
        //            { "lastName", "bennington" },
        //            { "backupEmail", "backup@email" },
        //            { "appRole", "1" }
        //        };
        //        var insertCommand = builder
        //            .BeginInsert("userProfile")
        //            .Columns(parameters.Keys)
        //            .Values(parameters.Keys)
        //            .AddParameters(parameters)
        //            .Build();
        //        await dao.SqlRowsAffected(insertCommand);

        //        var parameters2 = new Dictionary<string, object>
        //        {
        //            { "OTP", "previousOTP" },
        //            { "Salt", "testSalt" },
        //            { "Timestamp", "2023-12-18 03:10:21.470" },
        //            { "Username", "test@email" }
        //        };
        //        var insertCommand2 = builder
        //            .BeginInsert("OTP")
        //            .Columns(parameters2.Keys)
        //            .Values(parameters2.Keys)
        //            .AddParameters(parameters2)
        //            .Build();
        //        await dao.SqlRowsAffected(insertCommand2);

        //        request.UserIdentity = "test@email";
        //        request.Proof = null;

        //        (string otp, result) = await auth.SendOTP_and_SaveToDB(request);

        //        request.Proof = otp;

        //        // Act
        //        timer.Start();
        //        (principal, result) = await auth.Authenticate(request);
        //        timer.Stop();

        //        // Assert
        //        Assert.IsFalse(result.HasError);
        //        Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

        //        //Cleanup
        //        await CleanupTestData();
        //    }

        //    [TestMethod]
        //    public async Task Authenticate_ExpiredOtp_Pass()
        //    {
        //        // Arrange
        //        Response result = new Response();
        //        var builder = new CustomSqlCommandBuilder();
        //        string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.local.txt");
        //        ConfigService configService = new ConfigService(configFilePath);
        //        GenOTP genotp = new GenOTP();
        //        Hashing hasher = new Hashing();
        //        SqlDAO dao = new SqlDAO(configService);
        //        SqlLogTarget target = new SqlLogTarget(dao);
        //        Logger log = new Logger(target);
        //        SSAuthService auth = new SSAuthService(genotp, hasher, dao, log);
        //        AuthenticationRequest request = new AuthenticationRequest();
        //        SSPrincipal principal = new SSPrincipal();
        //        Stopwatch timer = new Stopwatch();

        //        var parameters = new Dictionary<string, object>
        //        {
        //            { "hashedUsername", "test@email" },
        //            { "firstName", "benny" },
        //            { "lastName", "bennington" },
        //            { "backupEmail", "backup@email" },
        //            { "appRole", "1" }
        //        };
        //        var insertCommand = builder
        //            .BeginInsert("userProfile")
        //            .Columns(parameters.Keys)
        //            .Values(parameters.Keys)
        //            .AddParameters(parameters)
        //            .Build();
        //        await dao.SqlRowsAffected(insertCommand);

        //        var parameters2 = new Dictionary<string, object>
        //        {
        //            { "OTP", "previousOTP" },
        //            { "Salt", "testSalt" },
        //            { "Timestamp", "2023-12-18 03:10:21.470" },
        //            { "Username", "test@email" }
        //        };
        //        var insertCommand2 = builder
        //            .BeginInsert("OTP")
        //            .Columns(parameters2.Keys)
        //            .Values(parameters2.Keys)
        //            .AddParameters(parameters2)
        //            .Build();
        //        await dao.SqlRowsAffected(insertCommand2);

        //        request.UserIdentity = "test@email";
        //        request.Proof = null;

        //        (string otp, result) = await auth.SendOTP_and_SaveToDB(request);

        //        request.Proof = otp;

        //        // Act
        //        Thread.Sleep(120000);
        //        (principal, result) = await auth.Authenticate(request);

        //        // Assert
        //        Assert.IsTrue(result.HasError);

        //        //Cleanup
        //        await CleanupTestData();
        //    }

    }
}
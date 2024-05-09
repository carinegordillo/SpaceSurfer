using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using SS.Backend.SystemObservability;

namespace SS.Backend.Tests.UsageAnalysisObeservability
{
    [TestClass]
    public class SystemObservabilityTests
    {
        private Logger logger;
        private Response response;
        private CustomSqlCommandBuilder commandBuilder;
        private ISqlDAO sqlDAO;
        private ConfigService configService;
        private ISystemObservabilityDAO _systemObservabilityDAOService;
        private IViewDurationService _viewDurationService;
        private ILoginCountService _loginCountService;
        private ICompanyReservationCountService _companyReservationCountService;
        private IMostUsedFeatureService _mostUsedFeatureService;
        private IRegistrationCountService _registrationCountService;
        private ICompanySpaceCountService _companySpaceCountService;

        [TestInitialize]
        public void TestInitialize()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

            response = new Response();
            commandBuilder = new CustomSqlCommandBuilder();
            configService = new ConfigService(configFilePath);
            sqlDAO = new SqlDAO(configService);
            _systemObservabilityDAOService = new SystemObservabilityDAO(sqlDAO);
            _viewDurationService = new ViewDurationService(_systemObservabilityDAOService);
            _loginCountService = new LoginCountService(_systemObservabilityDAOService);
            _companyReservationCountService = new CompanyReservationCountService(_systemObservabilityDAOService);
            _registrationCountService = new RegistrationCountService(_systemObservabilityDAOService);
            _companySpaceCountService = new CompanySpaceCountService(_systemObservabilityDAOService);
            _mostUsedFeatureService = new MostUsedFeatureService(_systemObservabilityDAOService);

        }

        [TestMethod]
        public async Task InsertViewDuration_Success()
        {
            Response response = new Response();
            Random random = new Random();
            int duration = random.Next(20, 250);
            int viewNum = random.Next(1, 10);

            response = await _viewDurationService.InsertViewDuration("qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=", $"View {viewNum}", duration);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.HasError);

        }

        [TestMethod]
        public async Task RetrievingTop3ViewDurations_6_months_Success()
        {
            Response response = new Response();

            try
            {
                for (int i = 0; i < 25; i++)
                {
                    Random random = new Random();
                    int duration = random.Next(20, 250);
                    int month = random.Next(1, 12);
                    int day = random.Next(1, 28);
                    int viewNum = i + 5;

                    var parameters = new Dictionary<string, object>
                {
                    { "hashedUsername", "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=" },
                    { "viewName", $"View {viewNum}"},
                    { "durationInSeconds", duration },
                    { "TimeStamp", $"2024-{month}-{day} 11:33:19.070"}
                };

                    commandBuilder = new CustomSqlCommandBuilder();

                    var cmd = commandBuilder.BeginInsert("ViewDurations")
                                                .Columns(parameters.Keys)
                                                .Values(parameters.Keys)
                                                .AddParameters(parameters)
                                                .Build();

                    response = await sqlDAO.SqlRowsAffected(cmd);

                }

                var durations = await _viewDurationService.GetTop3ViewDuration("qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=", "6 months");

                foreach (var duration in durations)
                {
                    Console.WriteLine($"DurationInSeconds: {duration.DurationInSeconds}");
                    Console.WriteLine($"ViewName: {duration.ViewName}");
                    Console.WriteLine();
                }


                var query = commandBuilder.BeginDelete("dbo.ViewDurations").Where("hashedUsername = 'qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY='").Build();

                response = await sqlDAO.SqlRowsAffected(query);

                Assert.IsNotNull(durations);
                Assert.IsFalse(response.HasError);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }


        }

        [TestMethod]
        public async Task RetrievingTop3ViewDurations_12_months_Success()
        {
            Response response = new Response();

            try
            {
                for (int i = 0; i < 25; i++)
                {
                    Random random = new Random();
                    int duration = random.Next(20, 250);
                    int month = random.Next(1, 12);
                    int day = random.Next(1, 28);
                    int viewNum = i + 5;

                    var parameters = new Dictionary<string, object>
                {
                    { "hashedUsername", "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=" },
                    { "viewName", $"View {viewNum}"},
                    { "durationInSeconds", duration },
                    { "TimeStamp", $"2024-{month}-{day} 11:33:19.070"}
                };

                    commandBuilder = new CustomSqlCommandBuilder();

                    var cmd = commandBuilder.BeginInsert("ViewDurations")
                                                .Columns(parameters.Keys)
                                                .Values(parameters.Keys)
                                                .AddParameters(parameters)
                                                .Build();

                    response = await sqlDAO.SqlRowsAffected(cmd);

                }

                var durations = await _viewDurationService.GetTop3ViewDuration("qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=", "12 months");

                foreach (var duration in durations)
                {
                    Console.WriteLine($"DurationInSeconds: {duration.DurationInSeconds}");
                    Console.WriteLine($"ViewName: {duration.ViewName}");
                    Console.WriteLine();
                }


                var query = commandBuilder.BeginDelete("dbo.ViewDurations").Where("hashedUsername = 'qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY='").Build();

                response = await sqlDAO.SqlRowsAffected(query);

                Assert.IsNotNull(durations);
                Assert.IsFalse(response.HasError);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task RetrievingTop3ViewDurations_24_months_Success()
        {
            Response response = new Response();

            try
            {
                for (int i = 0; i < 35; i++)
                {
                    Random random = new Random();
                    int duration = random.Next(20, 250);
                    int year = random.Next(21, 24);
                    int month = random.Next(1, 12);
                    int day = random.Next(1, 28);
                    int viewNum = i + 5;

                    var parameters = new Dictionary<string, object>
                {
                    { "hashedUsername", "qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=" },
                    { "viewName", $"View {viewNum}"},
                    { "durationInSeconds", duration },
                    { "TimeStamp", $"20{year}-{month}-{day} 11:33:19.070"}
                };

                    commandBuilder = new CustomSqlCommandBuilder();

                    var cmd = commandBuilder.BeginInsert("ViewDurations")
                                                .Columns(parameters.Keys)
                                                .Values(parameters.Keys)
                                                .AddParameters(parameters)
                                                .Build();

                    response = await sqlDAO.SqlRowsAffected(cmd);

                }

                var durations = await _viewDurationService.GetTop3ViewDuration("qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=", "24 months");

                foreach (var duration in durations)
                {
                    Console.WriteLine($"DurationInSeconds: {duration.DurationInSeconds}");
                    Console.WriteLine($"ViewName: {duration.ViewName}");
                    Console.WriteLine();
                }


                var query = commandBuilder.BeginDelete("dbo.ViewDurations").Where("hashedUsername = 'qGkjtLi+R/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY='").Build();

                response = await sqlDAO.SqlRowsAffected(query);

                Assert.IsNotNull(durations);
                Assert.IsFalse(response.HasError);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }


        }


        [TestMethod]
        public async Task RetrievingLogCount_6_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var logCounts = await _loginCountService.GetLoginCount("testLOGS", "6 months");

                foreach (var logCount in logCounts)
                {
                    Console.WriteLine($"Month: {logCount.Month}");
                    Console.WriteLine($"Year: {logCount.Year}");
                    Console.WriteLine($"Failed Logins: {logCount.FailedLogins}");
                    Console.WriteLine($"Successful Logins: {logCount.SuccessfulLogins}");
                    Console.WriteLine();
                }

                Assert.IsNotNull(logCounts);



            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }


        [TestMethod]
        public async Task RetrievingLogCount_12_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var logCounts = await _loginCountService.GetLoginCount("testLOGS", "12 months");

                foreach (var logCount in logCounts)
                {
                    Console.WriteLine($"Month: {logCount.Month}");
                    Console.WriteLine($"Year: {logCount.Year}");
                    Console.WriteLine($"Failed Logins: {logCount.FailedLogins}");
                    Console.WriteLine($"Successful Logins: {logCount.SuccessfulLogins}");
                    Console.WriteLine();
                }


                Assert.IsNotNull(logCounts);


            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task RetrievingLogCount_24_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var logCounts = await _loginCountService.GetLoginCount("testLOGS", "24 months");

                foreach (var logCount in logCounts)
                {
                    Console.WriteLine($"Month: {logCount.Month}");
                    Console.WriteLine($"Year: {logCount.Year}");
                    Console.WriteLine($"Failed Logins: {logCount.FailedLogins}");
                    Console.WriteLine($"Successful Logins: {logCount.SuccessfulLogins}");
                    Console.WriteLine();
                }


                Assert.IsNotNull(logCounts);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task RetrievingTop3CompanyWithMostReservations_6_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var companyReservationCounts = await _companyReservationCountService.GetTop3CompaniesWithMostReservations("testLOGS", "6 months");

                foreach (var companyReservationCount in companyReservationCounts)
                {
                    Console.WriteLine($"Company Name: {companyReservationCount.CompanyName}");
                    Console.WriteLine($"Reservation Count: {companyReservationCount.ReservationCount}");
                    Console.WriteLine();
                }

                Assert.IsNotNull(companyReservationCounts);

                var query = commandBuilder.BeginDelete("dbo.Logs").Where("Username = 'testLOGS'").Build();

                response = await sqlDAO.SqlRowsAffected(query);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task RetrievingTop3CompanyWithMostReservations_12_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var companyReservationCounts = await _companyReservationCountService.GetTop3CompaniesWithMostReservations("testLOGS", "12 months");

                foreach (var companyReservationCount in companyReservationCounts)
                {
                    Console.WriteLine($"Company Name: {companyReservationCount.CompanyName}");
                    Console.WriteLine($"Reservation Count: {companyReservationCount.ReservationCount}");
                    Console.WriteLine();
                }

                Assert.IsNotNull(companyReservationCounts);

                var query = commandBuilder.BeginDelete("dbo.Logs").Where("Username = 'testLOGS'").Build();

                response = await sqlDAO.SqlRowsAffected(query);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task RetrievingTop3CompanyWithMostReservations_24_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var companyReservationCounts = await _companyReservationCountService.GetTop3CompaniesWithMostReservations("testLOGS", "12 months");

                foreach (var companyReservationCount in companyReservationCounts)
                {
                    Console.WriteLine($"Company Name: {companyReservationCount.CompanyName}");
                    Console.WriteLine($"Reservation Count: {companyReservationCount.ReservationCount}");
                    Console.WriteLine();
                }

                Assert.IsNotNull(companyReservationCounts);

                var query = commandBuilder.BeginDelete("dbo.Logs").Where("Username = 'testLOGS'").Build();

                response = await sqlDAO.SqlRowsAffected(query);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task InsertMostUsedFeature_Success()
        {
            Response response = new Response();
            Random random = new Random();
            int featureNum = random.Next(1, 10);

            response = await _mostUsedFeatureService.InsertUsedFeature("HCGl3rHu5KQyzNKfiLlm7ZMg0eCDSjxs1ZMWWp8E7Uw=", $"Feature {featureNum}");

            Assert.IsNotNull(response);
            Assert.IsFalse(response.HasError);

        }

        [TestMethod]
        public async Task RetrievingTop3FeaturesUsed_6_months_Success()
        {
            Response response = new Response();

            try
            {
                for (int i = 0; i < 25; i++)
                {
                    Random random = new Random();
                    int year = random.Next(23, 24);
                    int month = random.Next(1, 12);
                    int day = random.Next(1, 28);
                    int featureNum = random.Next(1, 12);

                    var parameters = new Dictionary<string, object>
                { 
                    { "FeatureName", $"Feature {featureNum}"},
                    { "hashedUsername", "HCGl3rHu5KQyzNKfiLlm7ZMg0eCDSjxs1ZMWWp8E7Uw=" },
                    { "TimeStamp", $"20{year}-{month}-{day} 11:33:19.070"}
                };

                    commandBuilder = new CustomSqlCommandBuilder();

                    var cmd = commandBuilder.BeginInsert("FeatureAccess")
                                                .Columns(parameters.Keys)
                                                .Values(parameters.Keys)
                                                .AddParameters(parameters)
                                                .Build();

                    response = await sqlDAO.SqlRowsAffected(cmd);

                }

                var features = await _mostUsedFeatureService.GetMostUsedFeatures("testLOGS", "6 months");

                foreach (var feature in features)
                {
                    Console.WriteLine($"Feature Name: {feature.FeatureName}");
                    Console.WriteLine($"Feature Count: {feature.UsageCount}");
                    Console.WriteLine();
                }

                var query = commandBuilder.BeginDelete("dbo.Logs").Where("Username = 'testLOGS'").Build();

                response = await sqlDAO.SqlRowsAffected(query);

                Assert.IsNotNull(features);
                Assert.IsFalse(response.HasError);



            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }


        }

        [TestMethod]
        public async Task RetrievingTop3FeaturesUsed_12_months_Success()
        {
            Response response = new Response();

            try
            {
                for (int i = 0; i < 25; i++)
                {
                    Random random = new Random();
                    int year = random.Next(22, 24);
                    int month = random.Next(1, 12);
                    int day = random.Next(1, 28);
                    int featureNum = random.Next(1,12);

                    var parameters = new Dictionary<string, object>
                {
                    { "FeatureName", $"Feature {featureNum}"},
                    { "hashedUsername", "HCGl3rHu5KQyzNKfiLlm7ZMg0eCDSjxs1ZMWWp8E7Uw=" },
                    { "TimeStamp", $"20{year}-{month}-{day} 11:33:19.070"}
                };

                    commandBuilder = new CustomSqlCommandBuilder();

                    var cmd = commandBuilder.BeginInsert("FeatureAccess")
                                                .Columns(parameters.Keys)
                                                .Values(parameters.Keys)
                                                .AddParameters(parameters)
                                                .Build();

                    response = await sqlDAO.SqlRowsAffected(cmd);

                }

                var features = await _mostUsedFeatureService.GetMostUsedFeatures("testLOGS", "12 months");

                foreach (var feature in features)
                {
                    Console.WriteLine($"Feature Name: {feature.FeatureName}");
                    Console.WriteLine($"Feature Count: {feature.UsageCount}");
                    Console.WriteLine();
                }

                var query = commandBuilder.BeginDelete("dbo.Logs").Where("Username = 'testLOGS'").Build();

                response = await sqlDAO.SqlRowsAffected(query);

                Assert.IsNotNull(features);
                Assert.IsFalse(response.HasError);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }


        }

        [TestMethod]
        public async Task RetrievingTop3FeaturesUsed_24_months_Success()
        {
            Response response = new Response();

            try
            {
                for (int i = 0; i < 25; i++)
                {
                    Random random = new Random();
                    int year = random.Next(21, 24);
                    int month = random.Next(1, 12);
                    int day = random.Next(1, 28);
                    int featureNum = random.Next(1, 12);

                    var parameters = new Dictionary<string, object>
                {
                    { "FeatureName", $"Feature {featureNum}"},
                    { "hashedUsername", "HCGl3rHu5KQyzNKfiLlm7ZMg0eCDSjxs1ZMWWp8E7Uw=" },
                    { "TimeStamp", $"20{year}-{month}-{day} 11:33:19.070"}
                };

                    commandBuilder = new CustomSqlCommandBuilder();

                    var cmd = commandBuilder.BeginInsert("FeatureAccess")
                                                .Columns(parameters.Keys)
                                                .Values(parameters.Keys)
                                                .AddParameters(parameters)
                                                .Build();

                    response = await sqlDAO.SqlRowsAffected(cmd);

                }

                var features = await _mostUsedFeatureService.GetMostUsedFeatures("testLOGS", "24 months");

                foreach (var feature in features)
                {
                    Console.WriteLine($"Feature Name: {feature.FeatureName}");
                    Console.WriteLine($"Feature Count: {feature.UsageCount}");
                    Console.WriteLine();
                }

                var query = commandBuilder.BeginDelete("dbo.Logs").Where("Username = 'testLOGS'").Build();

                response = await sqlDAO.SqlRowsAffected(query);

                Assert.IsNotNull(features);
                Assert.IsFalse(response.HasError);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }


        }

        [TestMethod]
        public async Task RetrievingTop3CompanyWithMostSpaces_6_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var companySpaceCounts = await _companySpaceCountService.GetTop3CompaniesWithMostSpaces("testLOGS", "6 months");

                foreach (var companySpaceCount in companySpaceCounts)
                {
                    Console.WriteLine($"Company Name: {companySpaceCount.CompanyName}");
                    Console.WriteLine($"Space Count: {companySpaceCount.SpaceCount}");
                    Console.WriteLine();
                }

                Assert.IsNotNull(companySpaceCounts);

                var query = commandBuilder.BeginDelete("dbo.Logs").Where("Username = 'testLOGS'").Build();

                response = await sqlDAO.SqlRowsAffected(query);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task RetrievingTop3CompanyWithMostSpaces_12_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var companySpaceCounts = await _companySpaceCountService.GetTop3CompaniesWithMostSpaces("testLOGS", "12 months");

                foreach (var companySpaceCount in companySpaceCounts)
                {
                    Console.WriteLine($"Company Name: {companySpaceCount.CompanyName}");
                    Console.WriteLine($"Space Count: {companySpaceCount.SpaceCount}");
                    Console.WriteLine();
                }

                Assert.IsNotNull(companySpaceCounts);

                var query = commandBuilder.BeginDelete("dbo.Logs").Where("Username = 'testLOGS'").Build();

                response = await sqlDAO.SqlRowsAffected(query);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task RetrievingTop3CompanyWithMostSpaces_24_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var companySpaceCounts = await _companySpaceCountService.GetTop3CompaniesWithMostSpaces("testLOGS", "24 months");

                foreach (var companySpaceCount in companySpaceCounts)
                {
                    Console.WriteLine($"Company Name: {companySpaceCount.CompanyName}");
                    Console.WriteLine($"Space Count: {companySpaceCount.SpaceCount}");
                    Console.WriteLine();
                }

                Assert.IsNotNull(companySpaceCounts);

                var query = commandBuilder.BeginDelete("dbo.Logs").Where("Username = 'testLOGS'").Build();

                response = await sqlDAO.SqlRowsAffected(query);

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task RetrievingRegistrationgCount_6_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var registrationCounts = await _registrationCountService.GetRegistrationCount("testLOGS", "6 months");

                foreach (var registrationCount in registrationCounts)
                {
                    Console.WriteLine($"Month: {registrationCount.Month}");
                    Console.WriteLine($"Year: {registrationCount.Year}");
                    Console.WriteLine($"Failed Registration: {registrationCount.FailedRegistrations}");
                    Console.WriteLine($"Successful Registration: {registrationCount.SuccessfulRegistrations}");
                    Console.WriteLine();
                }

                Assert.IsNotNull(registrationCounts);



            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task RetrievingRegistrationgCount_12_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var registrationCounts = await _registrationCountService.GetRegistrationCount("testLOGS", "12 months");

                foreach (var registrationCount in registrationCounts)
                {
                    Console.WriteLine($"Month: {registrationCount.Month}");
                    Console.WriteLine($"Year: {registrationCount.Year}");
                    Console.WriteLine($"Failed Registration: {registrationCount.FailedRegistrations}");
                    Console.WriteLine($"Successful Registration: {registrationCount.SuccessfulRegistrations}");
                    Console.WriteLine();
                }

                Assert.IsNotNull(registrationCounts);



            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }

        [TestMethod]
        public async Task RetrievingRegistrationgCount_24_months_Success()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));
            Response response = new Response();

            try
            {

                var registrationCounts = await _registrationCountService.GetRegistrationCount("testLOGS", "24 months");

                foreach (var registrationCount in registrationCounts)
                {
                    Console.WriteLine($"Month: {registrationCount.Month}");
                    Console.WriteLine($"Year: {registrationCount.Year}");
                    Console.WriteLine($"Failed Registration: {registrationCount.FailedRegistrations}");
                    Console.WriteLine($"Successful Registration: {registrationCount.SuccessfulRegistrations}");
                    Console.WriteLine();
                }

                Assert.IsNotNull(registrationCounts);



            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                Assert.IsFalse(response.HasError);
            }

        }
    }
}
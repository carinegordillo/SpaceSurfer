﻿using SS.Backend.DataAccess;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;

namespace SS.Backend.SystemObservability
{
    public class SystemObservabilityDAO : ISystemObservabilityDAO
    {
        private ISqlDAO _sqlDAO;
        private ConfigService configService;
        private CustomSqlCommandBuilder commandBuilder;

        public SystemObservabilityDAO(ISqlDAO sqlDao)
        {
            _sqlDAO = sqlDao;
        }

        public async Task<Response> InsertViewDuration(string username, string viewName, int durationInSeconds)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {

                var parameters = new Dictionary<string, object>
                {
                    { "hashedUsername", username },
                    { "viewName", viewName },
                    { "durationInSeconds", durationInSeconds },
                    { "timestamp", DateTime.UtcNow}
                };

                commandBuilder = new CustomSqlCommandBuilder();

                var query = commandBuilder.BeginInsert("ViewDurations")
                                            .Columns(parameters.Keys)
                                            .Values(parameters.Keys)
                                            .AddParameters(parameters)
                                            .Build();


                response = await _sqlDAO.SqlRowsAffected(query);

                if (!response.HasError)
                {

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Insertion Of View Duration In System Observability DAO"
                    };

                    await logger.SaveData(entry);
                }
                else
                {

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Insertion Of View Duration In System Observability DAO"
                    };

                    await logger.SaveData(errorEntry);
                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data Store",
                    description = "Data Access Error in System Observability DAO"
                };

                await logger.SaveData(errorEntry);
            }

            return response;
        }

        public async Task<Response> RetrieveTop3ViewDurations(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;

                if (timeSpan == "6 months")
                {
                    startDate = DateTime.Today.AddMonths(-6);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "12 months")
                {
                    startDate = DateTime.Today.AddMonths(-12);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "24 months")
                {
                    startDate = DateTime.Today.AddMonths(-24);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else
                {

                    response.HasError = true;
                    response.ErrorMessage = "Timespan Was Not Chosen Between 6 Months, 12 Months, 24 Months";

                    // Handle invalid time span
                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Business",
                        description = "Invalid TimeSpan For System Observability"
                    };

                    await logger.SaveData(errorEntry);

                    return response;
                }

                commandBuilder = new CustomSqlCommandBuilder();

                var query = commandBuilder.BeginStoredProcedure("GetTopViewDurations")
                                        .AddParameters(new Dictionary<string, object> { { "StartDate", startDate },{"EndDate", endDate}})
                                        .Build();
                response = await _sqlDAO.ReadSqlResult(query);

                if (!response.HasError)
                {

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Top 3 Retrieval View Durations In DB"
                    };

                    await logger.SaveData(entry);
                }
                else if (response.ErrorMessage == "No rows found.")
                {
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Top 3 Retrieval View Durations In DB, But It Was Empty"
                    };

                    await logger.SaveData(entry);
                }
                else
                {

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Top 3 Retrieval View Durations In DB"
                    };

                    await logger.SaveData(errorEntry);
                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data Store",
                    description = "Data Access Error in System Observability DAO"
                };

                await logger.SaveData(errorEntry);
            }

            return response;

        }

        public async Task<Response> RetrieveLoginsCount(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;

                if (timeSpan == "6 months")
                {
                    startDate = DateTime.Today.AddMonths(-6);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "12 months")
                {
                    startDate = DateTime.Today.AddMonths(-12);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "24 months")
                {
                    startDate = DateTime.Today.AddMonths(-24);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else
                {

                    response.HasError = true;
                    response.ErrorMessage = "Timespan Was Not Chosen Between 6 Months, 12 Months, 24 Months";

                    // Handle invalid time span
                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Business",
                        description = "Invalid TimeSpan For System Observability DAO"
                    };

                    await logger.SaveData(errorEntry);

                    return response;
                }

                commandBuilder = new CustomSqlCommandBuilder();

                var query = commandBuilder.BeginStoredProcedure("GetMonthlyLoginStats")
                                        .AddParameters(new Dictionary<string, object> { { "StartDate", startDate },{"EndDate", endDate}})
                                        .Build();
                response = await _sqlDAO.ReadSqlResult(query);

                if (!response.HasError)
                {

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Retrieval of Login Counts In DB"
                    };

                    await logger.SaveData(entry);
                }
                else if (response.ErrorMessage == "No rows found.")
                {
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Retrieval Of Login Count In DB, But It Was Empty"
                    };

                    await logger.SaveData(entry);
                }
                else
                {

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Retrieval Of Login Count In DB"
                    };

                    await logger.SaveData(errorEntry);
                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data Store",
                    description = "Data Access Error in System Observability DAO"
                };

                await logger.SaveData(errorEntry);
            }

            return response;
        }


        public async Task<Response> RetrieveCompanyReservationsCount(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;

                if (timeSpan == "6 months")
                {
                    startDate = DateTime.Today.AddMonths(-6);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "12 months")
                {
                    startDate = DateTime.Today.AddMonths(-12);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "24 months")
                {
                    startDate = DateTime.Today.AddMonths(-24);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else
                {

                    response.HasError = true;
                    response.ErrorMessage = "Timespan Was Not Chosen Between 6 Months, 12 Months, 24 Months";

                    // Handle invalid time span
                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Business",
                        description = "Invalid TimeSpan For System Observability DAO"
                    };

                    await logger.SaveData(errorEntry);

                    return response;
                }

                commandBuilder = new CustomSqlCommandBuilder();

                var query = commandBuilder.BeginStoredProcedure("GetTopCompaniesByReservations")
                                        .AddParameters(new Dictionary<string, object> { { "StartDate", startDate },{"EndDate", endDate}})
                                        .Build();

                response = await _sqlDAO.ReadSqlResult(query);

                if (!response.HasError)
                {

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Retrieval of Top 3 Company/Facility Reservation Counts In DB"
                    };

                    await logger.SaveData(entry);
                }
                else if (response.ErrorMessage == "No rows found.")
                {
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Retrieval of Top 3 Company/Facility Reservation Counts In DB, but it was empty"
                    };

                    await logger.SaveData(entry);
                }
                else
                {

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Retrieval of Top 3 Company/Facility Reservation Counts In DB"
                    };

                    await logger.SaveData(errorEntry);
                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data Store",
                    description = "Data Access Error in System Observability"
                };

                await logger.SaveData(errorEntry);
            }

            return response;
        }


        public async Task<Response> RetrieveCompanySpaceCount(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;

                if (timeSpan == "6 months")
                {
                    startDate = DateTime.Today.AddMonths(-6);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "12 months")
                {
                    startDate = DateTime.Today.AddMonths(-12);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "24 months")
                {
                    startDate = DateTime.Today.AddMonths(-24);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else
                {

                    response.HasError = true;
                    response.ErrorMessage = "Timespan Was Not Chosen Between 6 Months, 12 Months, 24 Months";

                    // Handle invalid time span
                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Business",
                        description = "Invalid TimeSpan For System Observability DAO"
                    };

                    await logger.SaveData(errorEntry);

                    return response;
                }

                commandBuilder = new CustomSqlCommandBuilder();

                var query = commandBuilder.BeginStoredProcedure("GetTopCompaniesBySpaceCount")
                                        .AddParameters(new Dictionary<string, object> { { "StartDate", startDate },{"EndDate", endDate}})
                                        .Build();


                response = await _sqlDAO.ReadSqlResult(query);

                if (!response.HasError)
                {

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Retrieval of Top 3 Company/Facility Space Counts In DB"
                    };

                    await logger.SaveData(entry);
                }
                else if (response.ErrorMessage == "No rows found.")
                {
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Retrieval of Top 3 Company/Facility Space Counts In DB, but it was empty"
                    };

                    await logger.SaveData(entry);
                }
                else
                {

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Retrieval of Top 3 Company/Facility Space Counts In DB"
                    };

                    await logger.SaveData(errorEntry);
                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data Store",
                    description = "Data Access Error in System Observability DAO"
                };

                await logger.SaveData(errorEntry);
            }

            return response;
        }
        public async Task<Response> InsertUsedFeature(string username, string Feature)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {

                var parameters = new Dictionary<string, object>
                {
                    { "FeatureName", Feature },
                    { "hashedUsername", username },
                    { "timestamp", DateTime.UtcNow}
                };

                commandBuilder = new CustomSqlCommandBuilder();

                var query = commandBuilder.BeginInsert("dbo.FeatureAccess")
                                            .Columns(parameters.Keys)
                                            .Values(parameters.Keys)
                                            .AddParameters(parameters)
                                            .Build();


                response = await _sqlDAO.SqlRowsAffected(query);

                if (!response.HasError)
                {

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Insertion Of Feature Used In System Observability DAO"
                    };

                    await logger.SaveData(entry);
                }
                else
                {

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Insertion Of Feature In System Observability DAO"
                    };

                    await logger.SaveData(errorEntry);
                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data Store",
                    description = "Data Access Error in System Observability"
                };

                await logger.SaveData(errorEntry);
            }

            return response;
        }
        public async Task<Response> RetrieveMostUsedFeatures(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;

                if (timeSpan == "6 months")
                {
                    startDate = DateTime.Today.AddMonths(-6);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "12 months")
                {
                    startDate = DateTime.Today.AddMonths(-12);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "24 months")
                {
                    startDate = DateTime.Today.AddMonths(-24);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else
                {

                    response.HasError = true;
                    response.ErrorMessage = "Timespan Was Not Chosen Between 6 Months, 12 Months, 24 Months";

                    // Handle invalid time span
                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Business",
                        description = "Invalid TimeSpan For System Observability DAO"
                    };

                    await logger.SaveData(errorEntry);

                    return response;
                }


                commandBuilder = new CustomSqlCommandBuilder();
                var query = commandBuilder.BeginStoredProcedure("GetTopFeatures")
                                        .AddParameters(new Dictionary<string, object> { { "StartDate", startDate },{"EndDate", endDate}})
                                        .Build();

                response = await _sqlDAO.ReadSqlResult(query);

                if (!response.HasError)
                {

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Retrieval of Top 3 Most Used Feature Counts In DB"
                    };

                    await logger.SaveData(entry);
                }
                else if (response.ErrorMessage == "No rows found.")
                {
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Retrieval of Top 3 Most Used Feature Counts In DB, but it was empty"
                    };

                    await logger.SaveData(entry);
                }
                else
                {

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Retrieval of Top 3 Most Used Feature Counts In DB"
                    };

                    await logger.SaveData(errorEntry);
                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data Store",
                    description = "Data Access Error in System Observability DAO"
                };

                await logger.SaveData(errorEntry);
            }

            return response;
        }

        public async Task<Response> RetrieveRegistrationsCount(string username, string timeSpan)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            configService = new ConfigService(configFilePath);
            Logger logger = new Logger(new SqlLogTarget(new SqlDAO(configService)));

            Response response = new Response();

            try
            {
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;

                if (timeSpan == "6 months")
                {
                    startDate = DateTime.Today.AddMonths(-6);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "12 months")
                {
                    startDate = DateTime.Today.AddMonths(-12);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else if (timeSpan == "24 months")
                {
                    startDate = DateTime.Today.AddMonths(-24);
                    endDate = DateTime.Today.AddDays(2).AddTicks(-1);
                }
                else
                {

                    response.HasError = true;
                    response.ErrorMessage = "Timespan Was Not Chosen Between 6 Months, 12 Months, 24 Months";

                    // Handle invalid time span
                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Business",
                        description = "Invalid TimeSpan For System Observability DAO"
                    };

                    await logger.SaveData(errorEntry);

                    return response;
                }

                commandBuilder = new CustomSqlCommandBuilder();

                var query = commandBuilder.BeginStoredProcedure("GetMonthlyRegistrations")
                                        .AddParameters(new Dictionary<string, object> { { "StartDate", startDate },{"EndDate", endDate}})
                                        .Build();

                response = await _sqlDAO.ReadSqlResult(query);

                if (!response.HasError)
                {

                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Retrieval of Registration Counts In DB"
                    };

                    await logger.SaveData(entry);
                }
                else if (response.ErrorMessage == "No rows found.")
                {
                    LogEntry entry = new LogEntry()

                    {
                        timestamp = DateTime.UtcNow,
                        level = "Info",
                        username = username,
                        category = "Data Store",
                        description = "Successful Retrieval Of Registration Count In DB, But It Was Empty"
                    };

                    await logger.SaveData(entry);
                }
                else
                {

                    LogEntry errorEntry = new LogEntry()
                    {
                        timestamp = DateTime.UtcNow,
                        level = "Error",
                        username = username,
                        category = "Data Store",
                        description = "Unsuccessful Retrieval Of Registration Count In DB"
                    };

                    await logger.SaveData(errorEntry);
                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;

                LogEntry errorEntry = new LogEntry()
                {
                    timestamp = DateTime.UtcNow,
                    level = "Error",
                    username = username,
                    category = "Data Store",
                    description = "Data Access Error in System Observability DAO"
                };

                await logger.SaveData(errorEntry);
            }

            return response;
        }
    }

}

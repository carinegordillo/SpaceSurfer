using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using SS.Backend.SystemObservability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

builder.Services.AddTransient<ConfigService>(provider => new ConfigService(configFilePath));
builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();
builder.Services.AddTransient<ISystemObservabilityDAO, SystemObservabilityDAO>();
builder.Services.AddTransient<ILoginCountService, LoginCountService>();
builder.Services.AddTransient<IRegistrationCountService, RegistrationCountService>();
builder.Services.AddTransient<IViewDurationService, ViewDurationService>();
builder.Services.AddTransient<IMostUsedFeatureService, MostUsedFeatureService>();
builder.Services.AddTransient<ICompanyReservationCountService, CompanyReservationCountService>();
builder.Services.AddTransient<ICompanySpaceCountService, CompanySpaceCountService>();

builder.Services.AddTransient<GenOTP>();
builder.Services.AddTransient<Hashing>();
builder.Services.AddTransient<Response>();
builder.Services.AddTransient<LogEntry>();
builder.Services.AddTransient<ILogTarget, SqlLogTarget>();
builder.Services.AddTransient<Logger>();
builder.Services.AddTransient<SqlDAO>();


builder.Services.AddTransient<SSAuthService>(provider =>
    new SSAuthService(
        provider.GetRequiredService<GenOTP>(),
        provider.GetRequiredService<Hashing>(),
        provider.GetRequiredService<SqlDAO>(),
        provider.GetRequiredService<Logger>()
    )
);


// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.Use((context, next) =>
{

    context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
    context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
    context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
    context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");


    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Append("Access-Control-Max-Age", "86400");
        context.Response.StatusCode = 204;
        return Task.CompletedTask;
    }

    return next();
});


app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllers();

app.Run();

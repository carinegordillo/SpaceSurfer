using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Data;
using SS.Backend.SharedNamespace;
using SS.Backend.ReservationManagement;
using SS.Backend.ReservationManagers;
using SS.Backend.DataAccess;
using SS.Backend.SpaceManager;
using SS.Backend.EmailConfirm;
using SS.Backend.Security;
using SS.Backend.Waitlist;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SS.Backend.Services.LoggingService;
using System.Text;





var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");


//Dao Setup
builder.Services.AddTransient<ConfigService>(provider =>new ConfigService(configFilePath));
builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();
builder.Services.AddTransient<ILogTarget, SqlLogTarget>();
builder.Services.AddTransient<SS.Backend.Services.LoggingService.ILogger, Logger>();

//Repo Setup
builder.Services.AddTransient<IReservationManagementRepository, ReservationManagementRepository>();

//Services Setup
builder.Services.AddTransient<IReservationCreatorService, ReservationCreatorService>();
builder.Services.AddTransient<IReservationCancellationService, ReservationCancellationService>();
builder.Services.AddTransient<IReservationModificationService, ReservationModificationService>();
builder.Services.AddTransient<IReservationReadService, ReservationReadService>();
builder.Services.AddTransient<IReservationValidationService, ReservationValidationService>();
builder.Services.AddTransient<IReservationStatusUpdater, ReservationStatusUpdater>();

//email confirmation setup
builder.Services.AddTransient<IEmailConfirmDAO, EmailConfirmDAO>();
builder.Services.AddTransient<IEmailConfirmService, EmailConfirmService>();
builder.Services.AddTransient<IEmailConfirmSender,EmailConfirmSender>();
builder.Services.AddTransient<IEmailConfirmList,EmailConfirmList>();
builder.Services.AddTransient<IConfirmationDeletion,ConfirmationDeletion>();

//Mangers Setup
builder.Services.AddTransient<IReservationCreationManager, ReservationCreationManager>();
builder.Services.AddTransient<IReservationCancellationManager, ReservationCancellationManager>();
builder.Services.AddTransient<IReservationModificationManager, ReservationModificationManager>();
builder.Services.AddTransient<IReservationReaderManager, ReservationReaderManager>();
builder.Services.AddTransient<IAvailibilityDisplayManager, AvailabilityDisplayManager>();

//spaceMAnger setup
builder.Services.AddTransient<ISpaceManagerDao, SpaceManagerDao>();
builder.Services.AddTransient<ISpaceReader, SpaceReader>();

//security
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
builder.Services.AddSwaggerGen();

var app = builder.Build();
// get localhost cofig file path
var corsConfigFilePath = Path.Combine(projectRootDirectory, "Configs", "originsConfig.json");
string allowedOrigin= "coudl not connect to config file";

if (File.Exists(corsConfigFilePath))
{
    string configJson = File.ReadAllText(corsConfigFilePath);
    
    JsonDocument doc = JsonDocument.Parse(configJson);
    JsonElement root = doc.RootElement.GetProperty("Origin");
    allowedOrigin = root.GetProperty("CorsAllowedOrigin").GetString() ?? "NA";
}

Console.WriteLine("Cors Allowed Origin: ");
Console.WriteLine(allowedOrigin);
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers[HeaderNames.Origin].ToString();
    var allowedOrigins = new[] {allowedOrigin};

    if (!string.IsNullOrEmpty(origin) && allowedOrigins.Contains(origin))
    {
        context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Accept");
        context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
    }
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.CompleteAsync();
    }
    else
    {
        await next();
    }
});

app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllers();

app.Run();

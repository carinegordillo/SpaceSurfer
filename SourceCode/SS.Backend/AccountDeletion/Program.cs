using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.Services.DeletingService;
using SS.Backend.Services.LoggingService;
using Microsoft.Net.Http.Headers;
using SS.Backend.SharedNamespace;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

builder.Services.AddTransient<ConfigService>(provider => new ConfigService(configFilePath));
builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();
builder.Services.AddTransient<IDatabaseHelper, DatabaseHelper>();
builder.Services.AddTransient<IAccountDeletion, AccountDeletion>();
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

 
 
app.Use((context, next) =>
{
    var origin = context.Request.Headers[HeaderNames.Origin].ToString();

      
    
    var allowedOrigins = new[] {allowedOrigin};

    context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
    context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
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

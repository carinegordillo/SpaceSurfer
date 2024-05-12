using SS.Backend.SpaceManager;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.Waitlist;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

builder.Services.AddTransient<ConfigService>(provider => new ConfigService(configFilePath));
builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<SS.Backend.Services.LoggingService.ILogger, Logger>();

builder.Services.AddTransient<ISpaceCreation, SpaceCreation>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();
builder.Services.AddTransient<ISpaceManagerDao, SpaceManagerDao>();
builder.Services.AddTransient<ISpaceModification, SpaceModification>();
builder.Services.AddTransient<ConfigService>(provider =>
    new ConfigService(configFilePath));
builder.Services.AddTransient<SqlDAO>();
builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<GenOTP>();
builder.Services.AddTransient<Hashing>();
builder.Services.AddTransient<Response>();
builder.Services.AddTransient<LogEntry>();
builder.Services.AddTransient<ILogTarget, SqlLogTarget>();
builder.Services.AddTransient<Logger>();
builder.Services.AddTransient<SSAuthService>(provider =>
    new SSAuthService(
        provider.GetRequiredService<GenOTP>(),
        provider.GetRequiredService<Hashing>(),
        provider.GetRequiredService<SqlDAO>(),
        provider.GetRequiredService<Logger>()
    )
);
builder.Services.AddTransient<WaitlistService>(provider =>
    new WaitlistService(
        provider.GetRequiredService<SqlDAO>()
    )
);

var app = builder.Build();

/// get localhost cofig file path
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

    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Append("Access-Control-Max-Age", "86400");
        context.Response.StatusCode = 204;
        return Task.CompletedTask;
    }

    return next();
});

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
//     app.UseDeveloperExceptionPage();
// }

// app.UseHttpsRedirection();

app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllers();

app.Run();
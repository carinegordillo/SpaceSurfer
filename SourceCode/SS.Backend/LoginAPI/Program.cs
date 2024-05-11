using Microsoft.Net.Http.Headers;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using SS.Backend.Services.ArchivingService ;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();



var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");


builder.Services.AddTransient<ConfigService>(provider =>
    new ConfigService(configFilePath));
builder.Services.AddTransient<SqlDAO>();
builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();
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


//adding archiving 

var archivingConfigFilePath = Path.Combine(projectRootDirectory, "Configs", "archivingconfig.json");

builder.Services.AddTransient<ITargetArchivingDestination, S3ArchivingDestination>();
builder.Services.AddSingleton(provider =>
{
    var archivingTarget = provider.GetRequiredService<ITargetArchivingDestination>();
    return new ArchivingService(archivingTarget, archivingConfigFilePath);
});


builder.Services.AddControllers();

var app = builder.Build();

var archivingService = app.Services.GetRequiredService<ArchivingService>();

app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("Application is starting. ArchivingService is being started...");
    archivingService.Start();
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Application is stopping. ArchivingService is being stopped...");
    archivingService.Stop();
});

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

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
        context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
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



app.UseStaticFiles();

app.MapControllers();

app.Run();

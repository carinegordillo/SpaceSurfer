using Microsoft.Net.Http.Headers;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using SS.Backend.Services.ArchivingService ;
using System.Text;

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

app.Lifetime.ApplicationStarted.Register(() => {
    Console.WriteLine("Application is starting. ArchivingService is being started...");
    archivingService.Start();
});

app.Lifetime.ApplicationStopping.Register(() => {
    Console.WriteLine("Application is stopping. ArchivingService is being stopped...");
    archivingService.Stop();
});

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.Use(async (context, next) =>
{
    // Get the origin header from the request
    var origin = context.Request.Headers[HeaderNames.Origin].ToString();

    var allowedOrigins = new[] { "http://localhost:3000" };


    if (!string.IsNullOrEmpty(origin) && allowedOrigins.Contains(origin))
    {
        context.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, origin);
        context.Response.Headers.Append(HeaderNames.AccessControlAllowMethods, "GET, POST, OPTIONS");
        context.Response.Headers.Append(HeaderNames.AccessControlAllowHeaders, "Content-Type, Accept");
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

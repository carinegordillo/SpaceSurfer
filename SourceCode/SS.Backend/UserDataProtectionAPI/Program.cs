using SS.Backend.DataAccess;
using SS.Backend.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using SS.Backend.UserDataProtection;
using SS.Backend.Services.DeletingService;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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
//builder.Services.AddTransient<IAccountDeletion, AccountDeletion>();
builder.Services.AddTransient<SSAuthService>(provider =>
    new SSAuthService(
        provider.GetRequiredService<GenOTP>(),
        provider.GetRequiredService<Hashing>(),
        provider.GetRequiredService<SqlDAO>(),
        provider.GetRequiredService<Logger>()
    )
);
builder.Services.AddTransient<UserDataProtection>(provider =>
    new UserDataProtection(
        provider.GetRequiredService<SqlDAO>(),
        provider.GetRequiredService<GenOTP>(),
        provider.GetRequiredService<Hashing>(),
        provider.GetRequiredService<Logger>()
    )
);

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

 
 
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers[HeaderNames.Origin].ToString();

      
     

    var allowedOrigins = new[] {allowedOrigin};

    if (!string.IsNullOrEmpty(origin) && allowedOrigins.Contains(origin))
    {
        context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
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

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
//     app.UseDeveloperExceptionPage();
// }

//app.UseHttpsRedirection();

app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllers();

app.Run();
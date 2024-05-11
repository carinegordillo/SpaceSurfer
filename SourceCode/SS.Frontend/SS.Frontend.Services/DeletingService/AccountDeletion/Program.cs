using Microsoft.Net.Http.Headers;
using SS.Backend.DataAccess;
using SS.Backend.Services.DeletingService;
using SS.Backend.SharedNamespace;
// using SS.Backend.Security;
using SS.Backend.Services.LoggingService;

// Creates a new web application builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// Add transient services for logging
// builder.Services.AddTransient<SqlLogTarget>();

// Adding authorization services


// Adding configuration service
builder.Services.AddSingleton(new ConfigService(Path.Combine("C:/Users/brand/Documents/GitHub/SpaceSurfer/SourceCode/SS.Backend/config.local.txt")));

// Register services for dependency injection
// builder.Services.AddTransient<GenOTP>();
// builder.Services.AddTransient<Hashing>();
// builder.Services.AddTransient<SqlDAO>();
// builder.Services.AddTransient<Logger>();
// builder.Services.AddTransient<IAuthenticator, SSAuthService>();
// builder.Services.AddTransient<IAuthorizer, SSAuthService>();
// builder.Services.AddTransient<IAccountDeletion, AccountDeletion>();


// builds the application
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();

// Middleware for authentication

// Middleware for Authorization

// Middleware to handle CORS preflight requests
app.Use(async (context, next) =>
{
    // Get the origin header from the request
    var origin = context.Request.Headers[HeaderNames.Origin].ToString();

    var allowedOrigins = new[] { "http://localhost:3000" };


    if (!string.IsNullOrEmpty(origin) && allowedOrigins.Contains(origin))
    {
        context.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, origin);
        context.Response.Headers.Append(HeaderNames.AccessControlAllowMethods, "POST, OPTIONS");
        context.Response.Headers.Append(HeaderNames.AccessControlAllowHeaders, "Content-Type, Accept");
        context.Response.Headers.Append(HeaderNames.AccessControlAllowCredentials, "true");
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

// Maps the controller endpoints
app.MapControllers();

// runs the application
app.Run();

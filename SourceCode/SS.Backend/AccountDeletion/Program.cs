using Microsoft.Net.Http.Headers;
using SS.Backend.DataAccess;
using SS.Backend.Services.DeletingService;

// Creates a new web application builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add transient services for logging
// builder.Services.AddTransient<SqlLogTarget>();

// Adding authorization services
builder.Services.AddAuthorization();

//// Get the root directory where the application is located
//var rootDirectory = Directory.GetParent(path: Directory.GetCurrentDirectory()).Parent.FullName;

//// Specify the relative path to your config file from the root directory
//var configFilePath = Path.Combine(rootDirectory, "Configs", "config.local.txt");

// Base Directory of the file
var baseDirectory = AppContext.BaseDirectory;

var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));

var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

// Adding configuration service
builder.Services.AddSingleton(new ConfigService(configFilePath));

// Register services for dependency injection
builder.Services.AddTransient<IAccountDeletion, AccountDeletion>();


// builds the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enables Swagger and the SwaggerUI
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

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

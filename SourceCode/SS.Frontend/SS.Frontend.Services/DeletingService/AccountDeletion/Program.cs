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

// Adding configuration service
builder.Services.AddSingleton(new ConfigService(Path.Combine("C:/Users/brand/Documents/GitHub/SpaceSurfer/SourceCode/SS.Backend/config.local.txt")));

// Register services for dependency injection
builder.Services.AddTransient<IAccountDeletion, AccountDeletion>();
builder.Services.AddTransient<IDatabaseHelper, DatabaseHelper>();

//// Registers an instance of Credential for database access
//builder.Services.AddTransient<ConfigService>(provider =>
//{
//    // Provide appropriate values for user and pass here
//    return new Credential("sa", "grfragk");
//});


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

    if (!string.IsNullOrEmpty(origin))
    {
        context.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, origin);
        context.Response.Headers.Append(HeaderNames.AccessControlAllowMethods, "GET, POST, PUT, DELETE, OPTIONS");
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

// Maps the controller endpoints
app.MapControllers();

// runs the application
app.Run();

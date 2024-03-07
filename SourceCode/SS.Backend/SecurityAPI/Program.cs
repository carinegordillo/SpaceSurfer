using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://spacesurfers.auth.com/", //not sure if this is right
        ValidAudience = "spacesurfers", //not sure if this is right
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("g3LQ4A6$h#Z%2&t*BKs@v7GxU9$FqNpDrn"))
    };
});

/*
builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
*/

builder.Services.AddAuthorization();

var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
builder.Services.AddTransient<ConfigService>(provider =>new ConfigService(configFilePath));

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
        provider.GetRequiredService<Logger>(),
        "g3LQ4A6$h#Z%2&t*BKs@v7GxU9$FqNpDrn"
    )
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Use((httpContext, next) =>
{
    httpContext.Response.Headers.AccessControlAllowOrigin = "http://localhost:3000/";
    httpContext.Response.Headers.AccessControlAllowMethods = "GET, POST, OPTIONS, PUT, DELETE";
    httpContext.Response.Headers.AccessControlAllowHeaders = "*";   //dont use *, specify what they have access to (comma separarted list)
    httpContext.Response.Headers.AccessControlAllowCredentials = "true";

    return next();
});

//CORS implementation (set vatiable values)
app.Use((httpContext, next) =>
{
    //address browser's preflight OPTIONS request
    if(httpContext.Request.Method == nameof(HttpMethod.Options).ToUpperInvariant())
    {
        httpContext.Response.StatusCode = 204; //everythign okay with no payload
        httpContext.Response.Headers.AccessControlAllowOrigin = "http://localhost:3000/";
        httpContext.Response.Headers.AccessControlAllowMethods = "GET, POST, OPTIONS, PUT, DELETE";
        httpContext.Response.Headers.AccessControlAllowHeaders = "*";   //dont use *, specify what they have access to (comma separarted list)
        httpContext.Response.Headers.AccessControlAllowCredentials = "true";

        return Task.CompletedTask; //terminate HTTP request
    }
    return next();
});

//app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

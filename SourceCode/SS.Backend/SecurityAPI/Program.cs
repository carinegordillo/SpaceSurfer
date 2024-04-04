// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
// using SS.Backend.DataAccess;
// using SS.Backend.Security;
// using SS.Backend.Services.LoggingService;
// using SS.Backend.SharedNamespace;
// using System.Text;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Net.Http.Headers;
// using SecurityAPI;
// using SS.Backend.EmailConfirm;


// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddControllers();

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = "https://spacesurfers.auth.com/", //not sure if this is right
//         ValidAudience = "spacesurfers", //not sure if this is right
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("g3LQ4A6$h#Z%2&t*BKs@v7GxU9$FqNpDrn")),
        
//         // Custom validation for additional claims
//         //LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
//         //AudienceValidator = (audiences, securityToken, validationParameters) => audiences.Contains("spacesurfers")
//     };

// });

// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("RequireClaimPolicy", policy =>
//         policy.RequireClaim("YourClaimType", "ExpectedValue"));
// });

// //builder.Services.AddAuthorization();

// var baseDirectory = AppContext.BaseDirectory;
// var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
// var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
// builder.Services.AddTransient<ConfigService>(provider =>new ConfigService(configFilePath));

// builder.Services.AddTransient<IEmailConfirmDAO, EmailConfirmDAO>();
// builder.Services.AddTransient<IEmailConfirmService, EmailConfirmService>();
// builder.Services.AddTransient<SqlDAO>();
// builder.Services.AddTransient<ISqlDAO, SqlDAO>();
// builder.Services.AddTransient<CustomSqlCommandBuilder>();
// builder.Services.AddTransient<GenOTP>();
// builder.Services.AddTransient<Hashing>();
// builder.Services.AddTransient<Response>();
// builder.Services.AddTransient<LogEntry>();
// builder.Services.AddTransient<ILogTarget, SqlLogTarget>();
// builder.Services.AddTransient<Logger>();
// builder.Services.AddTransient<SSAuthService>(provider =>
//     new SSAuthService(
//         provider.GetRequiredService<GenOTP>(),
//         provider.GetRequiredService<Hashing>(),
//         provider.GetRequiredService<SqlDAO>(),
//         provider.GetRequiredService<Logger>()//,
//         //"g3LQ4A6$h#Z%2&t*BKs@v7GxU9$FqNpDrn"
//     )
// );

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// app.Use((httpContext, next) =>
// {
//     //var origin = httpContext.Request.Headers[HeaderNames.Origin].ToString();

//     const string allowedOrigin = "http://localhost:8080";

//     httpContext.Response.Headers.Add("Access-Control-Allow-Origin", allowedOrigin);
//     httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, DELETE");
//     httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept"); // Specify allowed headers
//     httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

//     // Handle preflight OPTIONS request
//     if (httpContext.Request.Method.Equals("OPTIONS", StringComparison.InvariantCultureIgnoreCase))
//     {
//         httpContext.Response.Headers.Add("Access-Control-Max-Age", "86400"); // Preflight cache duration
//         httpContext.Response.StatusCode = 204; // No Content
//         return Task.CompletedTask;
//     }

//     return next();
// });


// //app.UseStaticFiles();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
//     app.UseDeveloperExceptionPage();
// }

// //app.UseHttpsRedirection();
// //app.UseMiddleware<AuthZMiddleware>();
// app.UseAuthentication();
// app.UseAuthorization();

// app.UseCors("AllowSpecificOrigin");

// app.MapControllers();

// app.Run();

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System;
using System.IO;
using System.Linq;
using System.Text;
using SS.Backend.EmailConfirm;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

builder.Services.AddTransient<IEmailConfirmDAO, EmailConfirmDAO>();
builder.Services.AddTransient<IEmailConfirmService, EmailConfirmService>();
builder.Services.AddTransient<IEmailConfirmSender, EmailConfirmSender>();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.UseMiddleware<AuthenticationMiddleware>();
//app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllers();

app.Run();

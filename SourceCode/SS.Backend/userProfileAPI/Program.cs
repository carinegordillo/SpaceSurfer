using SS.Backend.UserManagement;
using System.Data;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();




var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");



builder.Services.AddTransient<ConfigService>(provider =>new ConfigService(configFilePath));


builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();
builder.Services.AddTransient<IUserManagementDao, UserManagementDao>();
builder.Services.AddTransient<IAccountRecoveryModifier, AccountRecoveryModifier>();
builder.Services.AddTransient<IProfileModifier, ProfileModifier>();
builder.Services.AddTransient<IAccountRecovery, AccountRecovery>();

//security

builder.Services.AddTransient<GenOTP>();
builder.Services.AddTransient<Hashing>();
builder.Services.AddTransient<Response>();
builder.Services.AddTransient<LogEntry>();
builder.Services.AddTransient<ILogTarget, SqlLogTarget>();
builder.Services.AddTransient<SS.Backend.Services.LoggingService.ILogger,Logger>();
builder.Services.AddTransient<Logger>();
builder.Services.AddTransient<SqlDAO>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Use((context, next) =>
{
    
    context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
    context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, OPTIONS");
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




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    
    
}




app.UseHttpsRedirection();

//app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllers();

app.Run();

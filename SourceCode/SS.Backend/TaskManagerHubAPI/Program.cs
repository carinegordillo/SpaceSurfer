var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Data;
using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;
using SS.Backend.TaskManagerHub;
// using SS.Backend.ReservationManagers;



using SS.Backend.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SS.Backend.Services.LoggingService;
using System.Text;





var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");


//Dao Setup
builder.Services.AddTransient<ConfigService>(provider =>new ConfigService(configFilePath));
builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();

//Repo Setup
builder.Services.AddTransient<ITaskManagerHubRepo, TaskManagerHubRepo>();

//Services Setup
builder.Services.AddTransient<ITaskManagerHubService, TaskManagerHubServiceService>();
builder.Services.AddTransient<ITaskManagerHubManager, TaskManagerHubManager>();

//Mangers Setup
// builder.Services.AddTransient<IReservationCreationManager, ReservationCreationManager>();


//security

builder.Services.AddTransient<GenOTP>();
builder.Services.AddTransient<Hashing>();
builder.Services.AddTransient<Response>();
builder.Services.AddTransient<LogEntry>();
builder.Services.AddTransient<ILogTarget, SqlLogTarget>();
builder.Services.AddTransient<Logger>();
builder.Services.AddTransient<SqlDAO>();


builder.Services.AddTransient<SSAuthService>(provider =>
    new SSAuthService(
        provider.GetRequiredService<GenOTP>(),
        provider.GetRequiredService<Hashing>(),
        provider.GetRequiredService<SqlDAO>(),
        provider.GetRequiredService<Logger>()
    )
);

// //waitlist
// builder.Services.AddTransient<WaitlistService>(provider =>
//     new WaitlistService(
//         provider.GetRequiredService<SqlDAO>()
//     )
// );


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


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
app.UseAuthorization();
app.MapControllers();
app.Run();

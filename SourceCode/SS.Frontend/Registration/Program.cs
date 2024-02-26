// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.

// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();

// app.Run();

using SS.Backend.Services.AccountCreationService;
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Reflection;
using SS.Backend.Services.LoggingService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// builder.Services.AddTransient<Credential>();

// builder.Services.AddTransient<ConfigService>(provider =>
//     new ConfigService(Path.Combine(AppContext.BaseDirectory, "config.local.txt")));
builder.Services.AddTransient<ConfigService>(provider =>
    new ConfigService("config.local.txt")); // Directly use the relative path
builder.Services.AddTransient<SqlDAO>();
builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();
builder.Services.AddTransient<Hashing>();
builder.Services.AddTransient<Response>();
builder.Services.AddTransient<LogEntry>();
builder.Services.AddTransient<ILogTarget, SqlLogTarget>();
// builder.Services.AddTransient<SealedPepperDAO>();


// Add services to the container.
builder.Services.AddTransient<IAccountCreation, AccountCreation>();

builder.Services.AddRazorPages();

var app = builder.Build();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
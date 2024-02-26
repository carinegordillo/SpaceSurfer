using SS.Backend.DataAccess;
using SS.Backend.Services;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add transient services for logging
// builder.Services.AddTransient<SqlLogTarget>();

builder.Services.AddAuthorization();

builder.Services.AddSingleton<ConfigService>(new ConfigService(Path.Combine(AppContext.BaseDirectory, "config.local.txt")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

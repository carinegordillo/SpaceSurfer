using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddSingleton<ConfigService>(new ConfigService(Path.Combine(AppContext.BaseDirectory, "config.local.txt")));
builder.Services.AddScoped<SqlDAO>();

builder.Services.AddTransient<SqlLogTarget>();
builder.Services.AddTransient<IAuthenticator, SSAuthService>(provider =>
    new SSAuthService(new GenOTP(), new Hashing(),
    provider.GetRequiredService<SqlDAO>(), new Logger(provider.GetRequiredService<SqlLogTarget>()), "g3LQ4A6$h#Z%2&t*BKs@v7GxU9$FqNpDrn"));


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

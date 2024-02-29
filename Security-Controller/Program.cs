using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
/*
builder.Services.AddTransient<SqlDAO>();

//builder.Services.AddTransient<Credential>();      //need help with transient order
builder.Services.AddTransient<Hashing>();

builder.Services.AddTransient<GenOTP>();
builder.Services.AddTransient<SSAuthService>();
*/

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

/*
builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
*/

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

//builder.Services.AddAuthorization();

//builder.Services.AddTransient<Credential>();
builder.Services.AddTransient<ConfigService>(provider =>
    new ConfigService(Path.Combine(AppContext.BaseDirectory, "config.local.txt")));
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
        provider.GetRequiredService<Logger>()//,
        //"g3LQ4A6$h#Z%2&t*BKs@v7GxU9$FqNpDrn"
    )
);

var app = builder.Build();
app.UseStaticFiles();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


//app.UseHttpsRedirection();
//app.UseAuthorization();

app.MapControllers();

app.Run();

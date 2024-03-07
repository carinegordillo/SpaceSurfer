using SS.Backend.UserManagement;
using SS.Backend.DataAccess;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

// Add services to the container.
//builder.Services.AddTransient<ISqlDAO, SealedSqlDAO>();



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


// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Use((context, next) =>
{
    
    context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Axios-Demo, Space-Surfer-Header");
    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

    
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Max-Age", "86400"); 
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

app.UseAuthorization();

app.MapControllers();

app.Run();

using SS.Backend.SpaceManager;
using SS.Backend.DataAccess;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");

builder.Services.AddTransient<ConfigService>(provider =>new ConfigService(configFilePath));

builder.Services.AddTransient<ISqlDAO, SqlDAO>();

builder.Services.AddTransient<ISpaceCreation, SpaceCreation>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();
builder.Services.AddTransient<ISpaceManagerDao, SpaceManagerDao>();
builder.Services.AddTransient<ISpaceModification, SpaceModification>();


var app = builder.Build();

// Manually handle CORS
app.Use(async (context, next) =>
{
    // Set the necessary headers for CORS
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");

    // Handle the preflight request
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});

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


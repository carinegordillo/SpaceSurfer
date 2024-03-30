using SS.Backend.UserManagement;
using SS.Backend.DataAccess;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

// Add services to the container.
//builder.Services.AddTransient<ISqlDAO, SealedSqlDAO>();



var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");


//Dao Setup
builder.Services.AddTransient<ConfigService>(provider =>new ConfigService(configFilePath));
builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();

//Repo Setup
builder.Services.AddTransient<IReservationManagementRepository, ReservationManagementRepository>();

//Services Setup
builder.services.AddTransient<IReservationCreatorService, ReservationCreatorService>();
builder.services.AddTransient<IReservationCancellationService, ReservationCancellationService>();
builder.services.AddTransient<IReservationModificationService, ReservationModificationService>();
builder.services.AddTransient<IReservationReaderService, ReservationReaderService>();

//Mangers Setup
builder.Services.AddTransient<IReservationCreationManager, ReservationCreationManager>();
builder.Services.AddTransient<IReservationCancellationManager, ReservationCancellationManager>();
builder.Services.AddTransient<IReservationModificationManager, ReservationModificationManager>();
builder.Services.AddTransient<IReservationReaderManager, ReservationReaderManager>();



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

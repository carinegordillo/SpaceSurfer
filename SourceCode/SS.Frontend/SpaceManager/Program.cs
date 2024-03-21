using SS.Backend.SpaceManager;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<ISpaceCreation, SpaceCreation>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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




// using SS.Backend.Services.AccountCreationService;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllers();
// builder.Services.AddTransient<IAccountCreation, AccountCreation>();

// // Define a CORS policy
// const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// // Add CORS services
// builder.Services.AddCors();

// var app = builder.Build();

// // Enable CORS
// app.UseCors(policy =>
// {
//     policy.AllowAnyOrigin();
//     policy.AllowAnyHeader();
//     policy.AllowAnyMethod();
// });

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
//     app.UseDeveloperExceptionPage();
// }

// app.UseHttpsRedirection();
// app.UseAuthorization();

// app.MapControllers();

// app.Run();
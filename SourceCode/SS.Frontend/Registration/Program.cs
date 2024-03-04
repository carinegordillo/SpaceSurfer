using SS.Backend.Services.AccountCreationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<IAccountCreation, AccountCreation>();

// Define a CORS policy
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS services
builder.Services.AddCors();

var app = builder.Build();

// Enable CORS
app.UseCors(policy =>
{
    policy.AllowAnyOrigin();
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
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
// using SS.Backend.DataAccess;
// using SS.Backend.SharedNamespace;
// using System.Reflection;
// using SS.Backend.Services.LoggingService;
// using Microsoft.Extensions.FileProviders;


// var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddControllers();

// builder.Services.AddTransient<ConfigService>(provider =>
//     new ConfigService("config.local.txt")); // Directly use the relative path
// builder.Services.AddTransient<SqlDAO>();
// builder.Services.AddTransient<ISqlDAO, SqlDAO>();
// builder.Services.AddTransient<CustomSqlCommandBuilder>();
// builder.Services.AddTransient<Hashing>();
// builder.Services.AddTransient<Response>();
// builder.Services.AddTransient<LogEntry>();
// builder.Services.AddTransient<ILogTarget, SqlLogTarget>();
// builder.Services.AddTransient<SS.Backend.SharedNamespace.Credential>(provider => 
// {
//     // Create and return a new Credential instance, possibly using other services
//     return new SS.Backend.SharedNamespace.Credential("sa", "kalynn");
// });
// builder.Services.AddTransient<IAccountCreation, AccountCreation>();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowSpecificOrigin",
//         builder => builder.WithOrigins("http://localhost:3000")
//             .AllowAnyHeader()
//             .AllowAnyMethod());
// });

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
//     app.UseDeveloperExceptionPage();
    
    
// }


// app.UseHttpsRedirection();

// app.UseCors("AllowSpecificOrigin");

// app.UseAuthorization();

// app.MapControllers();

// app.Run();



// var app = builder.Build();
// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new PhysicalFileProvider(
//         Path.Combine(Directory.GetCurrentDirectory(), "pages")),
//     RequestPath = "/Pages"
// });
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.MapControllers();
// app.Run();

// app.MapPost("/", () => 
// {
//     return "<h2>This is dynamic POST html<h2>";
// });

// app.MapGet("/", () => 
// {
//     return "<h2>Yjhis is dynamic GET html<h2>";
// });
// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();
// app.MapRazorPages();

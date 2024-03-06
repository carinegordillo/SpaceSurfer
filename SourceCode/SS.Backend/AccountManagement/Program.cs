using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.UserManagement;
using SS.Backend.DataAccess;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

// Add services to the container.
//builder.Services.AddTransient<ISqlDAO, SealedSqlDAO>();
builder.Services.AddTransient<ConfigService>(provider =>
    new ConfigService(Path.Combine("//Users/carinegordillo/config.txt")));//AppContext.BaseDirectory, "config.local.txt")));
builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();
builder.Services.AddTransient<IUserManagementDao, UserManagementDao>();
builder.Services.AddTransient<IAccountRecoveryModifier, AccountRecoveryModifier>();
builder.Services.AddTransient<IProfileModifier, ProfileModifier>();
builder.Services.AddTransient<IAccountRecovery, AccountRecovery>();

// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
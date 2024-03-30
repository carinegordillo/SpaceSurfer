using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;
using SS.Backend.Security;
using SS.Backend.DataAccess;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//dao
var baseDirectory = AppContext.BaseDirectory;
var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
builder.Services.AddTransient<ConfigService>(provider =>new ConfigService(configFilePath));

builder.Services.AddTransient<ISqlDAO, SqlDAO>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();



//service setup
builder.Services.AddTransient<ISpaceManagerDao, SpaceManagerDao>();
builder.Services.AddTransient<ISpaceReader, SpaceReader>();


var app = builder.Build();




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


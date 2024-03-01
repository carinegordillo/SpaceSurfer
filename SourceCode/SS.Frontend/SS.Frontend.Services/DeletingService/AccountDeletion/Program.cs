// Program.cs
using Microsoft.Net.Http.Headers;
using SS.Backend.DataAccess;
using SS.Backend.Services.DeletingService;
using SS.Backend.SharedNamespace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add transient services for logging
// builder.Services.AddTransient<SqlLogTarget>();

builder.Services.AddAuthorization();

builder.Services.AddSingleton(new ConfigService(Path.Combine(AppContext.BaseDirectory, "config.local.txt")));

builder.Services.AddTransient<IAccountDeletion, AccountDeletion>();
builder.Services.AddTransient<IDatabaseHelper, DatabaseHelper>();
builder.Services.AddTransient<CustomSqlCommandBuilder>();
builder.Services.AddTransient<SqlDAO>();
builder.Services.AddTransient<SealedSqlDAO>();
builder.Services.AddTransient<Credential>(provider =>
{
    // Provide appropriate values for user and pass here
    return new Credential("sa", "grfragk");
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.Use(async (httpContext, next) =>
{

    await next();

    // Explicitly only wanting code to execite on the way out of pipeline (Response/outbound direction)
    if (httpContext.Response.Headers.ContainsKey(HeaderNames.XPoweredBy))
    {
        httpContext.Response.Headers.Remove(HeaderNames.XPoweredBy);
    }

    httpContext.Response.Headers.Server = "";

});

app.Use((httpContext, next) =>
{
    if (httpContext.Request.Method.ToUpper() == nameof(HttpMethod.Options).ToUpper() && httpContext.Request.Headers.XRequestedWith == "XMLHttpRequest")
    {
        var allowedMethods = new List<string>()
        {
            HttpMethods.Get,
            HttpMethods.Post,
            HttpMethods.Options,
            HttpMethods.Head
        };

        httpContext.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, "*");
        httpContext.Response.Headers.AccessControlAllowMethods = string.Join(",", allowedMethods); // "GET, POST, OPTIONS, HEAD"
        httpContext.Response.Headers.AccessControlAllowHeaders = "*";
        httpContext.Response.Headers.AccessControlMaxAge = TimeSpan.FromHours(2).Seconds.ToString();
    }
    next.Invoke(httpContext);

    return next(httpContext);
});

app.MapControllers();

app.Run();

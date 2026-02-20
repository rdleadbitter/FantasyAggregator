using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FantasyAggregatorApp.Data;
using Microsoft.Extensions.Configuration;

// HOSTING: Local development
// dotnet run
// Swagger: http://localhost:5196/swagger/index.html

var builder = WebApplication.CreateBuilder(args);

// Allow CORS from localhost (for the client)
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:5002", "http://localhost:5003");
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("LocalDev");
app.UseAuthorization();
app.MapControllers();

var config = new ConfigurationBuilder()
    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

DbConnector.Init(config.GetValue<string>("ConnectionString") ?? config["ConnectionString"]);


app.Run();
